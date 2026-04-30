using FreeAgent.Client;
using FreeAgent.Client.Authentication;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Wraps <see cref="FreeAgentOAuthClient"/> and provides authorization URL generation,
/// authorization code exchange, and <see cref="FreeAgentClient"/> creation with automatic token refresh.
/// </summary>
public sealed class OAuthService : IDisposable
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private FreeAgentEnvironment _selectedEnvironment = FreeAgentEnvironment.Production;
    private FreeAgentOAuthClient? _oauthClient;
    private readonly Lock _clientLock = new();
    private bool _disposed;

    /// <summary>
    /// Initializes the service from application configuration.
    /// Expects <c>FreeAgent:ClientId</c>, <c>FreeAgent:ClientSecret</c>, and <c>FreeAgent:RedirectUri</c> keys.
    /// </summary>
    public OAuthService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var section = configuration.GetSection("FreeAgent");
        _clientId = section["ClientId"] ?? string.Empty;
        _clientSecret = section["ClientSecret"] ?? string.Empty;
        _redirectUri = section["RedirectUri"] ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the selected FreeAgent API environment for the next OAuth connection attempt.
    /// Changing this value resets the cached OAuth client so endpoint URLs stay in sync.
    /// </summary>
    public FreeAgentEnvironment SelectedEnvironment
    {
        get
        {
            lock (_clientLock)
            {
                return _selectedEnvironment;
            }
        }

        set
        {
            lock (_clientLock)
            {
                if (_selectedEnvironment == value)
                {
                    return;
                }

                _selectedEnvironment = value;
                _oauthClient?.Dispose();
                _oauthClient = null;
            }
        }
    }

    /// <summary>
    /// Returns <c>true</c> when all required OAuth credentials are present in configuration.
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_clientId) &&
        !string.IsNullOrWhiteSpace(_clientSecret) &&
        !string.IsNullOrWhiteSpace(_redirectUri);

    /// <summary>
    /// Builds the FreeAgent OAuth authorization URL including the CSRF state parameter.
    /// </summary>
    public string BuildAuthorizationUrl(string state)
    {
        EnsureConfigured();
        return GetOrCreateOAuthClient().GetAuthorizationUrl(state);
    }

    /// <summary>
    /// Exchanges an authorization code for an <see cref="OAuthTokenResponse"/>.
    /// </summary>
    public async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        return await GetOrCreateOAuthClient()
            .ExchangeCodeForTokenAsync(code, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a <see cref="FreeAgentClient"/> configured for automatic token refresh.
    /// The caller is responsible for disposing the returned client.
    /// </summary>
    public FreeAgentClient CreateFreeAgentClient(OAuthTokenResponse token)
    {
        ArgumentNullException.ThrowIfNull(token);
        EnsureConfigured();

        var oauthClient = GetOrCreateOAuthClient();
        return new FreeAgentClient(oauthClient, token, SelectedEnvironment);
    }

    private FreeAgentOAuthClient GetOrCreateOAuthClient()
    {
        lock (_clientLock)
        {
            return _oauthClient ??= new FreeAgentOAuthClient(_clientId, _clientSecret, _redirectUri, _selectedEnvironment);
        }
    }

    private void EnsureConfigured()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                "FreeAgent OAuth credentials are not configured. " +
                "Set FreeAgent:ClientId, FreeAgent:ClientSecret, and FreeAgent:RedirectUri " +
                "using dotnet user-secrets. See samples/README.md for instructions.");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            lock (_clientLock)
            {
                _oauthClient?.Dispose();
                _oauthClient = null;
            }
            _disposed = true;
        }
    }
}
