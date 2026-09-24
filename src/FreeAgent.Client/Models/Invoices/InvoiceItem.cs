using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Represents a line item on a FreeAgent invoice.
/// </summary>
public sealed class InvoiceItem
{
    /// <summary>
    /// Invoice item resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Position in the invoice, starting at 1.
    /// </summary>
    [JsonPropertyName("position")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Position { get; set; }

    /// <summary>
    /// Line item type.
    /// </summary>
    [JsonPropertyName("item_type")]
    [JsonConverter(typeof(NullableInvoiceItemTypeJsonConverter))]
    public InvoiceItemType? ItemType { get; set; }

    /// <summary>
    /// Quantity of the item type.
    /// </summary>
    [JsonPropertyName("quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Invoice item details.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Unit price.
    /// </summary>
    [JsonPropertyName("price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Price { get; set; }

    /// <summary>
    /// Standard sales tax rate.
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
    /// Wire representation of the stock item link.
    /// </summary>
    [JsonPropertyName("stock_item")]
    public string? StockItemUri { get; set; }

    /// <summary>
    /// Wire representation of the category link.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonInclude]
    internal ExpandableField<Category>? CategoryLink { get; set; }

    /// <summary>
    /// Category nominal code when returned as a URI link.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode => CategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : null;

    /// <summary>
    /// Category to assign on create or update requests.
    /// </summary>
    [JsonIgnore]
    public CategoryReference? Category { get; set; }

    /// <summary>
    /// Wire representation of the project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Project when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Project? Project => ProjectLink?.Value;

    /// <summary>
    /// Project identifier parsed from the line item response.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId => ProjectLink?.Id;

    /// <summary>
    /// Project to assign on create or update requests.
    /// </summary>
    [JsonIgnore]
    public ProjectReference? LinkedProject { get; set; }

    /// <summary>
    /// Wire representation of the invoice item identifier returned on read responses.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonInclude]
    internal long? WireItemId { get; set; }

    private long? _explicitItemId;

    /// <summary>
    /// Invoice item identifier used when updating or deleting an existing line item.
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

    /// <summary>
    /// Stock item URI to assign on create or update requests.
    /// </summary>
    /// <remarks>
    /// Full stock item operations are not yet implemented in the SDK.
    /// </remarks>
    [JsonIgnore]
    public string? StockItem { get; set; }

    private long? TryParseItemIdFromUrl() =>
        !string.IsNullOrWhiteSpace(Url) && FreeAgentResourceId.TryParse(Url, out var id) ? id : null;
}
