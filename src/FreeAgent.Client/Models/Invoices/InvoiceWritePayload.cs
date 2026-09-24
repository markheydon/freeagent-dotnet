using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Writable invoice attributes for create and update requests.
/// </summary>
internal sealed class InvoiceWritePayload
{
    [JsonPropertyName("contact")]
    public ContactReference? Contact { get; set; }

    [JsonPropertyName("project")]
    public ProjectReference? Project { get; set; }

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
    public BankAccountReference? BankAccount { get; set; }

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

    public static InvoiceWritePayload FromInvoice(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        ContactReference? contact = null;
        if (!invoice.OmitBillingContactFromWrite)
        {
            contact = invoice.BillingContact
                ?? (invoice.ContactLink?.Uri is string contactUri ? ContactReference.Parse(contactUri) : null);
        }
        else
        {
            contact = invoice.BillingContact;
        }

        ProjectReference? project = null;
        if (!invoice.OmitProjectFromWrite)
        {
            project = invoice.LinkedProject
                ?? (invoice.ProjectLink?.Uri is string projectUri ? ProjectReference.Parse(projectUri) : null);
        }
        else
        {
            project = invoice.LinkedProject;
        }

        BankAccountReference? bankAccount = null;
        if (!invoice.OmitBankAccountFromWrite)
        {
            bankAccount = invoice.RemittanceBankAccount;
            if (bankAccount is null && invoice.BankAccountLink?.Uri is string bankAccountUri)
            {
                bankAccount = BankAccountReference.Parse(bankAccountUri);
            }
        }
        else
        {
            bankAccount = invoice.RemittanceBankAccount;
        }

        List<InvoiceItemWritePayload>? items = null;
        if (!invoice.OmitInvoiceItemsFromWrite && invoice.InvoiceItems is not null)
        {
            items = invoice.InvoiceItems.ConvertAll(InvoiceItemWritePayload.FromInvoiceItem);
        }

        return new InvoiceWritePayload
        {
            Contact = contact,
            Project = project,
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
            BankAccount = bankAccount,
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
