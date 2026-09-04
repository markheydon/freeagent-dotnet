using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Categories;
using FreeAgent.Client.Services.Company;
using FreeAgent.Client.Services.Contacts;
using FreeAgent.Client.Services.EmailAddresses;

namespace FreeAgent.Client;

/// <summary>
/// Main client for interacting with the FreeAgent API.
/// </summary>
public sealed class FreeAgentClient : IDisposable
{
    private readonly FreeAgentHttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// Company API service.
    /// </summary>
    public CompanyService Company { get; }

    /// <summary>
    /// Contacts API service.
    /// </summary>
    public ContactService Contacts { get; }

    /// <summary>
    /// Categories API service.
    /// </summary>
    public CategoryService Categories { get; }

    /// <summary>
    /// Email addresses API service.
    /// </summary>
    public EmailAddressesService EmailAddresses { get; }

    /// <summary>
    /// Initializes a new instance with an access token.
    /// </summary>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentClient(
        string accessToken,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production,
        FreeAgentHttpClientOptions? options = null)
        : this(new FreeAgentHttpClient(accessToken, environment, options))
    {
    }

    /// <summary>
    /// Initializes a new instance with OAuth client and token for automatic refresh.
    /// </summary>
    /// <param name="oauthClient">OAuth client</param>
    /// <param name="token">OAuth token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentClient(
        FreeAgentOAuthClient oauthClient,
        OAuthTokenResponse token,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production,
        FreeAgentHttpClientOptions? options = null)
        : this(new FreeAgentHttpClient(oauthClient, token, environment, options))
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom HTTP client.
    /// </summary>
    /// <param name="httpClient">Custom HTTP client</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentClient(HttpClient httpClient, string accessToken, FreeAgentHttpClientOptions? options = null)
        : this(new FreeAgentHttpClient(httpClient, accessToken, options))
    {
    }

    private FreeAgentClient(FreeAgentHttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        Company = new CompanyService(_httpClient);
        Contacts = new ContactService(_httpClient);
        Categories = new CategoryService(_httpClient);
        EmailAddresses = new EmailAddressesService(_httpClient);
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
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }

            _disposed = true;
        }
    }
}
