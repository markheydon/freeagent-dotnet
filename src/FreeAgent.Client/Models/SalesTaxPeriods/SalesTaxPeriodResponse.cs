using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for a single sales tax period API response.
/// </summary>
public sealed class SalesTaxPeriodResponse
{
    /// <summary>
    /// Sales tax period payload.
    /// </summary>
    [JsonPropertyName("sales_tax_period")]
    public SalesTaxPeriod? SalesTaxPeriod { get; set; }
}
