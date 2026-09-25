using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Represents a line item on a FreeAgent estimate.
/// </summary>
public sealed class EstimateItem
{
    private SettableLinkValue _categoryNominalCode;

    /// <summary>
    /// Estimate item resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Position on the estimate, starting at 1.
    /// </summary>
    [JsonPropertyName("position")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Position { get; set; }

    /// <summary>
    /// Line item type.
    /// </summary>
    [JsonPropertyName("item_type")]
    [JsonConverter(typeof(NullableEstimateItemTypeJsonConverter))]
    public EstimateItemType? ItemType { get; set; }

    /// <summary>
    /// Quantity.
    /// </summary>
    [JsonPropertyName("quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Unit price.
    /// </summary>
    [JsonPropertyName("price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Price { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Standard sales tax rate.
    /// </summary>
    [JsonPropertyName("sales_tax_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxRate { get; set; }

    /// <summary>
    /// Total amount of sales tax.
    /// </summary>
    [JsonPropertyName("sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxValue { get; set; }

    /// <summary>
    /// Second sales tax rate for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxRate { get; set; }

    /// <summary>
    /// Sales tax status for the line item.
    /// </summary>
    [JsonPropertyName("sales_tax_status")]
    public InvoiceSalesTaxStatus? SalesTaxStatus { get; set; }

    /// <summary>
    /// Second sales tax status for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_status")]
    public string? SecondSalesTaxStatus { get; set; }

    /// <summary>
    /// Total amount of second sales tax for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxValue { get; set; }

    /// <summary>
    /// Wire representation of the category link.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonInclude]
    internal ExpandableField<Category>? CategoryLink { get; set; }

    /// <summary>
    /// Category nominal code for create and update requests. Populated after GET when the API returns a category link.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode
    {
        get => _categoryNominalCode.Get(
            CategoryLink?.Uri is string uri ? CategoryReference.ExtractNominalCode(uri) : null);
        set => _categoryNominalCode.Set(value);
    }

    /// <summary>
    /// Category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? Category => CategoryLink?.Value;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Wire representation of the estimate item identifier returned on read responses.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonInclude]
    internal long? WireItemId { get; set; }

    private long? _explicitItemId;

    /// <summary>
    /// Estimate item identifier used when updating or deleting an existing line item.
    /// </summary>
    [JsonIgnore]
    public long? ItemId
    {
        get => _explicitItemId ?? WireItemId ?? TryParseItemIdFromUrl();
        set => _explicitItemId = value;
    }

    /// <summary>
    /// When set to <c>1</c>, deletes the line item on update.
    /// </summary>
    [JsonIgnore]
    public int? Destroy { get; set; }

    internal SettableLinkValue CategoryNominalCodeBacking => _categoryNominalCode;

    internal string? CategoryLinkNominalCode =>
        CategoryLink?.Uri is string uri ? CategoryReference.ExtractNominalCode(uri) : null;

    private long? TryParseItemIdFromUrl() =>
        !string.IsNullOrWhiteSpace(Url) && FreeAgentResourceId.TryParse(Url, out var id) ? id : null;
}
