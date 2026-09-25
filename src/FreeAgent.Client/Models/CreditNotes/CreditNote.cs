using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Represents a FreeAgent credit note.
/// </summary>
public sealed class CreditNote : IFreeAgentResource
{
    private SettableLinkId _contactId;
    private SettableLinkId _projectId;
    private SettableLinkId _bankAccountId;

    /// <summary>
    /// Credit note resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Credit note status.
    /// </summary>
    [JsonPropertyName("status")]
    public CreditNoteStatus? Status { get; set; }

    /// <summary>
    /// Credit note status with relative due date text.
    /// </summary>
    [JsonPropertyName("long_status")]
    public string? LongStatus { get; set; }

    /// <summary>
    /// Wire representation of the contact link.
    /// </summary>
    [JsonPropertyName("contact")]
    [JsonInclude]
    internal ExpandableField<Contact>? ContactLink { get; set; }

    /// <summary>
    /// Contact when returned nested on the wire or hydrated via <see cref="CreditNoteGetOptions.IncludeContact"/>.
    /// </summary>
    [JsonIgnore]
    public Contact? Contact => ContactLink?.Value;

    /// <summary>
    /// Contact identifier for create and update requests. Populated after GET when the API returns a contact link.
    /// </summary>
    [JsonIgnore]
    public long? ContactId
    {
        get => _contactId.Get(ContactLink?.Id);
        set => _contactId.Set(value);
    }

    /// <summary>
    /// Display name of the contact for this credit note.
    /// </summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>
    /// Wire representation of the project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Project when returned nested on the wire or hydrated via <see cref="CreditNoteGetOptions.IncludeProject"/>.
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
    /// Property URI for landlord companies.
    /// </summary>
    /// <remarks>
    /// Full property operations are not yet implemented in the SDK.
    /// </remarks>
    [JsonPropertyName("property")]
    public string? PropertyUri { get; set; }

    /// <summary>
    /// Credit note reference.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Date of the credit note.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Credit note due date.
    /// </summary>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    /// <summary>
    /// Payment terms in days. Set to zero for due on receipt.
    /// </summary>
    [JsonPropertyName("payment_terms_in_days")]
    public int? PaymentTermsInDays { get; set; }

    /// <summary>
    /// Credit note currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Construction Industry Scheme rate band name.
    /// </summary>
    [JsonPropertyName("cis_rate")]
    public string? CisRate { get; set; }

    /// <summary>
    /// CIS deduction rate percentage.
    /// </summary>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeductionRate { get; set; }

    /// <summary>
    /// Total CIS deduction for this credit note.
    /// </summary>
    [JsonPropertyName("cis_deduction")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeduction { get; set; }

    /// <summary>
    /// CIS deduction already paid for this credit note.
    /// </summary>
    [JsonPropertyName("cis_deduction_suffered")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeductionSuffered { get; set; }

    /// <summary>
    /// Additional text added to the bottom of the credit note.
    /// </summary>
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }

    /// <summary>
    /// Discount applied across the whole credit note.
    /// </summary>
    [JsonPropertyName("discount_percent")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DiscountPercent { get; set; }

    /// <summary>
    /// Client contact name override for this credit note.
    /// </summary>
    [JsonPropertyName("client_contact_name")]
    public string? ClientContactName { get; set; }

    /// <summary>
    /// Payment terms override for this credit note.
    /// </summary>
    [JsonPropertyName("payment_terms")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Purchase order reference override.
    /// </summary>
    [JsonPropertyName("po_reference")]
    public string? PoReference { get; set; }

    /// <summary>
    /// Wire representation of the bank account link.
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
    /// Bank account identifier for create and update requests. Populated after GET when the API returns a bank account link.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId
    {
        get => _bankAccountId.Get(BankAccountLink?.Id);
        set => _bankAccountId.Set(value);
    }

    /// <summary>
    /// Whether to omit the logo and company address on the credit note.
    /// </summary>
    [JsonPropertyName("omit_header")]
    public bool? OmitHeader { get; set; }

    /// <summary>
    /// Whether to display the project name in the Other Information section.
    /// Writable on create and on draft updates; omitted once the credit note leaves draft.
    /// </summary>
    [JsonPropertyName("show_project_name")]
    public bool? ShowProjectName { get; set; }

    /// <summary>
    /// Credit note VAT status for reporting purposes.
    /// </summary>
    [JsonPropertyName("ec_status")]
    public InvoiceEcStatus? EcStatus { get; set; }

    /// <summary>
    /// Place of supply when <see cref="EcStatus"/> is EC VAT MOSS.
    /// </summary>
    [JsonPropertyName("place_of_supply")]
    public string? PlaceOfSupply { get; set; }

    /// <summary>
    /// Net value.
    /// </summary>
    [JsonPropertyName("net_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? NetValue { get; set; }

    /// <summary>
    /// Exchange rate into the company's native currency.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Whether sales tax applies to the credit note.
    /// </summary>
    [JsonPropertyName("involves_sales_tax")]
    public bool? InvolvesSalesTax { get; set; }

    /// <summary>
    /// Total value of sales tax.
    /// </summary>
    [JsonPropertyName("sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxValue { get; set; }

    /// <summary>
    /// Total value of second sales tax for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxValue { get; set; }

    /// <summary>
    /// Gross value.
    /// </summary>
    [JsonPropertyName("total_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalValue { get; set; }

    /// <summary>
    /// Amount refunded so far.
    /// </summary>
    [JsonPropertyName("refunded_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RefundedValue { get; set; }

    /// <summary>
    /// Amount yet to be refunded.
    /// </summary>
    [JsonPropertyName("due_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DueValue { get; set; }

    /// <summary>
    /// Whether VAT status was registration applied for at credit note date.
    /// </summary>
    [JsonPropertyName("is_interim_uk_vat")]
    public bool? IsInterimUkVat { get; set; }

    /// <summary>
    /// Date when the credit note was fully refunded.
    /// </summary>
    [JsonPropertyName("refunded_on")]
    public DateOnly? RefundedOn { get; set; }

    /// <summary>
    /// Date when the credit note was written off.
    /// </summary>
    [JsonPropertyName("written_off_date")]
    public DateOnly? WrittenOffDate { get; set; }

    /// <summary>
    /// Credit note line items.
    /// </summary>
    [JsonPropertyName("credit_note_items")]
    public List<CreditNoteItem>? CreditNoteItems { get; set; }

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

    internal SettableLinkId ContactIdBacking => _contactId;

    internal long? ContactLinkId => ContactLink?.Id;

    internal SettableLinkId ProjectIdBacking => _projectId;

    internal long? ProjectLinkId => ProjectLink?.Id;

    internal SettableLinkId BankAccountIdBacking => _bankAccountId;

    internal long? BankAccountLinkId => BankAccountLink?.Id;

    /// <summary>
    /// Attaches a hydrated contact to this credit note.
    /// </summary>
    /// <param name="contact">Contact details.</param>
    internal void AttachContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        ContactLink = new ExpandableField<Contact>(contact.Url, contact);
    }

    /// <summary>
    /// Attaches a hydrated project to this credit note.
    /// </summary>
    /// <param name="project">Project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }
}
