using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
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

    public static CreditNoteWritePayload FromCreditNote(
        CreditNote creditNote,
        FreeAgentEnvironment environment,
        bool omitLineItems = false)
    {
        ArgumentNullException.ThrowIfNull(creditNote);

        List<CreditNoteItemWritePayload>? items = null;
        if (!omitLineItems && creditNote.CreditNoteItems is not null)
        {
            items = creditNote.CreditNoteItems.ConvertAll(i => CreditNoteItemWritePayload.FromCreditNoteItem(i, environment));
        }

        return new CreditNoteWritePayload
        {
            Contact = LinkedResourceWriteMapper.ToContactReference(environment, creditNote.ContactId),
            Project = LinkedResourceWriteMapper.ToProjectReference(environment, creditNote.ProjectId),
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
            BankAccount = LinkedResourceWriteMapper.ToBankAccountReference(environment, creditNote.BankAccountId),
            OmitHeader = creditNote.OmitHeader,
            ShowProjectName = creditNote.ShowProjectName,
            EcStatus = creditNote.EcStatus,
            PlaceOfSupply = creditNote.PlaceOfSupply,
            CreditNoteItems = items
        };
    }
}
