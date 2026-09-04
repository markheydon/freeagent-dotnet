using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// Wrapper for single-user API responses.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// User payload.
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }
}
