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
    /// Generates a cryptographically random state value, stores it for CSRF validation,
    /// and returns it for inclusion in the OAuth authorization URL.
    /// </summary>
    public string GenerateAndStorePendingState()
    {
        var state = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        lock (_lock)
        {
            _pendingState = state;
        }
        return state;
    }

    /// <summary>
    /// Validates the state returned from the OAuth callback against the stored pending state.
    /// Clears the stored state regardless of outcome to prevent replay.
    /// </summary>
    /// <returns><c>true</c> if the state matches; <c>false</c> otherwise.</returns>
    public bool ValidateAndClearState(string state)
    {
        ArgumentNullException.ThrowIfNull(state);
        lock (_lock)
        {
            var valid = _pendingState is not null &&
                        string.Equals(_pendingState, state, StringComparison.Ordinal);
            _pendingState = null;
            return valid;
        }
    }
}
