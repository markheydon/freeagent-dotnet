using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Models.CisSettings;

/// <summary>
/// Company CIS subcontractor registration details.
/// </summary>
public sealed class CisSubcontractorDetails
{
    /// <summary>
    /// Deduction rates that apply to the company.
    /// </summary>
    [JsonPropertyName("cis_deduction_rates")]
    public List<CisDeductionRate>? CisDeductionRates { get; set; }

    /// <summary>
    /// Prior deductions recorded before FreeAgent, or null when none recorded.
    /// </summary>
    [JsonPropertyName("prior_deductions")]
    public CisPriorDeductions? PriorDeductions { get; set; }
}
