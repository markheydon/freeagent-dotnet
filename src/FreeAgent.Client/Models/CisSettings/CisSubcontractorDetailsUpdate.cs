using System.Text.Json.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Subcontractor section for a partial CIS settings update.
/// </summary>
public sealed class CisSubcontractorDetailsUpdate
{
    /// <summary>
    /// Deduction rates to apply when specified.
    /// </summary>
    [JsonPropertyName("cis_deduction_rates")]
    public List<CisDeductionRate>? CisDeductionRates { get; set; }

    /// <summary>
    /// Prior deductions patch (omit to leave unchanged, null to clear).
    /// </summary>
    [JsonIgnore]
    public PatchValue<CisPriorDeductions> PriorDeductions { get; set; }
}
