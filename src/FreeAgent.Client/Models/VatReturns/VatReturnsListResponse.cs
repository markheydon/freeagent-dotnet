using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.VatReturns;

/// <summary>
/// Response envelope for listing VAT returns.
/// </summary>
internal sealed class VatReturnsListResponse
{
    /// <summary>
    /// VAT returns for the company.
    /// </summary>
    [JsonPropertyName("vat_returns")]
    public List<VatReturn>? VatReturns { get; set; }
}
