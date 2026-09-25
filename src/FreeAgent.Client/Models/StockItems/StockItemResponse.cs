using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.StockItems;

/// <summary>
/// Wrapper for a single stock item API response.
/// </summary>
public sealed class StockItemResponse
{
    /// <summary>
    /// Stock item payload.
    /// </summary>
    [JsonPropertyName("stock_item")]
    public StockItem? StockItem { get; set; }
}
