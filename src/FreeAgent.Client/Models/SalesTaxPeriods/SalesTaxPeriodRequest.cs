using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.SalesTaxPeriods;

/// <summary>
/// Wrapper for sales tax period write API requests.
/// </summary>
/// <typeparam name="T">Create or update payload type.</typeparam>
internal sealed class SalesTaxPeriodRequest<T>
{
    /// <summary>
    /// Sales tax period payload.
    /// </summary>
    [JsonPropertyName("sales_tax_period")]
    public T SalesTaxPeriod { get; set; } = default!;
}
