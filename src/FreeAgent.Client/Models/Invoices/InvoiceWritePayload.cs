using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Writable invoice attributes for create and update requests.
/// </summary>
internal sealed class InvoiceWritePayload
{
    [JsonPropertyName("contact")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ContactReference>))]
    public WriteLink<ContactReference>? Contact { get; set; }

    [JsonPropertyName("project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? Project { get; set; }

    [JsonPropertyName("property")]
    public string? Property { get; set; }

    [JsonPropertyName("include_timeslips")]
    public InvoiceIncludeTimeslips? IncludeTimeslips { get; set; }

    [JsonPropertyName("include_expenses")]
    public InvoiceIncludeExpenses? IncludeExpenses { get; set; }

    [JsonPropertyName("include_estimates")]
    public InvoiceIncludeEstimates? IncludeEstimates { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    [JsonPropertyName("payment_terms_in_days")]
    public int? PaymentTermsInDays { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("cis_rate")]
    public string? CisRate { get; set; }

    [JsonPropertyName("cis_deduction_rate")]
    public decimal? CisDeductionRate { get; set; }

    [JsonPropertyName("comments")]
    public string? Comments { get; set; }

    [JsonPropertyName("send_new_invoice_emails")]
    public bool? SendNewInvoiceEmails { get; set; }

    [JsonPropertyName("send_reminder_emails")]
    public bool? SendReminderEmails { get; set; }

    [JsonPropertyName("send_thank_you_emails")]
    public bool? SendThankYouEmails { get; set; }

    [JsonPropertyName("discount_percent")]
    public decimal? DiscountPercent { get; set; }

    [JsonPropertyName("client_contact_name")]
    public string? ClientContactName { get; set; }

    [JsonPropertyName("payment_terms")]
    public string? PaymentTerms { get; set; }

    [JsonPropertyName("po_reference")]
    public string? PoReference { get; set; }

    [JsonPropertyName("bank_account")]
    [JsonConverter(typeof(WriteLinkJsonConverter<BankAccountReference>))]
    public WriteLink<BankAccountReference>? BankAccount { get; set; }

    [JsonPropertyName("omit_header")]
    public bool? OmitHeader { get; set; }

    [JsonPropertyName("show_project_name")]
    public bool? ShowProjectName { get; set; }

    [JsonPropertyName("always_show_bic_and_iban")]
    public bool? AlwaysShowBicAndIban { get; set; }

    [JsonPropertyName("ec_status")]
    public InvoiceEcStatus? EcStatus { get; set; }

    [JsonPropertyName("place_of_supply")]
    public string? PlaceOfSupply { get; set; }

    [JsonPropertyName("payment_methods")]
    public InvoicePaymentMethods? PaymentMethods { get; set; }

    [JsonPropertyName("invoice_items")]
    public List<InvoiceItemWritePayload>? InvoiceItems { get; set; }

    public static InvoiceWritePayload FromInvoice(
        Invoice invoice,
        FreeAgentEnvironment environment,
        bool omitLineItems = false,
        LinkedResourceWriteOptions linkOptions = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        List<InvoiceItemWritePayload>? items = null;
        if (!omitLineItems && invoice.InvoiceItems is not null)
        {
            items = invoice.InvoiceItems.ConvertAll(i => InvoiceItemWritePayload.FromInvoiceItem(i, environment));
        }

        return new InvoiceWritePayload
        {
            Contact = LinkedResourceWriteMapper.ResolveContactReference(
                environment,
                invoice.ContactIdBacking,
                invoice.ContactLinkId,
                linkOptions.OmitContact),
            Project = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                invoice.ProjectIdBacking,
                invoice.ProjectLinkId,
                linkOptions.OmitProject),
            Property = invoice.PropertyUri,
            IncludeTimeslips = invoice.IncludeTimeslips,
            IncludeExpenses = invoice.IncludeExpenses,
            IncludeEstimates = invoice.IncludeEstimates,
            Reference = invoice.Reference,
            DatedOn = invoice.DatedOn,
            DueOn = invoice.DueOn,
            PaymentTermsInDays = invoice.PaymentTermsInDays,
            Currency = invoice.Currency,
            CisRate = invoice.CisRate,
            CisDeductionRate = invoice.CisDeductionRate,
            Comments = invoice.Comments,
            SendNewInvoiceEmails = invoice.SendNewInvoiceEmails,
            SendReminderEmails = invoice.SendReminderEmails,
            SendThankYouEmails = invoice.SendThankYouEmails,
            DiscountPercent = invoice.DiscountPercent,
            ClientContactName = invoice.ClientContactName,
            PaymentTerms = invoice.PaymentTerms,
            PoReference = invoice.PoReference,
            BankAccount = LinkedResourceWriteMapper.ResolveBankAccountReference(
                environment,
                invoice.BankAccountIdBacking,
                invoice.BankAccountLinkId,
                linkOptions.OmitBankAccount),
            OmitHeader = invoice.OmitHeader,
            ShowProjectName = invoice.ShowProjectName,
            AlwaysShowBicAndIban = invoice.AlwaysShowBicAndIban,
            EcStatus = invoice.EcStatus,
            PlaceOfSupply = invoice.PlaceOfSupply,
            PaymentMethods = invoice.PaymentMethods,
            InvoiceItems = items
        };
    }
}
