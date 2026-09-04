using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// Request envelope for creating or updating a user.
/// </summary>
internal class UserRequest
{
    /// <summary>
    /// User payload.
    /// </summary>
    [JsonPropertyName("user")]
    public UserWritePayload User { get; set; } = new();
}
