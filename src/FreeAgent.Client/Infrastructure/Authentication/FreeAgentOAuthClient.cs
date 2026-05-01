using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client.Infrastructure.Authentication;

/// <summary>
/// OAuth 2.0 client for FreeAgent API authentication.
/// </summary>
public class FreeAgentOAuthClient : IDisposable
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private bool _disposed;

    private readonly string _authorizationEndpoint;
    private readonly string _tokenEndpoint;

    // Cached to avoid allocating a new instance per call (CA1869)
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initializes a new instance of the OAuth client.
    /// </summary>
    /// <param name="clientId">OAuth client ID</param>
    /// <param name="clientSecret">OAuth client secret</param>
    /// <param name="redirectUri">OAuth redirect URI</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    public FreeAgentOAuthClient(string clientId, string clientSecret, string redirectUri, FreeAgentEnvironment environment = FreeAgentEnvironment.Production)
    {
        _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        _redirectUri = redirectUri ?? throw new ArgumentNullException(nameof(redirectUri));
        _httpClient = new HttpClient();
        _ownsHttpClient = true;
        _authorizationEndpoint = FreeAgentEnvironmentEndpoints.GetOAuthAuthorizationEndpoint(environment);
        _tokenEndpoint = FreeAgentEnvironmentEndpoints.GetOAuthTokenEndpoint(environment);
    }

    /// <summary>
    /// Initializes a new instance of the OAuth client with a custom HttpClient.
    /// </summary>
    /// <param name="clientId">OAuth client ID</param>
    /// <param name="clientSecret">OAuth client secret</param>
    /// <param name="redirectUri">OAuth redirect URI</param>
    /// <param name="httpClient">Custom HttpClient instance</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    public FreeAgentOAuthClient(string clientId, string clientSecret, string redirectUri, HttpClient httpClient, FreeAgentEnvironment environment = FreeAgentEnvironment.Production)
    {
        _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        _redirectUri = redirectUri ?? throw new ArgumentNullException(nameof(redirectUri));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = false;
        _authorizationEndpoint = FreeAgentEnvironmentEndpoints.GetOAuthAuthorizationEndpoint(environment);
        _tokenEndpoint = FreeAgentEnvironmentEndpoints.GetOAuthTokenEndpoint(environment);
    }

    /// <summary>
    /// Generates the authorization URL for the OAuth flow.
    /// </summary>
    /// <param name="state">Optional state parameter for CSRF protection</param>
    /// <returns>Authorization URL</returns>
    public string GetAuthorizationUrl(string? state = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["response_type"] = "code",
            ["client_id"] = _clientId,
            ["redirect_uri"] = _redirectUri
        };

        if (!string.IsNullOrEmpty(state))
        {
            queryParams["state"] = state;
        }

        var queryString = string.Join("&", queryParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{_authorizationEndpoint}?{queryString}";
    }

    /// <summary>
    /// Exchanges an authorization code for an access token.
    /// </summary>
    /// <param name="code">Authorization code received from callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OAuth token response</returns>
    public async Task<OAuthTokenResponse> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(code))
        {
            throw new ArgumentNullException(nameof(code));
        }

        var requestData = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _redirectUri,
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret
        };

        return await RequestTokenAsync(requestData, cancellationToken);
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OAuth token response</returns>
    public async Task<OAuthTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        var requestData = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret
        };

        return await RequestTokenAsync(requestData, cancellationToken);
    }

    private async Task<OAuthTokenResponse> RequestTokenAsync(Dictionary<string, string> requestData, CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(requestData);
        using var response = await _httpClient.PostAsync(_tokenEndpoint, content, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new FreeAgentOAuthException($"Token request failed: {response.StatusCode} - {responseBody}");
        }

        var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<OAuthTokenResponse>(responseBody, JsonOptions);

        if (tokenResponse == null)
        {
            throw new FreeAgentOAuthException("Failed to deserialize token response");
        }

        return tokenResponse;
    }

    /// <summary>
    /// Disposes the HTTP client.
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
            if (disposing && _ownsHttpClient)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }
}
