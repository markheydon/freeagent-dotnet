using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Request attributes for creating a credit note reconciliation.
/// </summary>
public sealed class CreateCreditNoteReconciliationRequest
{
    internal decimal GrossValue { get; private init; }

    internal long InvoiceId { get; private init; }

    internal long CreditNoteId { get; private init; }

    internal DateOnly? DatedOn { get; private init; }

    internal decimal? ExchangeRate { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    /// <summary>
    /// Creates a request to reconcile a credit note against an invoice.
    /// </summary>
    /// <param name="grossValue">Amount reconciled between the credit note and invoice.</param>
    /// <param name="invoiceId">Invoice being reconciled.</param>
    /// <param name="creditNoteId">Credit note being reconciled.</param>
    /// <param name="datedOn">Optional date the reconciliation takes effect.</param>
    /// <param name="exchangeRate">Optional exchange rate into the company's native currency.</param>
    /// <param name="currency">Optional reconciliation currency code.</param>
    /// <returns>A create-credit-note-reconciliation request.</returns>
    public static CreateCreditNoteReconciliationRequest Create(
        decimal grossValue,
        long invoiceId,
        long creditNoteId,
        DateOnly? datedOn = null,
        decimal? exchangeRate = null,
        CurrencyCode? currency = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);

        return new CreateCreditNoteReconciliationRequest
        {
            GrossValue = grossValue,
            InvoiceId = invoiceId,
            CreditNoteId = creditNoteId,
            DatedOn = datedOn,
            ExchangeRate = exchangeRate,
            Currency = currency
        };
    }
}
