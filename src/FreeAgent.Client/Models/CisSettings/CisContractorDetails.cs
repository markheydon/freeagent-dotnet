using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CisSettings;

/// <summary>
/// Company CIS contractor registration details.
/// </summary>
public sealed class CisContractorDetails
{
    /// <summary>
    /// Date the company started reporting as a contractor.
    /// </summary>
    [JsonPropertyName("reporting_starts_on")]
    public DateOnly? ReportingStartsOn { get; set; }

    /// <summary>
    /// How often the company pays HMRC.
    /// </summary>
    [JsonPropertyName("paye_ni_period")]
    public CisSettingsPayeNiPeriod? PayeNiPeriod { get; set; }
}
