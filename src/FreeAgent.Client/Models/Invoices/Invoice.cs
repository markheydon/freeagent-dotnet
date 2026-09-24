using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.RecurringInvoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Represents a FreeAgent invoice.
/// </summary>
public sealed class Invoice : IFreeAgentResource
{
    /// <summary>
    /// Invoice resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Invoice status.
    /// </summary>
    [JsonPropertyName("status")]
    public InvoiceStatus? Status { get; set; }

    /// <summary>
    /// Invoice status with relative due date text.
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
    /// Contact when returned nested on the wire or hydrated via <see cref="InvoiceGetOptions.IncludeContact"/>.
    /// </summary>
    [JsonIgnore]
    public Contact? Contact => ContactLink?.Value;

    /// <summary>
    /// Contact identifier parsed from the invoice response.
    /// </summary>
    [JsonIgnore]
    public long? ContactId => ContactLink?.Id;

    /// <summary>
    /// Contact to assign on create or update requests.
    /// </summary>
    [JsonIgnore]
    public ContactReference? BillingContact { get; set; }

    /// <summary>
    /// When <see langword="true"/>, create and update payloads exclude the contact even when a contact link exists on the model.
    /// </summary>
    [JsonIgnore]
    public bool OmitBillingContactFromWrite { get; set; }

    /// <summary>
    /// Display name of the contact for this invoice.
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
    /// Project when returned nested on the wire or hydrated via <see cref="InvoiceGetOptions.IncludeProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? Project => ProjectLink?.Value;

    /// <summary>
    /// Project identifier parsed from the invoice response.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId => ProjectLink?.Id;

    /// <summary>
    /// Project to assign on create or update requests.
    /// </summary>
    [JsonIgnore]
    public ProjectReference? LinkedProject { get; set; }

    /// <summary>
    /// When <see langword="true"/>, create and update payloads exclude the project even when a project link exists on the model.
    /// </summary>
    [JsonIgnore]
    public bool OmitProjectFromWrite { get; set; }

    /// <summary>
    /// When <see langword="true"/>, create and update payloads exclude the bank account even when a bank account link exists on the model.
    /// </summary>
    [JsonIgnore]
    public bool OmitBankAccountFromWrite { get; set; }

    /// <summary>
    /// When <see langword="true"/>, create and update payloads exclude invoice line items even when items exist on the model.
    /// </summary>
    [JsonIgnore]
    public bool OmitInvoiceItemsFromWrite { get; set; }

    /// <summary>
    /// Property URI for landlord companies.
    /// </summary>
    /// <remarks>
    /// Full property operations are not yet implemented in the SDK.
    /// </remarks>
    [JsonPropertyName("property")]
    public string? PropertyUri { get; set; }

    /// <summary>
    /// Timeslip grouping option.
    /// </summary>
    [JsonPropertyName("include_timeslips")]
    public InvoiceIncludeTimeslips? IncludeTimeslips { get; set; }

    /// <summary>
    /// Expense grouping option.
    /// </summary>
    [JsonPropertyName("include_expenses")]
    public InvoiceIncludeExpenses? IncludeExpenses { get; set; }

    /// <summary>
    /// Estimate grouping option.
    /// </summary>
    [JsonPropertyName("include_estimates")]
    public InvoiceIncludeEstimates? IncludeEstimates { get; set; }

    /// <summary>
    /// Invoice reference.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Date of the invoice.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Invoice due date.
    /// </summary>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    /// <summary>
    /// Payment terms in days. Set to zero for due on receipt.
    /// </summary>
    [JsonPropertyName("payment_terms_in_days")]
    public int? PaymentTermsInDays { get; set; }

    /// <summary>
    /// Invoice currency code.
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
    /// Total CIS deduction for this invoice.
    /// </summary>
    [JsonPropertyName("cis_deduction")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeduction { get; set; }

    /// <summary>
    /// CIS deduction already paid for this invoice.
    /// </summary>
    [JsonPropertyName("cis_deduction_suffered")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CisDeductionSuffered { get; set; }

    /// <summary>
    /// Additional text added to the bottom of the invoice.
    /// </summary>
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }

    /// <summary>
    /// Whether to email this invoice automatically using the default template.
    /// </summary>
    [JsonPropertyName("send_new_invoice_emails")]
    public bool? SendNewInvoiceEmails { get; set; }

    /// <summary>
    /// Whether to email payment reminders if the invoice goes unpaid.
    /// </summary>
    [JsonPropertyName("send_reminder_emails")]
    public bool? SendReminderEmails { get; set; }

    /// <summary>
    /// Whether to email a thank you once the invoice has been paid.
    /// </summary>
    [JsonPropertyName("send_thank_you_emails")]
    public bool? SendThankYouEmails { get; set; }

    /// <summary>
    /// Discount applied across the whole invoice.
    /// </summary>
    [JsonPropertyName("discount_percent")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DiscountPercent { get; set; }

    /// <summary>
    /// Client contact name override for this invoice.
    /// </summary>
    [JsonPropertyName("client_contact_name")]
    public string? ClientContactName { get; set; }

    /// <summary>
    /// Payment terms override for this invoice.
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
    /// Bank account identifier parsed from the invoice response.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId => BankAccountLink?.Id;

    /// <summary>
    /// Bank account to assign on create or update requests.
    /// </summary>
    [JsonIgnore]
    public BankAccountReference? RemittanceBankAccount { get; set; }

    /// <summary>
    /// Whether to omit the logo and company address on the invoice.
    /// </summary>
    [JsonPropertyName("omit_header")]
    public bool? OmitHeader { get; set; }

    /// <summary>
    /// Whether to display the project name in the Other Information section.
    /// </summary>
    [JsonPropertyName("show_project_name")]
    public bool? ShowProjectName { get; set; }

    /// <summary>
    /// Whether to always display BIC and IBAN numbers on the invoice.
    /// </summary>
    [JsonPropertyName("always_show_bic_and_iban")]
    public bool? AlwaysShowBicAndIban { get; set; }

    /// <summary>
    /// Invoice VAT status for reporting purposes.
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
    /// Whether sales tax applies to the invoice.
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
    /// Amount paid so far.
    /// </summary>
    [JsonPropertyName("paid_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? PaidValue { get; set; }

    /// <summary>
    /// Amount yet to be paid.
    /// </summary>
    [JsonPropertyName("due_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DueValue { get; set; }

    /// <summary>
    /// Whether VAT status was registration applied for at invoice date.
    /// </summary>
    [JsonPropertyName("is_interim_uk_vat")]
    public bool? IsInterimUkVat { get; set; }

    /// <summary>
    /// Date when the invoice was fully paid.
    /// </summary>
    [JsonPropertyName("paid_on")]
    public DateOnly? PaidOn { get; set; }

    /// <summary>
    /// Date when the invoice was written off.
    /// </summary>
    [JsonPropertyName("written_off_date")]
    public DateOnly? WrittenOffDate { get; set; }

    /// <summary>
    /// Wire representation of the recurring invoice link.
    /// </summary>
    [JsonPropertyName("recurring_invoice")]
    [JsonInclude]
    internal ExpandableField<RecurringInvoice>? RecurringInvoiceLink { get; set; }

    /// <summary>
    /// Recurring invoice when returned nested on the wire. Read-only when generated.
    /// </summary>
    [JsonIgnore]
    public RecurringInvoice? RecurringInvoice => RecurringInvoiceLink?.Value;

    /// <summary>
    /// Recurring invoice identifier parsed from the invoice response.
    /// </summary>
    [JsonIgnore]
    public long? RecurringInvoiceId => RecurringInvoiceLink?.Id;

    /// <summary>
    /// Online payment URL when enabled.
    /// </summary>
    [JsonPropertyName("payment_url")]
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// Online payment method flags.
    /// </summary>
    [JsonPropertyName("payment_methods")]
    public InvoicePaymentMethods? PaymentMethods { get; set; }

    /// <summary>
    /// Invoice line items.
    /// </summary>
    [JsonPropertyName("invoice_items")]
    public List<InvoiceItem>? InvoiceItems { get; set; }

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

    /// <summary>
    /// Attaches a hydrated contact to this invoice.
    /// </summary>
    /// <param name="contact">Contact details.</param>
    internal void AttachContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        ContactLink = new ExpandableField<Contact>(contact.Url, contact);
    }

    /// <summary>
    /// Attaches a hydrated project to this invoice.
    /// </summary>
    /// <param name="project">Project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }
}
