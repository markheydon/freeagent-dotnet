using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.StockItems;

/// <summary>
/// Represents a FreeAgent stock item.
/// </summary>
public sealed class StockItem : IFreeAgentResource
{
    /// <summary>
    /// Stock item resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Free-text description or code used when adding the item to an invoice or estimate.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Stock on hand as of the FreeAgent start date.
    /// </summary>
    [JsonPropertyName("opening_quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OpeningQuantity { get; set; }

    /// <summary>
    /// Value of stock on hand as of the FreeAgent start date.
    /// </summary>
    [JsonPropertyName("opening_balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OpeningBalance { get; set; }

    /// <summary>
    /// Wire representation of the cost of sales category link.
    /// </summary>
    [JsonPropertyName("cost_of_sale_category")]
    [JsonInclude]
    internal ExpandableField<Category>? CostOfSaleCategoryLink { get; set; }

    /// <summary>
    /// Cost of sales category nominal code when returned as a URI link.
    /// </summary>
    [JsonIgnore]
    public string? CostOfSaleCategoryNominalCode => CostOfSaleCategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : null;

    /// <summary>
    /// Cost of sales category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? CostOfSaleCategory => CostOfSaleCategoryLink?.Value;

    /// <summary>
    /// Stock on hand as of today.
    /// </summary>
    [JsonPropertyName("stock_on_hand")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StockOnHand { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
