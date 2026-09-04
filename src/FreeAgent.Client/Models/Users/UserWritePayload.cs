using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// Writable user attributes for create and update requests.
/// </summary>
internal sealed class UserWritePayload
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("ni_number")]
    public string? NiNumber { get; set; }

    [JsonPropertyName("unique_tax_reference")]
    public string? UniqueTaxReference { get; set; }

    [JsonPropertyName("role")]
    public UserRole? Role { get; set; }

    [JsonPropertyName("opening_mileage")]
    public decimal? OpeningMileage { get; set; }

    [JsonPropertyName("send_invitation")]
    public bool? SendInvitation { get; set; }

    [JsonPropertyName("permission_level")]
    public UserPermissionLevel? PermissionLevel { get; set; }

    public static UserWritePayload FromUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserWritePayload
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            NiNumber = user.NiNumber,
            UniqueTaxReference = user.UniqueTaxReference,
            Role = user.Role,
            OpeningMileage = user.OpeningMileage,
            SendInvitation = user.SendInvitation,
            PermissionLevel = user.PermissionLevel
        };
    }
}
