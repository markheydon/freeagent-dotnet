using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Represents a FreeAgent credit note reconciliation between an invoice and a credit note.
/// </summary>
public sealed class CreditNoteReconciliation : IFreeAgentResource
{
    /// <summary>
    /// Credit note reconciliation resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Amount reconciled between the credit note and invoice.
    /// </summary>
    [JsonPropertyName("gross_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? GrossValue { get; set; }

    /// <summary>
    /// Date the reconciliation takes effect.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Exchange rate at which the invoice amount is converted into the company's native currency.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Credit note reconciliation currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Wire representation of the invoice link.
    /// </summary>
    [JsonPropertyName("invoice")]
    [JsonInclude]
    internal ExpandableField<Invoice>? InvoiceLink { get; set; }

    /// <summary>
    /// Invoice when returned nested on the wire or hydrated via <see cref="CreditNoteReconciliationGetOptions.IncludeInvoice"/>.
    /// </summary>
    [JsonIgnore]
    public Invoice? Invoice => InvoiceLink?.Value;

    /// <summary>
    /// Invoice identifier parsed from the reconciliation response.
    /// </summary>
    [JsonIgnore]
    public long? InvoiceId => InvoiceLink?.Id;

    /// <summary>
    /// Wire representation of the credit note link.
    /// </summary>
    [JsonPropertyName("credit_note")]
    [JsonInclude]
    internal ExpandableField<CreditNote>? CreditNoteLink { get; set; }

    /// <summary>
    /// Credit note when returned nested on the wire or hydrated via <see cref="CreditNoteReconciliationGetOptions.IncludeCreditNote"/>.
    /// </summary>
    [JsonIgnore]
    public CreditNote? CreditNote => CreditNoteLink?.Value;

    /// <summary>
    /// Credit note identifier parsed from the reconciliation response.
    /// </summary>
    [JsonIgnore]
    public long? CreditNoteId => CreditNoteLink?.Id;

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
    /// Attaches a hydrated invoice to this reconciliation.
    /// </summary>
    /// <param name="invoice">Invoice details.</param>
    internal void AttachInvoice(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        InvoiceLink = new ExpandableField<Invoice>(invoice.Url, invoice);
    }

    /// <summary>
    /// Attaches a hydrated credit note to this reconciliation.
    /// </summary>
    /// <param name="creditNote">Credit note details.</param>
    internal void AttachCreditNote(CreditNote creditNote)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        CreditNoteLink = new ExpandableField<CreditNote>(creditNote.Url, creditNote);
    }
}
