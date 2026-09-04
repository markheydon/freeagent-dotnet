using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// Wrapper for users list API responses.
/// </summary>
public class UsersResponse
{
    /// <summary>
    /// User list payload.
    /// </summary>
    [JsonPropertyName("users")]
    public List<User>? Users { get; set; }
}
