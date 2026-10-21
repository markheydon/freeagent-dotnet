using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.DepreciationProfiles;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.StockItems;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Represents a line item on a FreeAgent bill.
/// </summary>
public sealed class BillItem
{
    private SettableLinkId _stockItemId;
    private SettableLinkId _projectId;
    private SettableLinkValue _categoryNominalCode;

    /// <summary>
    /// Bill item resource URL. Use an empty string when creating a new item on update.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Wire representation of the parent bill link.
    /// </summary>
    [JsonPropertyName("bill")]
    [JsonInclude]
    internal ExpandableField<Bill>? BillLink { get; set; }

    /// <summary>
    /// Parent bill when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Bill? Bill => BillLink?.Value;

    /// <summary>
    /// Parent bill identifier parsed from the bill link on read responses.
    /// </summary>
    [JsonIgnore]
    public long? BillId => BillLink?.Id;

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
    /// Description of the bill item.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Value of the item including taxes.
    /// </summary>
    [JsonPropertyName("total_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalValue { get; set; }

    /// <summary>
    /// Value of the item excluding taxes.
    /// </summary>
    [JsonPropertyName("total_value_ex_tax")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalValueExTax { get; set; }

    /// <summary>
    /// Manual sales tax amount in the company's native currency.
    /// </summary>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ManualSalesTaxAmount { get; set; }

    /// <summary>
    /// Standard sales tax rate.
    /// </summary>
    [JsonPropertyName("sales_tax_rate")]
    public PurchaseSalesTaxRate? SalesTaxRate { get; set; }

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
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    /// <summary>
    /// Second sales tax status for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_status")]
    public SecondSalesTaxStatus? SecondSalesTaxStatus { get; set; }

    /// <summary>
    /// Sales tax value for the line item.
    /// </summary>
    [JsonPropertyName("sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxValue { get; set; }

    /// <summary>
    /// Second sales tax value for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxValue { get; set; }

    /// <summary>
    /// Unit of measure for the line item.
    /// </summary>
    [JsonPropertyName("unit")]
    public BillItemUnit? Unit { get; set; }

    /// <summary>
    /// Quantity of the <see cref="Unit"/>.
    /// </summary>
    [JsonPropertyName("quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Wire representation of the stock item link.
    /// </summary>
    [JsonPropertyName("stock_item")]
    [JsonInclude]
    internal ExpandableField<StockItem>? StockItemLink { get; set; }

    /// <summary>
    /// Stock item identifier for create and update requests. Populated after GET when the API returns a stock item link.
    /// </summary>
    [JsonIgnore]
    public long? StockItemId
    {
        get => _stockItemId.Get(StockItemLink?.Id);
        set => _stockItemId.Set(value);
    }

    /// <summary>
    /// Stock item when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public StockItem? StockItem => StockItemLink?.Value;

    /// <summary>
    /// Description of the stock item. Read-only when a stock item is set.
    /// </summary>
    [JsonPropertyName("stock_item_description")]
    public string? StockItemDescription { get; set; }

    /// <summary>
    /// Quantity of stock item units purchased.
    /// </summary>
    [JsonPropertyName("stock_altering_quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StockAlteringQuantity { get; set; }

    /// <summary>
    /// Deprecated depreciation schedule field for capital asset purchases.
    /// </summary>
    [JsonPropertyName("depreciation_schedule")]
    public string? DepreciationSchedule { get; set; }

    /// <summary>
    /// Capital asset resource URI. Read-only on the wire.
    /// </summary>
    [JsonPropertyName("capital_asset")]
    public string? CapitalAssetUri { get; set; }

    /// <summary>
    /// Capital asset purchase attributes for create and update requests.
    /// </summary>
    [JsonIgnore]
    public CapitalAssetPurchaseWrite? CapitalAssetPurchase { get; set; }

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
    /// Project identifier for create and update requests. Populated after GET when the API returns a project link.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId
    {
        get => _projectId.Get(ProjectLink?.Id);
        set => _projectId.Set(value);
    }

    /// <summary>
    /// CIS deduction rate percentage for the line item.
    /// </summary>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeductionRate { get; set; }

    /// <summary>
    /// Bill item identifier parsed from <see cref="Url"/>.
    /// </summary>
    [JsonIgnore]
    public long? ItemId => TryParseItemIdFromUrl();

    /// <summary>
    /// When set to <c>1</c>, deletes the line item on update.
    /// </summary>
    [JsonIgnore]
    public int? Destroy { get; set; }

    internal SettableLinkId StockItemIdBacking => _stockItemId;

    internal long? StockItemLinkId => StockItemLink?.Id;

    internal SettableLinkValue CategoryNominalCodeBacking => _categoryNominalCode;

    internal string? CategoryLinkNominalCode =>
        CategoryLink?.Uri is string uri ? CategoryReference.ExtractNominalCode(uri) : null;

    internal SettableLinkId ProjectIdBacking => _projectId;

    internal long? ProjectLinkId => ProjectLink?.Id;

    private long? TryParseItemIdFromUrl() =>
        !string.IsNullOrWhiteSpace(Url) && FreeAgentResourceId.TryParse(Url, out var id) ? id : null;
}
