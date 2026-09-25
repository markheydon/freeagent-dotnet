using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.StockItems;

/// <summary>
/// Wrapper for stock items list API responses.
/// </summary>
public sealed class StockItemsResponse
{
    /// <summary>
    /// Stock items list payload.
    /// </summary>
    [JsonPropertyName("stock_items")]
    public List<StockItem>? StockItems { get; set; }
}
