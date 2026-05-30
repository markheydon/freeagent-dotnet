using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// OAuth 2.0 token response.
/// </summary>
public class OAuthTokenResponse
{
    private DateTimeOffset? _expiresAtUtc;

    /// <summary>
    /// Access token for API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token type (usually "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Token expiration time in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Absolute UTC expiry for persistence and round-trip safety.
    /// </summary>
    [JsonPropertyName("expires_at_utc")]
    public DateTimeOffset? ExpiresAtUtc
    {
        get => _expiresAtUtc;
        set => _expiresAtUtc = value?.ToUniversalTime();
    }

    /// <summary>
    /// Checks if the token has expired.
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => DateTimeOffset.UtcNow >= GetEffectiveExpiresAtUtc();

    /// <summary>
    /// Checks if the token will expire soon (within 5 minutes).
    /// </summary>
    [JsonIgnore]
    public bool IsExpiringSoon => DateTimeOffset.UtcNow >= GetEffectiveExpiresAtUtc().AddMinutes(-5);

    /// <summary>
    /// Remaining lifetime before expiry.
    /// </summary>
    [JsonIgnore]
    public TimeSpan TimeUntilExpiry
    {
        get
        {
            var remaining = GetEffectiveExpiresAtUtc() - DateTimeOffset.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Initialises the persisted expiry timestamp from <see cref="ExpiresIn"/>.
    /// </summary>
    /// <param name="issuedAtUtc">Optional UTC issue time. Defaults to current UTC time.</param>
    public void InitialiseExpiryUtc(DateTimeOffset? issuedAtUtc = null)
    {
        var resolvedIssuedAtUtc = issuedAtUtc ?? DateTimeOffset.UtcNow;
        _expiresAtUtc = resolvedIssuedAtUtc.AddSeconds(Math.Max(0, ExpiresIn));
    }

    internal void EnsureExpiryUtc()
    {
        if (_expiresAtUtc is null)
        {
            InitialiseExpiryUtc();
        }
    }

    private DateTimeOffset GetEffectiveExpiresAtUtc()
    {
        EnsureExpiryUtc();
        return _expiresAtUtc!.Value;
    }
}
