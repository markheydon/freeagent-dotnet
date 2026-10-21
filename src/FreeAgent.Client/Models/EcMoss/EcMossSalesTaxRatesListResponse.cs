using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response wrapper for EC VAT MOSS sales tax rates lookup.
/// </summary>
internal sealed class EcMossSalesTaxRatesListResponse
{
    /// <summary>
    /// MOSS sales tax rate collection.
    /// </summary>
    [JsonPropertyName("sales_tax_rates")]
    public List<EcMossSalesTaxRate>? SalesTaxRates { get; set; }
}
