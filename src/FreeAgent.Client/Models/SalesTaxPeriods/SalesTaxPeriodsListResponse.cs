using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for a sales tax periods list API response.
/// </summary>
internal sealed class SalesTaxPeriodsListResponse
{
    /// <summary>
    /// Sales tax period collection payload.
    /// </summary>
    [JsonPropertyName("sales_tax_periods")]
    public List<SalesTaxPeriod>? SalesTaxPeriods { get; set; }
}
