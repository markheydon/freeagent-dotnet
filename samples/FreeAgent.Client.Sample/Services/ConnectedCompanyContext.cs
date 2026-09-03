namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Loads the connected FreeAgent company record so the sample app can link to the web UI.
/// </summary>
public sealed class ConnectedCompanyContext
{
    private readonly TokenStore _tokenStore;
    private readonly OAuthService _oauthService;
    private readonly Lock _lock = new();
    private string? _companyName;
    private string? _subdomain;
    private bool _isLoaded;

    public ConnectedCompanyContext(TokenStore tokenStore, OAuthService oauthService)
    {
        _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        _oauthService = oauthService ?? throw new ArgumentNullException(nameof(oauthService));
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

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (!_tokenStore.IsConnected)
        {
            Clear();
            return;
        }

        var token = _tokenStore.GetToken();
        if (token is null)
        {
            Clear();
            return;
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
        }
        catch
        {
            Clear();
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
}
