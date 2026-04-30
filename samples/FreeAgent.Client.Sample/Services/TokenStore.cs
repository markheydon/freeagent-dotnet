using FreeAgent.Client;
using FreeAgent.Client.Authentication;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Thread-safe in-memory store for the active FreeAgent OAuth token.
/// Token lifetime is scoped to the application process; tokens are lost on restart.
/// </summary>
public sealed class TokenStore
{
    private readonly Lock _lock = new();
    private OAuthTokenResponse? _token;
    private FreeAgentEnvironment _connectedEnvironment = FreeAgentEnvironment.Production;
    private string? _pendingState;
    private FreeAgentEnvironment _pendingEnvironment = FreeAgentEnvironment.Production;

    /// <summary>
    /// Returns the stored token, or <c>null</c> if not connected.
    /// </summary>
    public OAuthTokenResponse? GetToken()
    {
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
        ArgumentNullException.ThrowIfNull(token);
        lock (_lock)
        {
            _token = token;
            _connectedEnvironment = environment;
        }
    }

    /// <summary>
    /// Clears the stored token (disconnect).
    /// </summary>
    public void ClearToken()
    {
        lock (_lock)
        {
            _token = null;
            _connectedEnvironment = FreeAgentEnvironment.Production;
        }
    }

    /// <summary>
    /// Returns the environment used for the active connection.
    /// </summary>
    public FreeAgentEnvironment ConnectedEnvironment
    {
        get
        {
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
            lock (_lock)
            {
                return _token is not null;
            }
        }
    }

    /// <summary>
    /// Generates a cryptographically random state value, stores it together with the chosen
    /// <paramref name="environment"/> for CSRF validation, and returns it for inclusion in
    /// the OAuth authorization URL.
    /// </summary>
    public string GenerateAndStorePendingState(FreeAgentEnvironment environment)
    {
        var state = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        lock (_lock)
        {
            _pendingState = state;
            _pendingEnvironment = environment;
        }
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
        lock (_lock)
        {
            var valid = _pendingState is not null &&
                        string.Equals(_pendingState, state, StringComparison.Ordinal);
            _pendingState = null;
            pendingEnvironment = _pendingEnvironment;
            _pendingEnvironment = FreeAgentEnvironment.Production;
            return valid;
        }
    }
}
