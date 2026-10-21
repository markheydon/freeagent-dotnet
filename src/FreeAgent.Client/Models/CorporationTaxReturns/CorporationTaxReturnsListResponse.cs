using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CorporationTaxReturns;

/// <summary>
/// Response envelope for listing corporation tax returns.
/// </summary>
internal sealed class CorporationTaxReturnsListResponse
{
    /// <summary>
    /// Corporation tax returns for the company.
    /// </summary>
    [JsonPropertyName("corporation_tax_returns")]
    public List<CorporationTaxReturn>? CorporationTaxReturns { get; set; }
}
