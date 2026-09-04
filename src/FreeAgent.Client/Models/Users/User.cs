using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// Represents a FreeAgent user.
/// </summary>
public class User
{
    /// <summary>
    /// User resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Login email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// First name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// UK National Insurance number.
    /// </summary>
    [JsonPropertyName("ni_number")]
    public string? NiNumber { get; set; }

    /// <summary>
    /// 10-digit UK Tax Reference.
    /// </summary>
    [JsonPropertyName("unique_tax_reference")]
    public string? UniqueTaxReference { get; set; }

    /// <summary>
    /// User role.
    /// </summary>
    [JsonPropertyName("role")]
    public UserRole? Role { get; set; }

    /// <summary>
    /// Opening mileage as of company start date.
    /// </summary>
    [JsonPropertyName("opening_mileage")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OpeningMileage { get; set; }

    /// <summary>
    /// When <see langword="true"/>, sends the user an invitation to set their password.
    /// </summary>
    [JsonPropertyName("send_invitation")]
    public bool? SendInvitation { get; set; }

    /// <summary>
    /// FreeAgent permission level.
    /// </summary>
    [JsonPropertyName("permission_level")]
    public UserPermissionLevel? PermissionLevel { get; set; }

    /// <summary>
    /// Payroll information for the current tax year when configured.
    /// </summary>
    [JsonPropertyName("current_payroll_profile")]
    public CurrentPayrollProfile? CurrentPayrollProfile { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Human-friendly display name derived from first and last name.
    /// </summary>
    [JsonIgnore]
    public string DisplayName
    {
        get
        {
            var fullName = string.Join(" ", new[] { FirstName, LastName }.Where(static x => !string.IsNullOrWhiteSpace(x)));
            return string.IsNullOrWhiteSpace(fullName) ? Url : fullName;
        }
    }
}
