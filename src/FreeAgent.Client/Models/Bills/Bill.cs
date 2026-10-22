using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent bill.
/// </summary>
public sealed class Bill : IFreeAgentResource
{
    private SettableLinkId _contactId;
    private SettableLinkId _projectId;
    private SettableLinkId _rebillToProjectId;
    private SettableLinkId _propertyId;

    /// <summary>
    /// Bill resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Bill payment status.
    /// </summary>
    [JsonPropertyName("status")]
    public BillStatus? Status { get; set; }

    /// <summary>
    /// Bill payment status with relative due date text.
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
    /// Contact when returned nested on the wire or hydrated via <see cref="BillGetOptions.IncludeContact"/>.
    /// </summary>
    /// <remarks>
    /// After a normal <c>GetBillAsync</c> this property is <see langword="null"/>; use <see cref="ContactId"/>
    /// instead, or pass <see cref="BillGetOptions.IncludeContact"/>.
    /// See the common pitfall section in <c>docs/explanation/linked-resources.md</c>.
    /// </remarks>
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
    /// Free-text bill reference.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Date of the bill.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Due date of the bill.
    /// </summary>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    /// <summary>
    /// Date of the most recent payment. Returned for fully paid bills and bill refunds.
    /// </summary>
    [JsonPropertyName("paid_on")]
    public DateOnly? PaidOn { get; set; }

    /// <summary>
    /// Bill currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Whether bill items are entered including sales tax on the web or mobile apps.
    /// </summary>
    [JsonPropertyName("input_total_values_inc_tax")]
    public bool? InputTotalValuesIncTax { get; set; }

    /// <summary>
    /// Total value of the bill.
    /// </summary>
    [JsonPropertyName("total_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalValue { get; set; }

    /// <summary>
    /// Due value of the bill.
    /// </summary>
    [JsonPropertyName("due_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DueValue { get; set; }

    /// <summary>
    /// Due value in the company's native currency.
    /// </summary>
    [JsonPropertyName("native_due_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? NativeDueValue { get; set; }

    /// <summary>
    /// Net value of the bill.
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
    /// Amount paid so far.
    /// </summary>
    [JsonPropertyName("paid_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? PaidValue { get; set; }

    /// <summary>
    /// Whether the bill will be paid using a hire purchase agreement.
    /// </summary>
    [JsonPropertyName("is_paid_by_hire_purchase")]
    public bool? IsPaidByHirePurchase { get; set; }

    /// <summary>
    /// Bill VAT status for reporting purposes.
    /// </summary>
    [JsonPropertyName("ec_status")]
    public BillEcStatus? EcStatus { get; set; }

    /// <summary>
    /// Free-text comments.
    /// </summary>
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }

    /// <summary>
    /// Wire representation of the project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Project when returned nested on the wire or hydrated via <see cref="BillGetOptions.IncludeProject"/>.
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
    /// Wire representation of the rebill target project link.
    /// </summary>
    [JsonPropertyName("rebill_to_project")]
    [JsonInclude]
    internal ExpandableField<Project>? RebillToProjectLink { get; set; }

    /// <summary>
    /// Rebill target project when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Project? RebillToProject => RebillToProjectLink?.Value;

    /// <summary>
    /// Rebill target project identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? RebillToProjectId
    {
        get => _rebillToProjectId.Get(RebillToProjectLink?.Id);
        set => _rebillToProjectId.Set(value);
    }

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
    /// Property identifier for create and update requests for landlord companies.
    /// </summary>
    [JsonIgnore]
    public long? PropertyId
    {
        get => _propertyId.Get(PropertyLink?.Id);
        set => _propertyId.Set(value);
    }

    /// <summary>
    /// Recurrence frequency for the bill.
    /// </summary>
    [JsonPropertyName("recurring")]
    public BillRecurringSchedule? Recurring { get; set; }

    /// <summary>
    /// Date when the bill should stop recurring.
    /// </summary>
    [JsonPropertyName("recurring_end_date")]
    public DateOnly? RecurringEndDate { get; set; }

    /// <summary>
    /// Attachment metadata returned on read responses.
    /// </summary>
    [JsonPropertyName("attachment")]
    public BillAttachment? Attachment { get; set; }

    /// <summary>
    /// Attachment payload for create and update requests. Not populated from API read responses.
    /// </summary>
    [JsonIgnore]
    public BillAttachmentWritePayload? AttachmentWrite { get; set; }

    /// <summary>
    /// CIS deduction band for the bill.
    /// </summary>
    [JsonPropertyName("cis_deduction_band")]
    public CisDeductionRate? CisDeductionBand { get; set; }

    /// <summary>
    /// CIS deduction rate percentage.
    /// </summary>
    [JsonPropertyName("cis_deduction_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeductionRate { get; set; }

    /// <summary>
    /// Total CIS deduction for this bill.
    /// </summary>
    [JsonPropertyName("cis_deduction")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeduction { get; set; }

    /// <summary>
    /// CIS deduction already suffered on payments for this bill.
    /// </summary>
    [JsonPropertyName("cis_deduction_suffered")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeductionSuffered { get; set; }

    /// <summary>
    /// Wire representation of the rebilled invoice link.
    /// </summary>
    [JsonPropertyName("rebilled_on_invoice_item")]
    [JsonInclude]
    internal ExpandableField<Invoice>? RebilledOnInvoiceLink { get; set; }

    /// <summary>
    /// Invoice when returned nested on the wire or hydrated via <see cref="BillGetOptions.IncludeRebilledOnInvoice"/>.
    /// </summary>
    [JsonIgnore]
    public Invoice? RebilledOnInvoice => RebilledOnInvoiceLink?.Value;

    /// <summary>
    /// Invoice identifier when <c>rebilled_on_invoice_item</c> links to an invoice. Read-only on the wire.
    /// </summary>
    [JsonIgnore]
    public long? RebilledOnInvoiceId => GetRebilledLinkId("invoices");

    /// <summary>
    /// Invoice line item identifier when <c>rebilled_on_invoice_item</c> links to an invoice item. Read-only on the wire.
    /// </summary>
    [JsonIgnore]
    public long? RebilledOnInvoiceItemId => GetRebilledLinkId("invoice_items");

    /// <summary>
    /// Bill line items.
    /// </summary>
    [JsonPropertyName("bill_items")]
    public List<BillItem>? BillItems { get; set; }

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

    internal SettableLinkId RebillToProjectIdBacking => _rebillToProjectId;

    internal long? RebillToProjectLinkId => RebillToProjectLink?.Id;

    internal SettableLinkId PropertyIdBacking => _propertyId;

    internal long? PropertyLinkId => PropertyLink?.Id;

    /// <summary>
    /// Attaches a hydrated contact to this bill.
    /// </summary>
    /// <param name="contact">Contact details.</param>
    internal void AttachContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        ContactLink = new ExpandableField<Contact>(contact.Url, contact);
    }

    /// <summary>
    /// Attaches a hydrated project to this bill.
    /// </summary>
    /// <param name="project">Project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    /// <summary>
    /// Attaches a hydrated rebill target project to this bill.
    /// </summary>
    /// <param name="project">Project details.</param>
    internal void AttachRebillToProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        RebillToProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    /// <summary>
    /// Attaches a hydrated invoice to this bill.
    /// </summary>
    /// <param name="invoice">Invoice details.</param>
    internal void AttachRebilledOnInvoice(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        RebilledOnInvoiceLink = new ExpandableField<Invoice>(invoice.Url, invoice);
    }

    private long? GetRebilledLinkId(string resourceSegment)
    {
        if (RebilledOnInvoiceLink?.Uri is not string uri)
        {
            return null;
        }

        if (!uri.Contains($"/{resourceSegment}/", StringComparison.OrdinalIgnoreCase)
            && !uri.EndsWith($"/{resourceSegment}", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return RebilledOnInvoiceLink.Id;
    }
}
