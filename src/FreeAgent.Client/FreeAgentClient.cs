using FreeAgent.Client.Authentication;
using FreeAgent.Client.Http;
using FreeAgent.Client.Services;

namespace FreeAgent.Client;

/// <summary>
/// Main client for interacting with the FreeAgent API.
/// </summary>
public class FreeAgentClient : IDisposable
{
    private readonly FreeAgentHttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// Company API service.
    /// </summary>
    public CompanyService Company { get; }

    /// <summary>
    /// Initializes a new instance with an access token.
    /// </summary>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    public FreeAgentClient(string accessToken, FreeAgentEnvironment environment = FreeAgentEnvironment.Production)
    {
        _httpClient = new FreeAgentHttpClient(accessToken, environment);
        Company = new CompanyService(_httpClient);
    }

    /// <summary>
    /// Initializes a new instance with OAuth client and token for automatic refresh.
    /// </summary>
    /// <param name="oauthClient">OAuth client</param>
    /// <param name="token">OAuth token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    public FreeAgentClient(FreeAgentOAuthClient oauthClient, OAuthTokenResponse token, FreeAgentEnvironment environment = FreeAgentEnvironment.Production)
    {
        _httpClient = new FreeAgentHttpClient(oauthClient, token, environment);
        Company = new CompanyService(_httpClient);
    }

    /// <summary>
    /// Initializes a new instance with a custom HTTP client.
    /// </summary>
    /// <param name="httpClient">Custom HTTP client</param>
    /// <param name="accessToken">OAuth access token</param>
    public FreeAgentClient(HttpClient httpClient, string accessToken)
    {
        _httpClient = new FreeAgentHttpClient(httpClient, accessToken);
        Company = new CompanyService(_httpClient);
    }

    /// <summary>
    /// Disposes the client and its resources.
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
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }
}
