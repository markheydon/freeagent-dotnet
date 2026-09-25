using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Writable credit note attributes for create and update requests.
/// </summary>
internal sealed class CreditNoteWritePayload
{
    [JsonPropertyName("contact")]
    public ContactReference? Contact { get; set; }

    [JsonPropertyName("project")]
    public ProjectReference? Project { get; set; }

    [JsonPropertyName("property")]
    public string? Property { get; set; }

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

    [JsonPropertyName("ec_status")]
    public InvoiceEcStatus? EcStatus { get; set; }

    [JsonPropertyName("place_of_supply")]
    public string? PlaceOfSupply { get; set; }

    [JsonPropertyName("credit_note_items")]
    public List<CreditNoteItemWritePayload>? CreditNoteItems { get; set; }

    public static CreditNoteWritePayload FromCreditNote(CreditNote creditNote)
    {
        ArgumentNullException.ThrowIfNull(creditNote);

        ContactReference? contact = null;
        if (!creditNote.OmitBillingContactFromWrite)
        {
            contact = creditNote.BillingContact
                ?? (creditNote.ContactLink?.Uri is string contactUri ? ContactReference.Parse(contactUri) : null);
        }
        else
        {
            contact = creditNote.BillingContact;
        }

        ProjectReference? project = null;
        if (!creditNote.OmitProjectFromWrite)
        {
            project = creditNote.LinkedProject
                ?? (creditNote.ProjectLink?.Uri is string projectUri ? ProjectReference.Parse(projectUri) : null);
        }
        else
        {
            project = creditNote.LinkedProject;
        }

        BankAccountReference? bankAccount = null;
        if (!creditNote.OmitBankAccountFromWrite)
        {
            bankAccount = creditNote.RemittanceBankAccount;
            if (bankAccount is null && creditNote.BankAccountLink?.Uri is string bankAccountUri)
            {
                bankAccount = BankAccountReference.Parse(bankAccountUri);
            }
        }
        else
        {
            bankAccount = creditNote.RemittanceBankAccount;
        }

        List<CreditNoteItemWritePayload>? items = null;
        if (!creditNote.OmitCreditNoteItemsFromWrite && creditNote.CreditNoteItems is not null)
        {
            items = creditNote.CreditNoteItems.ConvertAll(CreditNoteItemWritePayload.FromCreditNoteItem);
        }

        return new CreditNoteWritePayload
        {
            Contact = contact,
            Project = project,
            Property = creditNote.PropertyUri,
            Reference = creditNote.Reference,
            DatedOn = creditNote.DatedOn,
            DueOn = creditNote.DueOn,
            PaymentTermsInDays = creditNote.PaymentTermsInDays,
            Currency = creditNote.Currency,
            CisRate = creditNote.CisRate,
            CisDeductionRate = creditNote.CisDeductionRate,
            Comments = creditNote.Comments,
            DiscountPercent = creditNote.DiscountPercent,
            ClientContactName = creditNote.ClientContactName,
            PaymentTerms = creditNote.PaymentTerms,
            PoReference = creditNote.PoReference,
            BankAccount = bankAccount,
            OmitHeader = creditNote.OmitHeader,
            ShowProjectName = creditNote.ShowProjectName,
            EcStatus = creditNote.EcStatus,
            PlaceOfSupply = creditNote.PlaceOfSupply,
            CreditNoteItems = items
        };
    }
}
