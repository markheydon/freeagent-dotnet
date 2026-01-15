using FreeAgent.Client.Authentication;
using FreeAgent.Client.Http;
using FreeAgent.Client.Services;

namespace FreeAgent.Client;

/// <summary>
/// Main client for interacting with the FreeAgent API.
/// </summary>
public class FreeAgentClient
{
    private readonly FreeAgentHttpClient _httpClient;

    /// <summary>
    /// Company API service.
    /// </summary>
    public CompanyService Company { get; }

    /// <summary>
    /// Initializes a new instance with an access token.
    /// </summary>
    /// <param name="accessToken">OAuth access token</param>
    public FreeAgentClient(string accessToken)
    {
        _httpClient = new FreeAgentHttpClient(accessToken);
        Company = new CompanyService(_httpClient);
    }

    /// <summary>
    /// Initializes a new instance with OAuth client and token for automatic refresh.
    /// </summary>
    /// <param name="oauthClient">OAuth client</param>
    /// <param name="token">OAuth token</param>
    public FreeAgentClient(FreeAgentOAuthClient oauthClient, OAuthTokenResponse token)
    {
        _httpClient = new FreeAgentHttpClient(oauthClient, token);
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
}
