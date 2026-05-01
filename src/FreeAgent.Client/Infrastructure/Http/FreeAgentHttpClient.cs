using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FreeAgent.Client.Infrastructure.Authentication;
using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// HTTP client for FreeAgent API with authentication and rate limiting support.
/// </summary>
public class FreeAgentHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly FreeAgentOAuthClient? _oauthClient;
    private OAuthTokenResponse? _currentToken;
    private readonly SemaphoreSlim _rateLimitSemaphore = new(1, 1);
    private readonly SemaphoreSlim _tokenRefreshSemaphore = new(1, 1);
    private readonly FreeAgentHttpClientOptions _options;
    private DateTime _nextAllowedRequestTime = DateTime.MinValue;
    private bool _disposed;

    // Cached to avoid allocating a new instance per call (CA1869)
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initializes a new instance with an access token.
    /// </summary>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentHttpClient(
        string accessToken,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production,
        FreeAgentHttpClientOptions? options = null)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        _options = options ?? new FreeAgentHttpClientOptions();
        _httpClient = new HttpClient { BaseAddress = new Uri(FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)) };
        _ownsHttpClient = true;
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FreeAgent.Client/1.0");
    }

    /// <summary>
    /// Initializes a new instance with OAuth client and token for automatic token refresh.
    /// </summary>
    /// <param name="oauthClient">OAuth client for token refresh</param>
    /// <param name="token">Initial OAuth token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentHttpClient(
        FreeAgentOAuthClient oauthClient,
        OAuthTokenResponse token,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production,
        FreeAgentHttpClientOptions? options = null)
    {
        _oauthClient = oauthClient ?? throw new ArgumentNullException(nameof(oauthClient));
        _currentToken = token ?? throw new ArgumentNullException(nameof(token));
        _options = options ?? new FreeAgentHttpClientOptions();

        _httpClient = new HttpClient { BaseAddress = new Uri(FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)) };
        _ownsHttpClient = true;
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token.AccessToken}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FreeAgent.Client/1.0");
    }

    /// <summary>
    /// Initializes a new instance with a custom HttpClient.
    /// </summary>
    /// <param name="httpClient">Custom HttpClient instance</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentHttpClient(HttpClient httpClient, string accessToken, FreeAgentHttpClientOptions? options = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = false;
        _options = options ?? new FreeAgentHttpClientOptions();

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(FreeAgentEnvironmentEndpoints.GetApiBaseUrl(FreeAgentEnvironment.Production));
        }

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FreeAgent.Client/1.0");
    }

    /// <summary>
    /// Sends a GET request to the API.
    /// </summary>
    /// <typeparam name="T">Response type</typeparam>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response</returns>
    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(
            HttpMethod.Get,
            endpoint,
            send: ct => _httpClient.GetAsync(endpoint, ct),
            deserialize: (response, ct) => HandleResponseAsync<T>(response, ct),
            cancellationToken);
    }

    /// <summary>
    /// Sends a GET request and returns deserialized payload with selected response headers.
    /// </summary>
    /// <typeparam name="T">Response type</typeparam>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response and response headers</returns>
    public async Task<FreeAgentHttpResponse<T>> GetWithMetadataAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(
            HttpMethod.Get,
            endpoint,
            send: ct => _httpClient.GetAsync(endpoint, ct),
            deserialize: async (response, ct) =>
            {
                var data = await HandleResponseAsync<T>(response, ct);
                var headers = response.Headers.ToDictionary(
                    static h => h.Key,
                    static h => (IReadOnlyList<string>)h.Value.ToList(),
                    StringComparer.OrdinalIgnoreCase);

                return new FreeAgentHttpResponse<T>(data, headers);
            },
            cancellationToken);
    }

    /// <summary>
    /// Sends a POST request to the API.
    /// </summary>
    /// <typeparam name="T">Response type</typeparam>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="content">Request content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response</returns>
    public async Task<T> PostAsync<T>(string endpoint, HttpContent content, CancellationToken cancellationToken = default)
    {
        var bufferedContent = await BufferedHttpContent.CreateAsync(content, cancellationToken);

        return await ExecuteWithRetryAsync(
            HttpMethod.Post,
            endpoint,
            send: ct => _httpClient.PostAsync(endpoint, bufferedContent.CreateContent(), ct),
            deserialize: (response, ct) => HandleResponseAsync<T>(response, ct),
            cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request to the API.
    /// </summary>
    /// <typeparam name="T">Response type</typeparam>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="content">Request content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response</returns>
    public async Task<T> PutAsync<T>(string endpoint, HttpContent content, CancellationToken cancellationToken = default)
    {
        var bufferedContent = await BufferedHttpContent.CreateAsync(content, cancellationToken);

        return await ExecuteWithRetryAsync(
            HttpMethod.Put,
            endpoint,
            send: ct => _httpClient.PutAsync(endpoint, bufferedContent.CreateContent(), ct),
            deserialize: (response, ct) => HandleResponseAsync<T>(response, ct),
            cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the API.
    /// </summary>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await ExecuteWithRetryAsync(
            HttpMethod.Delete,
            endpoint,
            send: ct => _httpClient.DeleteAsync(endpoint, ct),
            deserialize: static async (response, ct) =>
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(ct);
                    throw new FreeAgentApiException($"API request failed: {response.StatusCode} - {errorContent}");
                }

                return true;
            },
            cancellationToken);
    }

    private async Task<TResult> ExecuteWithRetryAsync<TResult>(
        HttpMethod method,
        string endpoint,
        Func<CancellationToken, Task<HttpResponseMessage>> send,
        Func<HttpResponseMessage, CancellationToken, Task<TResult>> deserialize,
        CancellationToken cancellationToken)
    {
        await EnsureValidTokenAsync(cancellationToken);

        var attempts = 0;

        while (true)
        {
            attempts++;
            HttpResponseMessage? response = null;

            try
            {
                await ApplyRateLimitAsync(cancellationToken);

                response = await send(cancellationToken);
                await HandleRateLimitHeadersAsync(response);

                if (response.IsSuccessStatusCode)
                {
                    return await deserialize(response, cancellationToken);
                }

                if (CanRetry(method, response.StatusCode, attempts))
                {
                    var retryDelay = GetRetryDelay(attempts, response);
                    response.Dispose();
                    await Task.Delay(retryDelay, cancellationToken);
                    continue;
                }

                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = GetRetryAfterDelay(response) ?? TimeSpan.FromSeconds(60);
                    throw new FreeAgentRateLimitException(
                        $"Rate limit exceeded after {attempts} attempts. Retry after {retryAfter}. Response: {errorContent}",
                        endpoint,
                        attempts,
                        response.StatusCode,
                        retryAfter);
                }

                throw new FreeAgentApiException(
                    $"API request failed after {attempts} attempts: {response.StatusCode} - {errorContent}",
                    endpoint,
                    attempts,
                    response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                if (CanRetry(method, null, attempts))
                {
                    var retryDelay = GetRetryDelay(attempts);
                    await Task.Delay(retryDelay, cancellationToken);
                    continue;
                }

                throw new FreeAgentNetworkException(
                    $"Network failure while calling '{endpoint}' after {attempts} attempts.",
                    endpoint,
                    attempts,
                    null,
                    ex);
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                if (CanRetry(method, null, attempts))
                {
                    var retryDelay = GetRetryDelay(attempts);
                    await Task.Delay(retryDelay, cancellationToken);
                    continue;
                }

                throw new FreeAgentTimeoutException(
                    $"Request timeout while calling '{endpoint}' after {attempts} attempts.",
                    endpoint,
                    attempts,
                    null,
                    ex);
            }
            finally
            {
                response?.Dispose();
            }
        }
    }

    private async Task EnsureValidTokenAsync(CancellationToken cancellationToken)
    {
        if (_oauthClient != null && _currentToken != null && _currentToken.IsExpiringSoon && !string.IsNullOrEmpty(_currentToken.RefreshToken))
        {
            await _tokenRefreshSemaphore.WaitAsync(cancellationToken);
            try
            {
                // Double-check after acquiring lock
                if (_currentToken.IsExpiringSoon)
                {
                    var oldRefreshToken = _currentToken.RefreshToken;
                    var newToken = await _oauthClient.RefreshTokenAsync(_currentToken.RefreshToken, cancellationToken);

                    // Preserve the refresh token if the provider doesn't return a new one
                    if (string.IsNullOrEmpty(newToken.RefreshToken))
                    {
                        newToken.RefreshToken = oldRefreshToken;
                    }

                    _currentToken = newToken;

                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {newToken.AccessToken}");
                }
            }
            finally
            {
                _tokenRefreshSemaphore.Release();
            }
        }
    }

    private async Task ApplyRateLimitAsync(CancellationToken cancellationToken)
    {
        await _rateLimitSemaphore.WaitAsync(cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            if (now < _nextAllowedRequestTime)
            {
                var delay = _nextAllowedRequestTime - now;
                await Task.Delay(delay, cancellationToken);
            }

            // Reserve the next slot before releasing the semaphore
            // Take the max of current time and existing next allowed time to handle concurrent requests
            var currentNextTime = _nextAllowedRequestTime > now ? _nextAllowedRequestTime : now;
            _nextAllowedRequestTime = currentNextTime.Add(_options.MinimumRequestSpacing);
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    private async Task HandleRateLimitHeadersAsync(HttpResponseMessage response)
    {
        await _rateLimitSemaphore.WaitAsync(CancellationToken.None);
        try
        {
            // Check for rate limit headers and adjust timing
            if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues))
            {
                if (int.TryParse(remainingValues.FirstOrDefault(), out var remaining) && remaining == 0)
                {
                    if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues))
                    {
                        if (long.TryParse(resetValues.FirstOrDefault(), out var resetTimestamp))
                        {
                            var resetTime = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).UtcDateTime;
                            // Only update if this reset time is later than what we already have
                            if (resetTime > _nextAllowedRequestTime)
                            {
                                _nextAllowedRequestTime = resetTime;
                            }
                        }
                    }
                }
            }

            // Handle 429 Too Many Requests
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var retryAfter = GetRetryAfterDelay(response) ?? TimeSpan.FromSeconds(60);
                var retryTime = DateTime.UtcNow.Add(retryAfter);

                // Only update if this retry time is later than what we already have
                if (retryTime > _nextAllowedRequestTime)
                {
                    _nextAllowedRequestTime = retryTime;
                }
            }

            // Note: We don't apply the default delay here anymore since it's now handled in ApplyRateLimitAsync
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    private static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new FreeAgentApiException($"API request failed: {response.StatusCode} - {content}");
        }

        var result = JsonSerializer.Deserialize<T>(content, JsonOptions);

        if (result == null)
        {
            throw new FreeAgentApiException("Failed to deserialize API response");
        }

        return result;
    }

    private bool CanRetry(HttpMethod method, HttpStatusCode? statusCode, int attempts)
    {
        if (!_options.IsMethodRetriable(method))
        {
            return false;
        }

        if (attempts > _options.MaxNetworkRetries)
        {
            return false;
        }

        return statusCode switch
        {
            HttpStatusCode.TooManyRequests => true,
            HttpStatusCode status when (int)status >= 500 => true,
            null => true,
            _ => false
        };
    }

    private TimeSpan GetRetryDelay(int attempts, HttpResponseMessage? response = null)
    {
        if (response is not null)
        {
            var retryAfter = GetRetryAfterDelay(response);
            if (retryAfter is not null)
            {
                return retryAfter.Value;
            }
        }

        var exponent = Math.Max(attempts - 1, 0);
        var delayMs = _options.BaseRetryDelay.TotalMilliseconds * Math.Pow(2, exponent);
        var cappedDelayMs = Math.Min(delayMs, _options.MaxRetryDelay.TotalMilliseconds);

        if (_options.UseRetryJitter && _options.RetryJitterFactor > 0)
        {
            var jitterOffset = cappedDelayMs * _options.RetryJitterFactor;
            var jitter = (Random.Shared.NextDouble() * 2d - 1d) * jitterOffset;
            cappedDelayMs = Math.Max(0, cappedDelayMs + jitter);
        }

        return TimeSpan.FromMilliseconds(cappedDelayMs);
    }

    private static TimeSpan? GetRetryAfterDelay(HttpResponseMessage response)
    {
        var retryAfterHeader = response.Headers.RetryAfter;
        if (retryAfterHeader is null)
        {
            return null;
        }

        if (retryAfterHeader.Delta is not null)
        {
            return retryAfterHeader.Delta;
        }

        if (retryAfterHeader.Date is not null)
        {
            var delay = retryAfterHeader.Date.Value - DateTimeOffset.UtcNow;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }

        return null;
    }

    private sealed class BufferedHttpContent
    {
        private readonly byte[] _bytes;
        private readonly List<KeyValuePair<string, IEnumerable<string>>> _headers;

        private BufferedHttpContent(byte[] bytes, List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            _bytes = bytes;
            _headers = headers;
        }

        public static async Task<BufferedHttpContent> CreateAsync(HttpContent content, CancellationToken cancellationToken)
        {
            var bytes = await content.ReadAsByteArrayAsync(cancellationToken);
            var headers = content.Headers.Select(static h => new KeyValuePair<string, IEnumerable<string>>(h.Key, h.Value)).ToList();

            return new BufferedHttpContent(bytes, headers);
        }

        public ByteArrayContent CreateContent()
        {
            var clone = new ByteArrayContent(_bytes);
            foreach (var header in _headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
    }

    /// <summary>
    /// Disposes the HTTP client and semaphores.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_ownsHttpClient)
                {
                    _httpClient?.Dispose();
                }
                _rateLimitSemaphore?.Dispose();
                _tokenRefreshSemaphore?.Dispose();
            }
            _disposed = true;
        }
    }
}
