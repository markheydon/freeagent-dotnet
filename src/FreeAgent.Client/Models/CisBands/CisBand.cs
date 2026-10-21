using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Models.CisBands;

/// <summary>
/// A Construction Industry Scheme band available to the company.
/// </summary>
public sealed class CisBand
{
    /// <summary>
    /// Band identifier (wire values such as <c>cis_gross</c>, <c>cis_standard</c>, <c>cis_higher</c>).
    /// </summary>
    [JsonPropertyName("name")]
    public CisDeductionRate? Name { get; set; }

    /// <summary>
    /// CIS deduction rate as a decimal fraction (for example <c>0.2</c> for 20%).
    /// </summary>
    [JsonPropertyName("deduction_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DeductionRate { get; set; }

    /// <summary>
    /// Income category description for this band.
    /// </summary>
    [JsonPropertyName("income_description")]
    public string? IncomeDescription { get; set; }

    /// <summary>
    /// Deduction category description for this band.
    /// </summary>
    [JsonPropertyName("deduction_description")]
    public string? DeductionDescription { get; set; }

    /// <summary>
    /// Nominal code for the band's categories (for example <c>061</c>).
    /// </summary>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; set; }
}
