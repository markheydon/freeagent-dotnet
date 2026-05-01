using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Authentication;

/// <summary>
/// OAuth 2.0 token response.
/// </summary>
public class OAuthTokenResponse
{
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
    /// Time when the token was issued (calculated).
    /// </summary>
    [JsonIgnore]
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Checks if the token has expired.
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => DateTime.UtcNow >= IssuedAt.AddSeconds(ExpiresIn);

    /// <summary>
    /// Checks if the token will expire soon (within 5 minutes).
    /// </summary>
    [JsonIgnore]
    public bool IsExpiringSoon => DateTime.UtcNow >= IssuedAt.AddSeconds(ExpiresIn - 300);
}
