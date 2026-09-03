using Microsoft.Extensions.Logging;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Loads the connected FreeAgent company record so the sample app can link to the web UI.
/// </summary>
public sealed partial class ConnectedCompanyContext
{
    private readonly TokenStore _tokenStore;
    private readonly OAuthService _oauthService;
    private readonly ILogger<ConnectedCompanyContext> _logger;
    private readonly Lock _lock = new();
    private string? _companyName;
    private string? _subdomain;
    private bool _isLoaded;

    public ConnectedCompanyContext(
        TokenStore tokenStore,
        OAuthService oauthService,
        ILogger<ConnectedCompanyContext> logger)
    {
        _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        _oauthService = oauthService ?? throw new ArgumentNullException(nameof(oauthService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsLoaded
    {
        get
        {
            lock (_lock)
            {
                return _isLoaded;
            }
        }
    }

    public string? CompanyName
    {
        get
        {
            lock (_lock)
            {
                return _companyName;
            }
        }
    }

    public string? Subdomain
    {
        get
        {
            lock (_lock)
            {
                return _subdomain;
            }
        }
    }

    public string? AccountUrl =>
        FreeAgentWebUrls.GetAccountUrl(_tokenStore.ConnectedEnvironment, Subdomain);

    /// <summary>
    /// Refreshes company metadata from the API.
    /// </summary>
    /// <returns><c>true</c> when company data was loaded; otherwise <c>false</c>.</returns>
    public async Task<bool> RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (!_tokenStore.IsConnected)
        {
            Clear();
            return false;
        }

        var token = _tokenStore.GetToken();
        if (token is null)
        {
            Clear();
            return false;
        }

        try
        {
            using var client = _oauthService.CreateFreeAgentClient(token, _tokenStore.ConnectedEnvironment);
            var company = await client.Company.GetCompanyAsync(cancellationToken);

            lock (_lock)
            {
                _companyName = company.Name;
                _subdomain = company.Subdomain;
                _isLoaded = true;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.RefreshFailed(_logger, ex);
            Clear();
            return false;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _companyName = null;
            _subdomain = null;
            _isLoaded = false;
        }
    }

    private static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to refresh connected company context.")]
        public static partial void RefreshFailed(ILogger logger, Exception ex);
    }
}
