using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.VatReturns;

/// <summary>
/// Response envelope for a single VAT return.
/// </summary>
internal sealed class VatReturnResponse
{
    /// <summary>
    /// VAT return payload.
    /// </summary>
    [JsonPropertyName("vat_return")]
    public VatReturn? VatReturn { get; set; }
}
