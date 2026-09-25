using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Writable credit note reconciliation attributes for create and update requests.
/// </summary>
internal sealed class CreditNoteReconciliationWritePayload
{
    [JsonPropertyName("gross_value")]
    public decimal? GrossValue { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("exchange_rate")]
    public decimal? ExchangeRate { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("invoice")]
    public InvoiceReference? Invoice { get; set; }

    [JsonPropertyName("credit_note")]
    public CreditNoteReference? CreditNote { get; set; }

    public static CreditNoteReconciliationWritePayload FromCreate(CreateCreditNoteReconciliationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreditNoteReconciliationWritePayload
        {
            GrossValue = request.GrossValue,
            DatedOn = request.DatedOn,
            ExchangeRate = request.ExchangeRate,
            Currency = request.Currency,
            Invoice = request.Invoice,
            CreditNote = request.CreditNote
        };
    }

    public static CreditNoteReconciliationWritePayload FromUpdate(UpdateCreditNoteReconciliationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreditNoteReconciliationWritePayload
        {
            GrossValue = request.GrossValue,
            DatedOn = request.DatedOn,
            ExchangeRate = request.ExchangeRate,
            Currency = request.Currency,
            Invoice = request.Invoice,
            CreditNote = request.CreditNote
        };
    }
}
