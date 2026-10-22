using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent bank transaction explanation.
/// </summary>
public sealed class BankTransactionExplanation : IFreeAgentResource
{
    /// <summary>
    /// Explanation resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Read-only explanation type (for example Payment or Invoice Receipt).
    /// </summary>
    [JsonPropertyName("type")]
    public BankTransactionExplanationType? Type { get; set; }

    /// <summary>
    /// Wire representation of the parent bank transaction.
    /// </summary>
    [JsonPropertyName("bank_transaction")]
    [JsonInclude]
    internal ExpandableField<BankTransaction>? BankTransactionLink { get; set; }

    /// <summary>
    /// Parent bank transaction when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankTransaction? BankTransaction => BankTransactionLink?.Value;

    /// <summary>
    /// Parent bank transaction identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? BankTransactionId => BankTransactionLink?.Id;

    /// <summary>
    /// Wire representation of the bank account.
    /// </summary>
    [JsonPropertyName("bank_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? BankAccountLink { get; set; }

    /// <summary>
    /// Bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? BankAccount => BankAccountLink?.Value;

    /// <summary>
    /// Bank account identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId => BankAccountLink?.Id;

    /// <summary>
    /// Wire representation of the accounting category.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonInclude]
    internal ExpandableField<Category>? CategoryLink { get; set; }

    /// <summary>
    /// Category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? Category => CategoryLink?.Value;

    /// <summary>
    /// Category nominal code parsed from the response.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode => CategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : CategoryLink?.Value?.NominalCode;

    /// <summary>
    /// Date of the explanation.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Explanation entry type, as returned by FreeAgent (typically a category name).
    /// </summary>
    [JsonPropertyName("entry_type")]
    public string? EntryType { get; set; }

    /// <summary>
    /// Gross value of the explanation in the bank account currency.
    /// </summary>
    [JsonPropertyName("gross_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? GrossValue { get; set; }

    /// <summary>
    /// VAT status for reporting purposes.
    /// </summary>
    [JsonPropertyName("ec_status")]
    public BankTransactionExplanationEcStatus? EcStatus { get; set; }

    /// <summary>
    /// Place of supply when <see cref="EcStatus"/> is EC VAT MOSS.
    /// </summary>
    [JsonPropertyName("place_of_supply")]
    public EcMossPlaceOfSupply? PlaceOfSupply { get; set; }

    /// <summary>
    /// Sales tax rate applied to the explanation.
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
    /// Sales tax value for the explanation.
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
    /// Sales tax status for the explanation.
    /// </summary>
    [JsonPropertyName("sales_tax_status")]
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    /// <summary>
    /// Second sales tax status for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_status")]
    public SecondSalesTaxStatus? SecondSalesTaxStatus { get; set; }

    /// <summary>
    /// Cheque number when applicable.
    /// </summary>
    [JsonPropertyName("cheque_number")]
    public string? ChequeNumber { get; set; }

    /// <summary>
    /// Singular attachment metadata (legacy API shape).
    /// </summary>
    [JsonPropertyName("attachment")]
    public BillAttachment? Attachment { get; set; }

    /// <summary>
    /// Attachment metadata when using API version 2026-09-01 or later.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<BillAttachment>? Attachments { get; set; }

    /// <summary>
    /// Whether the explanation was guessed and awaits approval.
    /// </summary>
    [JsonPropertyName("marked_for_review")]
    public bool? MarkedForReview { get; set; }

    /// <summary>
    /// Whether this explanation represents money in.
    /// </summary>
    [JsonPropertyName("is_money_in")]
    public bool? IsMoneyIn { get; set; }

    /// <summary>
    /// Whether this explanation represents money out.
    /// </summary>
    [JsonPropertyName("is_money_out")]
    public bool? IsMoneyOut { get; set; }

    /// <summary>
    /// Whether money was paid to or received from a user.
    /// </summary>
    [JsonPropertyName("is_money_paid_to_user")]
    public bool? IsMoneyPaidToUser { get; set; }

    /// <summary>
    /// Whether the explanation cannot be changed.
    /// </summary>
    [JsonPropertyName("is_locked")]
    public bool? IsLocked { get; set; }

    /// <summary>
    /// Attributes that cannot be modified when locked.
    /// </summary>
    [JsonPropertyName("locked_attributes")]
    public List<string>? LockedAttributes { get; set; }

    /// <summary>
    /// Reason the explanation is locked.
    /// </summary>
    [JsonPropertyName("locked_reason")]
    public string? LockedReason { get; set; }

    /// <summary>
    /// Whether the explanation can be deleted.
    /// </summary>
    [JsonPropertyName("is_deletable")]
    public bool? IsDeletable { get; set; }

    /// <summary>
    /// Wire representation of the linked project.
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
    /// Project identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId => ProjectLink?.Id;

    /// <summary>
    /// Rebill type for payment or refund explanations.
    /// </summary>
    [JsonPropertyName("rebill_type")]
    public BillRebillType? RebillType { get; set; }

    /// <summary>
    /// Rebill factor when rebilling by markup or price.
    /// </summary>
    [JsonPropertyName("rebill_factor")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RebillFactor { get; set; }

    /// <summary>
    /// Receipt reference for payment or refund explanations.
    /// </summary>
    [JsonPropertyName("receipt_reference")]
    public string? ReceiptReference { get; set; }

    /// <summary>
    /// Wire representation of the paid invoice or refunded credit note.
    /// </summary>
    [JsonPropertyName("paid_invoice")]
    [JsonInclude]
    internal ExpandableField<Invoice>? PaidInvoiceLink { get; set; }

    /// <summary>
    /// Invoice when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Invoice? PaidInvoice => PaidInvoiceLink?.Value;

    /// <summary>
    /// Invoice identifier when <c>paid_invoice</c> links to an invoice.
    /// </summary>
    [JsonIgnore]
    public long? PaidInvoiceId => GetPaidInvoiceLinkId("invoices");

    /// <summary>
    /// Credit note identifier when <c>paid_invoice</c> links to a credit note refund.
    /// </summary>
    [JsonIgnore]
    public long? PaidCreditNoteId => GetPaidInvoiceLinkId("credit_notes");

    /// <summary>
    /// Hydrated credit note when <c>paid_invoice</c> links to a credit note and <see cref="BankTransactionExplanationGetOptions.IncludePaidCreditNote"/> is used.
    /// </summary>
    [JsonIgnore]
    internal ExpandableField<CreditNote>? PaidCreditNoteHydrationLink { get; set; }

    /// <summary>
    /// Credit note when returned nested on the wire or hydrated via <see cref="BankTransactionExplanationGetOptions.IncludePaidCreditNote"/>.
    /// </summary>
    [JsonIgnore]
    public CreditNote? PaidCreditNote => PaidCreditNoteHydrationLink?.Value;

    /// <summary>
    /// Foreign currency value when explaining a foreign currency invoice.
    /// </summary>
    [JsonPropertyName("foreign_currency_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ForeignCurrencyValue { get; set; }

    /// <summary>
    /// Wire representation of the paid or refunded bill.
    /// </summary>
    [JsonPropertyName("paid_bill")]
    [JsonInclude]
    internal ExpandableField<Bill>? PaidBillLink { get; set; }

    /// <summary>
    /// Bill when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Bill? PaidBill => PaidBillLink?.Value;

    /// <summary>
    /// Bill identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? PaidBillId => PaidBillLink?.Id;

    /// <summary>
    /// Wire representation of the paid or receiving user.
    /// </summary>
    [JsonPropertyName("paid_user")]
    [JsonInclude]
    internal ExpandableField<User>? PaidUserLink { get; set; }

    /// <summary>
    /// User when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public User? PaidUser => PaidUserLink?.Value;

    /// <summary>
    /// User identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? PaidUserId => PaidUserLink?.Id;

    /// <summary>
    /// Wire representation of the transfer destination or source bank account.
    /// </summary>
    [JsonPropertyName("transfer_bank_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? TransferBankAccountLink { get; set; }

    /// <summary>
    /// Transfer bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? TransferBankAccount => TransferBankAccountLink?.Value;

    /// <summary>
    /// Transfer bank account identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? TransferBankAccountId => TransferBankAccountLink?.Id;

    /// <summary>
    /// Wire representation of the paired transfer explanation.
    /// </summary>
    [JsonPropertyName("linked_transfer_explanation")]
    [JsonInclude]
    internal ExpandableField<BankTransactionExplanation>? LinkedTransferExplanationLink { get; set; }

    /// <summary>
    /// Paired transfer explanation when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankTransactionExplanation? LinkedTransferExplanation => LinkedTransferExplanationLink?.Value;

    /// <summary>
    /// Paired transfer explanation identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? LinkedTransferExplanationId => LinkedTransferExplanationLink?.Id;

    /// <summary>
    /// Wire representation of the paired transfer bank account.
    /// </summary>
    [JsonPropertyName("linked_transfer_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? LinkedTransferAccountLink { get; set; }

    /// <summary>
    /// Paired transfer bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? LinkedTransferAccount => LinkedTransferAccountLink?.Value;

    /// <summary>
    /// Paired transfer bank account identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? LinkedTransferAccountId => LinkedTransferAccountLink?.Id;

    /// <summary>
    /// Wire representation of the stock item.
    /// </summary>
    [JsonPropertyName("stock_item")]
    [JsonInclude]
    internal ExpandableField<StockItem>? StockItemLink { get; set; }

    /// <summary>
    /// Stock item when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public StockItem? StockItem => StockItemLink?.Value;

    /// <summary>
    /// Stock item identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? StockItemId => StockItemLink?.Id;

    /// <summary>
    /// Quantity of stock purchased or sold.
    /// </summary>
    [JsonPropertyName("stock_altering_quantity")]
    public int? StockAlteringQuantity { get; set; }

    /// <summary>
    /// Wire representation of the capital asset link on read responses.
    /// </summary>
    [JsonPropertyName("capital_asset")]
    [JsonInclude]
    internal string? CapitalAssetWire { get; set; }

    /// <summary>
    /// Capital asset identifier for purchases. Read-only from the API.
    /// </summary>
    [JsonIgnore]
    public long? CapitalAssetId =>
        CapitalAssetReference.TryParse(CapitalAssetWire, out var reference) ? reference.Id : null;

    /// <summary>
    /// Deprecated depreciation years hint for capital asset purchases.
    /// </summary>
    [JsonPropertyName("asset_life_years")]
    public string? AssetLifeYears { get; set; }

    /// <summary>
    /// Wire representation of the disposed capital asset link on read responses.
    /// </summary>
    [JsonPropertyName("disposed_asset")]
    [JsonInclude]
    internal string? DisposedAssetWire { get; set; }

    /// <summary>
    /// Disposed capital asset identifier for disposals. Read-only from the API.
    /// </summary>
    [JsonIgnore]
    public long? DisposedAssetId =>
        CapitalAssetReference.TryParse(DisposedAssetWire, out var reference) ? reference.Id : null;

    /// <summary>
    /// Wire representation of the property link.
    /// </summary>
    [JsonPropertyName("property")]
    [JsonInclude]
    internal ExpandableField<LandlordProperty>? PropertyLink { get; set; }

    /// <summary>
    /// Landlord property when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public LandlordProperty? Property => PropertyLink?.Value;

    /// <summary>
    /// Property identifier parsed from the response for UK unincorporated landlord companies.
    /// </summary>
    [JsonIgnore]
    public long? PropertyId => PropertyLink?.Id;

    /// <summary>
    /// Wire representation of the opening balance contact.
    /// </summary>
    [JsonPropertyName("direct_contact")]
    [JsonInclude]
    internal ExpandableField<Contact>? DirectContactLink { get; set; }

    /// <summary>
    /// Contact when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Contact? DirectContact => DirectContactLink?.Value;

    /// <summary>
    /// Contact identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? DirectContactId => DirectContactLink?.Id;

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    internal void AttachCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        CategoryLink = new ExpandableField<Category>(category.Url, category);
    }

    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    internal void AttachBankAccount(BankAccount bankAccount)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        BankAccountLink = new ExpandableField<BankAccount>(bankAccount.Url, bankAccount);
    }

    internal void AttachPaidInvoice(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        PaidInvoiceLink = new ExpandableField<Invoice>(invoice.Url, invoice);
    }

    internal void AttachPaidCreditNote(CreditNote creditNote)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        PaidCreditNoteHydrationLink = new ExpandableField<CreditNote>(creditNote.Url, creditNote);
    }

    internal void AttachPaidBill(Bill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        PaidBillLink = new ExpandableField<Bill>(bill.Url, bill);
    }

    internal void AttachPaidUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        PaidUserLink = new ExpandableField<User>(user.Url, user);
    }

    internal void AttachDirectContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        DirectContactLink = new ExpandableField<Contact>(contact.Url, contact);
    }

    internal void AttachTransferBankAccount(BankAccount bankAccount)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        TransferBankAccountLink = new ExpandableField<BankAccount>(bankAccount.Url, bankAccount);
    }

    internal void AttachStockItem(StockItem stockItem)
    {
        ArgumentNullException.ThrowIfNull(stockItem);
        StockItemLink = new ExpandableField<StockItem>(stockItem.Url, stockItem);
    }

    private long? GetPaidInvoiceLinkId(string resourceSegment)
    {
        if (PaidInvoiceLink?.Uri is not string uri)
        {
            return null;
        }

        if (!uri.Contains($"/{resourceSegment}/", StringComparison.OrdinalIgnoreCase)
            && !uri.EndsWith($"/{resourceSegment}", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return PaidInvoiceLink.Id;
    }
}
