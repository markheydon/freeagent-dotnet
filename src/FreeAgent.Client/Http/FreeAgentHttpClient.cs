using System.Net;
using System.Text.Json;
using FreeAgent.Client.Authentication;

namespace FreeAgent.Client.Http;

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
    private DateTime _nextAllowedRequestTime = DateTime.MinValue;
    private bool _disposed;

    private const string BaseUrl = "https://api.freeagent.com/v2";
    private const int DefaultRateLimitDelayMs = 1000; // 1 request per second as a safe default

    /// <summary>
    /// Initializes a new instance with an access token.
    /// </summary>
    /// <param name="accessToken">OAuth access token</param>
    public FreeAgentHttpClient(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _ownsHttpClient = true;
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FreeAgent.Client/1.0");
    }

    /// <summary>
    /// Initializes a new instance with OAuth client and token for automatic token refresh.
    /// </summary>
    /// <param name="oauthClient">OAuth client for token refresh</param>
    /// <param name="token">Initial OAuth token</param>
    public FreeAgentHttpClient(FreeAgentOAuthClient oauthClient, OAuthTokenResponse token)
    {
        _oauthClient = oauthClient ?? throw new ArgumentNullException(nameof(oauthClient));
        _currentToken = token ?? throw new ArgumentNullException(nameof(token));

        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _ownsHttpClient = true;
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token.AccessToken}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FreeAgent.Client/1.0");
    }

    /// <summary>
    /// Initializes a new instance with a custom HttpClient.
    /// </summary>
    /// <param name="httpClient">Custom HttpClient instance</param>
    /// <param name="accessToken">OAuth access token</param>
    public FreeAgentHttpClient(HttpClient httpClient, string accessToken)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = false;
        
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(BaseUrl);
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
        await EnsureValidTokenAsync(cancellationToken);
        await ApplyRateLimitAsync(cancellationToken);

        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        await HandleRateLimitHeadersAsync(response);

        return await HandleResponseAsync<T>(response, cancellationToken);
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
        await EnsureValidTokenAsync(cancellationToken);
        await ApplyRateLimitAsync(cancellationToken);

        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        await HandleRateLimitHeadersAsync(response);

        return await HandleResponseAsync<T>(response, cancellationToken);
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
        await EnsureValidTokenAsync(cancellationToken);
        await ApplyRateLimitAsync(cancellationToken);

        var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
        await HandleRateLimitHeadersAsync(response);

        return await HandleResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the API.
    /// </summary>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await EnsureValidTokenAsync(cancellationToken);
        await ApplyRateLimitAsync(cancellationToken);

        var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
        await HandleRateLimitHeadersAsync(response);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new FreeAgentApiException($"API request failed: {response.StatusCode} - {errorContent}");
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
                    var newToken = await _oauthClient.RefreshTokenAsync(_currentToken.RefreshToken, cancellationToken);
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
                            _nextAllowedRequestTime = resetTime;
                        }
                    }
                }
            }

            // Handle 429 Too Many Requests
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (response.Headers.RetryAfter != null)
                {
                    var retryAfter = response.Headers.RetryAfter.Delta ?? TimeSpan.FromSeconds(60);
                    _nextAllowedRequestTime = DateTime.UtcNow.Add(retryAfter);
                }
                else
                {
                    _nextAllowedRequestTime = DateTime.UtcNow.AddSeconds(60);
                }

                var errorContent = await response.Content.ReadAsStringAsync(CancellationToken.None);
                throw new FreeAgentRateLimitException($"Rate limit exceeded. Retry after {_nextAllowedRequestTime}");
            }

            // Apply a safe default delay between requests
            _nextAllowedRequestTime = DateTime.UtcNow.AddMilliseconds(DefaultRateLimitDelayMs);
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new FreeAgentApiException($"API request failed: {response.StatusCode} - {content}");
        }

        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
        {
            throw new FreeAgentApiException("Failed to deserialize API response");
        }

        return result;
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
