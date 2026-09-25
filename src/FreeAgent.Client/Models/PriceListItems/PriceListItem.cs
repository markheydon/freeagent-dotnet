using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.StockItems;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Represents a FreeAgent price list item.
/// </summary>
public sealed class PriceListItem : IFreeAgentResource
{
    /// <summary>
    /// Price list item resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Unique code used to identify the item when adding it to an invoice or estimate.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Item quantity.
    /// </summary>
    [JsonPropertyName("quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Line item type.
    /// </summary>
    [JsonPropertyName("item_type")]
    [JsonConverter(typeof(NullableInvoiceItemTypeJsonConverter))]
    public InvoiceItemType? ItemType { get; set; }

    /// <summary>
    /// Free-text description of the item.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Unit price of one item.
    /// </summary>
    [JsonPropertyName("price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Price { get; set; }

    /// <summary>
    /// UK VAT status for the item.
    /// </summary>
    [JsonPropertyName("vat_status")]
    public PriceListItemVatStatus? VatStatus { get; set; }

    /// <summary>
    /// Standard sales tax rate for universal and US accounts.
    /// </summary>
    [JsonPropertyName("sales_tax_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxRate { get; set; }

    /// <summary>
    /// Second sales tax rate for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxRate { get; set; }

    /// <summary>
    /// Wire representation of the income category link.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonInclude]
    internal ExpandableField<Category>? CategoryLink { get; set; }

    /// <summary>
    /// Income category nominal code when returned as a URI link.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode => CategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : null;

    /// <summary>
    /// Income category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? Category => CategoryLink?.Value;

    /// <summary>
    /// Wire representation of the stock item link.
    /// </summary>
    [JsonPropertyName("stock_item")]
    [JsonInclude]
    internal ExpandableField<StockItem>? StockItemLink { get; set; }

    /// <summary>
    /// Stock item identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? StockItemId => StockItemLink?.Id;

    /// <summary>
    /// Stock item when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public StockItem? StockItemResource => StockItemLink?.Value;

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
