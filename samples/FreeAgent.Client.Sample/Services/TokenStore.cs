using FreeAgent.Client;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Thread-safe store for the active FreeAgent OAuth token.
/// Restores and persists the session in a short-lived browser cookie for local development.
/// </summary>
public sealed class TokenStore
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Lock _lock = new();
    private OAuthTokenResponse? _token;
    private FreeAgentEnvironment _connectedEnvironment = FreeAgentEnvironment.Production;
    private string? _pendingState;
    private FreeAgentEnvironment _pendingEnvironment = FreeAgentEnvironment.Production;

    /// <summary>
    /// Initialises the token store.
    /// </summary>
    public TokenStore(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Returns the stored token, or <c>null</c> if not connected.
    /// </summary>
    public OAuthTokenResponse? GetToken()
    {
        TryRestoreFromCurrentRequest();

        lock (_lock)
        {
            return _token;
        }
    }

    /// <summary>
    /// Stores the token received from a successful OAuth exchange.
    /// </summary>
    public void SetToken(OAuthTokenResponse token, FreeAgentEnvironment environment)
    {
        SetToken(token, environment, response: null);
    }

    /// <summary>
    /// Stores the token received from a successful OAuth exchange and persists the session cookie when the HTTP response allows it.
    /// </summary>
    public void SetToken(OAuthTokenResponse token, FreeAgentEnvironment environment, HttpResponse? response)
    {
        ArgumentNullException.ThrowIfNull(token);

        lock (_lock)
        {
            _token = token;
            _connectedEnvironment = environment;
        }

        PersistCurrentSession(response);
    }

    /// <summary>
    /// Clears the stored token (disconnect).
    /// </summary>
    public void ClearToken()
    {
        ClearToken(response: null);
    }

    /// <summary>
    /// Clears the stored token (disconnect) and removes the session cookie when the HTTP response allows it.
    /// </summary>
    public void ClearToken(HttpResponse? response)
    {
        lock (_lock)
        {
            _token = null;
            _connectedEnvironment = FreeAgentEnvironment.Production;
        }

        ClearPersistedSession(response);
    }

    /// <summary>
    /// Returns the environment used for the active connection.
    /// </summary>
    public FreeAgentEnvironment ConnectedEnvironment
    {
        get
        {
            TryRestoreFromCurrentRequest();

            lock (_lock)
            {
                return _connectedEnvironment;
            }
        }
    }

    /// <summary>
    /// Returns <c>true</c> when a token is stored and the user is considered connected.
    /// </summary>
    public bool IsConnected
    {
        get
        {
            TryRestoreFromCurrentRequest();

            lock (_lock)
            {
                return _token is not null;
            }
        }
    }

    /// <summary>
    /// Restores a previously persisted OAuth session from the current HTTP request, if present.
    /// </summary>
    public void TryRestoreFromCurrentRequest()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        lock (_lock)
        {
            if (_token is not null)
            {
                return;
            }
        }

        if (!OAuthSessionPersistence.TryLoad(httpContext.Request, out var token, out var environment)
            || token is null)
        {
            return;
        }

        lock (_lock)
        {
            if (_token is not null)
            {
                return;
            }

            _token = token;
            _connectedEnvironment = environment;
        }
    }

    /// <summary>
    /// Generates a cryptographically random state value, stores it together with the chosen
    /// <paramref name="environment"/> for CSRF validation, and returns it for inclusion in
    /// the OAuth authorization URL.
    /// </summary>
    public string GenerateAndStorePendingState(FreeAgentEnvironment environment)
    {
        return GenerateAndStorePendingState(environment, response: null);
    }

    /// <summary>
    /// Generates and stores pending OAuth state, persisting a CSRF cookie when the HTTP response allows it.
    /// </summary>
    public string GenerateAndStorePendingState(FreeAgentEnvironment environment, HttpResponse? response)
    {
        var state = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        lock (_lock)
        {
            _pendingState = state;
            _pendingEnvironment = environment;
        }

        PersistPendingState(state, environment, response);
        return state;
    }

    /// <summary>
    /// Validates the state returned from the OAuth callback against the stored pending state.
    /// Clears the stored state and environment regardless of outcome to prevent replay.
    /// </summary>
    /// <param name="state">The state value returned from the OAuth callback.</param>
    /// <param name="pendingEnvironment">
    /// When this method returns <c>true</c>, contains the environment that was selected
    /// when the authorization URL was generated. When <c>false</c>, the value is undefined.
    /// </param>
    /// <returns><c>true</c> if the state matches; <c>false</c> otherwise.</returns>
    public bool ValidateAndClearState(string state, out FreeAgentEnvironment pendingEnvironment)
    {
        ArgumentNullException.ThrowIfNull(state);

        string? expectedState;
        FreeAgentEnvironment environment;

        lock (_lock)
        {
            expectedState = _pendingState;
            environment = _pendingEnvironment;
            _pendingState = null;
            _pendingEnvironment = FreeAgentEnvironment.Production;
        }

        if (expectedState is null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null
                && OAuthPendingStatePersistence.TryLoad(httpContext.Request, out var cookieState, out var cookieEnvironment)
                && cookieState is not null)
            {
                expectedState = cookieState;
                environment = cookieEnvironment;
            }
        }

        ClearPendingState(_httpContextAccessor.HttpContext?.Response);

        var valid = expectedState is not null &&
                    string.Equals(expectedState, state, StringComparison.Ordinal);
        pendingEnvironment = valid ? environment : FreeAgentEnvironment.Production;
        return valid;
    }

    private static HttpResponse? ResolveResponse(HttpResponse? response, IHttpContextAccessor httpContextAccessor) =>
        response ?? httpContextAccessor.HttpContext?.Response;

    private void PersistCurrentSession(HttpResponse? response)
    {
        response = ResolveResponse(response, _httpContextAccessor);
        if (response is null || response.HasStarted)
        {
            return;
        }

        OAuthTokenResponse? token;
        FreeAgentEnvironment environment;

        lock (_lock)
        {
            if (_token is null)
            {
                return;
            }

            token = _token;
            environment = _connectedEnvironment;
        }

        OAuthSessionPersistence.Save(response, token, environment);
    }

    private void ClearPersistedSession(HttpResponse? response)
    {
        response = ResolveResponse(response, _httpContextAccessor);
        if (response is null || response.HasStarted)
        {
            return;
        }

        OAuthSessionPersistence.Clear(response);
    }

    private void PersistPendingState(string state, FreeAgentEnvironment environment, HttpResponse? response)
    {
        response = ResolveResponse(response, _httpContextAccessor);
        if (response is null || response.HasStarted)
        {
            return;
        }

        OAuthPendingStatePersistence.Save(response, state, environment);
    }

    private void ClearPendingState(HttpResponse? response)
    {
        response = ResolveResponse(response, _httpContextAccessor);
        if (response is null || response.HasStarted)
        {
            return;
        }

        OAuthPendingStatePersistence.Clear(response);
    }
}
