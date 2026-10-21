using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CorporationTaxReturns;

/// <summary>
/// Response envelope for a single corporation tax return.
/// </summary>
internal sealed class CorporationTaxReturnResponse
{
    /// <summary>
    /// Corporation tax return payload.
    /// </summary>
    [JsonPropertyName("corporation_tax_return")]
    public CorporationTaxReturn? CorporationTaxReturn { get; set; }
}
