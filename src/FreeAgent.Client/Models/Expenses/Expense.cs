using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent expense.
/// </summary>
public sealed class Expense : IFreeAgentResource
{
    private SettableLinkId _userId;
    private SettableLinkValue _categoryNominalCode;
    private bool? _isMileageCategory;
    private SettableLinkId _projectId;
    private SettableLinkId _rebillToProjectId;
    private SettableLinkId _stockItemId;

    /// <summary>
    /// Expense resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Wire representation of the user link.
    /// </summary>
    [JsonPropertyName("user")]
    [JsonInclude]
    internal ExpandableField<User>? UserLink { get; set; }

    /// <summary>
    /// Claimant when returned nested on the wire or hydrated via <see cref="ExpenseGetOptions.IncludeUser"/>.
    /// </summary>
    [JsonIgnore]
    public User? User => UserLink?.Value;

    /// <summary>
    /// User identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? UserId
    {
        get => _userId.Get(UserLink?.Id);
        set => _userId.Set(value);
    }

    /// <summary>
    /// Wire representation of the category link or mileage literal.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonConverter(typeof(ExpenseCategoryJsonConverter))]
    [JsonInclude]
    internal ExpenseCategoryField? CategoryLink { get; set; }

    /// <summary>
    /// Category when returned nested on the wire or hydrated via <see cref="ExpenseGetOptions.IncludeCategory"/>.
    /// </summary>
    [JsonIgnore]
    public Category? Category => CategoryLink?.Value;

    /// <summary>
    /// When <see langword="true"/>, the expense uses the mileage category rather than an accounting category.
    /// </summary>
    [JsonIgnore]
    public bool IsMileageCategory
    {
        get => _isMileageCategory ?? CategoryLink?.IsMileage == true;
        set => _isMileageCategory = value;
    }

    /// <summary>
    /// Category nominal code for create and update requests when <see cref="IsMileageCategory"/> is <see langword="false"/>.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode
    {
        get => _categoryNominalCode.Get(CategoryLink?.NominalCode);
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _isMileageCategory = false;
            }

            _categoryNominalCode.Set(value);
        }
    }

    /// <summary>
    /// Date of the expense.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Expense currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Total value in the expense currency.
    /// </summary>
    [JsonPropertyName("gross_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? GrossValue { get; set; }

    /// <summary>
    /// Total value in the company's native currency.
    /// </summary>
    [JsonPropertyName("native_gross_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? NativeGrossValue { get; set; }

    /// <summary>
    /// Sales tax rate.
    /// </summary>
    [JsonPropertyName("sales_tax_rate")]
    public PurchaseSalesTaxRate? SalesTaxRate { get; set; }

    /// <summary>
    /// Total sales tax value.
    /// </summary>
    [JsonPropertyName("sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxValue { get; set; }

    /// <summary>
    /// Total sales tax value in the company's native currency.
    /// </summary>
    [JsonPropertyName("native_sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? NativeSalesTaxValue { get; set; }

    /// <summary>
    /// Second sales tax rate for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxRate { get; set; }

    /// <summary>
    /// Manual sales tax amount in the company's native currency.
    /// </summary>
    [JsonPropertyName("manual_sales_tax_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ManualSalesTaxAmount { get; set; }

    /// <summary>
    /// Sales tax status for the expense.
    /// </summary>
    [JsonPropertyName("sales_tax_status")]
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    /// <summary>
    /// Second sales tax status for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_status")]
    public SecondSalesTaxStatus? SecondSalesTaxStatus { get; set; }

    /// <summary>
    /// VAT status for reporting purposes.
    /// </summary>
    [JsonPropertyName("ec_status")]
    public BillEcStatus? EcStatus { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Receipt reference.
    /// </summary>
    [JsonPropertyName("receipt_reference")]
    public string? ReceiptReference { get; set; }

    /// <summary>
    /// Wire representation of the stock item link.
    /// </summary>
    [JsonPropertyName("stock_item")]
    [JsonInclude]
    internal ExpandableField<StockItem>? StockItemLink { get; set; }

    /// <summary>
    /// Stock item when returned nested on the wire or hydrated via <see cref="ExpenseGetOptions.IncludeStockItem"/>.
    /// </summary>
    [JsonIgnore]
    public StockItem? StockItem => StockItemLink?.Value;

    /// <summary>
    /// Stock item identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? StockItemId
    {
        get => _stockItemId.Get(StockItemLink?.Id);
        set => _stockItemId.Set(value);
    }

    /// <summary>
    /// Description of the stock item when returned by the API.
    /// </summary>
    [JsonPropertyName("stock_item_description")]
    public string? StockItemDescription { get; set; }

    /// <summary>
    /// Quantity of stock units purchased.
    /// </summary>
    [JsonPropertyName("stock_altering_quantity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StockAlteringQuantity { get; set; }

    /// <summary>
    /// Wire representation of the project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Project when returned nested on the wire or hydrated via <see cref="ExpenseGetOptions.IncludeProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? Project => ProjectLink?.Value;

    /// <summary>
    /// Project identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId
    {
        get => _projectId.Get(ProjectLink?.Id);
        set => _projectId.Set(value);
    }

    /// <summary>
    /// Project rebill calculation type.
    /// </summary>
    [JsonPropertyName("rebill_type")]
    public BillRebillType? RebillType { get; set; }

    /// <summary>
    /// Rebill factor when <see cref="RebillType"/> is markup or price.
    /// </summary>
    [JsonPropertyName("rebill_factor")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RebillFactor { get; set; }

    /// <summary>
    /// Wire representation of the rebill-to-project link.
    /// </summary>
    [JsonPropertyName("rebill_to_project")]
    [JsonInclude]
    internal ExpandableField<Project>? RebillToProjectLink { get; set; }

    /// <summary>
    /// Rebill-to project when returned nested on the wire or hydrated via <see cref="ExpenseGetOptions.IncludeRebillToProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? RebillToProject => RebillToProjectLink?.Value;

    /// <summary>
    /// Rebill-to project identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? RebillToProjectId
    {
        get => _rebillToProjectId.Get(RebillToProjectLink?.Id);
        set => _rebillToProjectId.Set(value);
    }

    /// <summary>
    /// Wire representation of the invoice where the expense was rebilled.
    /// </summary>
    [JsonPropertyName("rebilled_on_invoice")]
    [JsonInclude]
    internal ExpandableField<Invoice>? RebilledOnInvoiceLink { get; set; }

    /// <summary>
    /// Invoice when returned nested on the wire or hydrated via <see cref="ExpenseGetOptions.IncludeRebilledOnInvoice"/>.
    /// </summary>
    [JsonIgnore]
    public Invoice? RebilledOnInvoice => RebilledOnInvoiceLink?.Value;

    /// <summary>
    /// Invoice identifier when the expense has been rebilled. Read-only from the API.
    /// </summary>
    [JsonIgnore]
    public long? RebilledOnInvoiceId => RebilledOnInvoiceLink?.Id;

    /// <summary>
    /// Property URI for landlord companies. Full property operations are not yet implemented in the SDK.
    /// </summary>
    [JsonPropertyName("property")]
    public string? PropertyUri { get; set; }

    /// <summary>
    /// Recurrence frequency.
    /// </summary>
    [JsonPropertyName("recurring")]
    public BillRecurringSchedule? Recurring { get; set; }

    /// <summary>
    /// Next recurrence date.
    /// </summary>
    [JsonPropertyName("next_recurs_on")]
    public DateOnly? NextRecursOn { get; set; }

    /// <summary>
    /// Date when recurrence ends.
    /// </summary>
    [JsonPropertyName("recurring_end_date")]
    public DateOnly? RecurringEndDate { get; set; }

    /// <summary>
    /// Attachment metadata returned on read responses.
    /// </summary>
    [JsonPropertyName("attachment")]
    public ExpenseAttachment? Attachment { get; set; }

    /// <summary>
    /// Attachment upload or link payload for create and update requests.
    /// </summary>
    [JsonIgnore]
    public ExpenseAttachmentWritePayload? AttachmentWrite { get; set; }

    /// <summary>
    /// Capital asset URI for capital purchases. Read-only on responses.
    /// </summary>
    [JsonPropertyName("capital_asset")]
    public string? CapitalAssetUri { get; set; }

    /// <summary>
    /// Capital asset purchase attributes for create and update requests.
    /// </summary>
    [JsonIgnore]
    public CapitalAssetPurchaseWrite? CapitalAssetPurchase { get; set; }

    /// <summary>
    /// Deprecated depreciation schedule hint. Prefer fetching the linked capital asset.
    /// </summary>
    [JsonPropertyName("depreciation_schedule")]
    public string? DepreciationSchedule { get; set; }

    /// <summary>
    /// Miles travelled for mileage claims.
    /// </summary>
    [JsonPropertyName("mileage")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Mileage { get; set; }

    /// <summary>
    /// Vehicle type for mileage claims.
    /// </summary>
    [JsonPropertyName("vehicle_type")]
    public ExpenseVehicleType? VehicleType { get; set; }

    /// <summary>
    /// Engine type for mileage claims when the vehicle is a car or motorcycle.
    /// </summary>
    [JsonPropertyName("engine_type")]
    public ExpenseEngineType? EngineType { get; set; }

    /// <summary>
    /// Engine size for mileage claims when the vehicle is a car or motorcycle.
    /// </summary>
    [JsonPropertyName("engine_size")]
    public string? EngineSize { get; set; }

    /// <summary>
    /// Whether mileage is reclaimed at the AMAP rate.
    /// </summary>
    [JsonPropertyName("reclaim_mileage")]
    public ExpenseReclaimMileage? ReclaimMileage { get; set; }

    /// <summary>
    /// Rate at which mileage is reclaimed under the HMRC threshold.
    /// </summary>
    [JsonPropertyName("initial_rate_mileage")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? InitialRateMileage { get; set; }

    /// <summary>
    /// Rate at which mileage is reclaimed.
    /// </summary>
    [JsonPropertyName("reclaim_mileage_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ReclaimMileageRate { get; set; }

    /// <summary>
    /// Rate at which mileage is rebilled.
    /// </summary>
    [JsonPropertyName("rebill_mileage_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RebillMileageRate { get; set; }

    /// <summary>
    /// Whether a VAT receipt is held when reclaiming mileage.
    /// </summary>
    [JsonPropertyName("have_vat_receipt")]
    public bool? HaveVatReceipt { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    internal SettableLinkValue CategoryNominalCodeBacking => _categoryNominalCode;

    internal bool? IsMileageCategoryBacking => _isMileageCategory;

    internal string? CategoryLinkNominalCode => CategoryLink?.NominalCode;

    internal bool CategoryLinkIsMileage => CategoryLink?.IsMileage == true;

    internal SettableLinkId UserIdBacking => _userId;

    internal long? UserLinkId => UserLink?.Id;

    internal SettableLinkId ProjectIdBacking => _projectId;

    internal long? ProjectLinkId => ProjectLink?.Id;

    internal SettableLinkId RebillToProjectIdBacking => _rebillToProjectId;

    internal long? RebillToProjectLinkId => RebillToProjectLink?.Id;

    internal SettableLinkId StockItemIdBacking => _stockItemId;

    internal long? StockItemLinkId => StockItemLink?.Id;

    internal void AttachUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        UserLink = new ExpandableField<User>(user.Url, user);
    }

    internal void AttachCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        CategoryLink = new ExpenseCategoryField(category.Url, category);
    }

    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    internal void AttachRebillToProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        RebillToProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    internal void AttachStockItem(StockItem stockItem)
    {
        ArgumentNullException.ThrowIfNull(stockItem);
        StockItemLink = new ExpandableField<StockItem>(stockItem.Url, stockItem);
    }

    internal void AttachRebilledOnInvoice(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        RebilledOnInvoiceLink = new ExpandableField<Invoice>(invoice.Url, invoice);
    }
}
