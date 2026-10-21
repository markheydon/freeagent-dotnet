using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.PayrollProfiles;

/// <summary>
/// API response envelope for payroll profiles in a tax year.
/// </summary>
public class PayrollProfilesListResponse
{
    /// <summary>
    /// Payroll profiles returned by FreeAgent.
    /// </summary>
    [JsonPropertyName("profiles")]
    public IReadOnlyList<PayrollProfile>? Profiles { get; set; }
}
