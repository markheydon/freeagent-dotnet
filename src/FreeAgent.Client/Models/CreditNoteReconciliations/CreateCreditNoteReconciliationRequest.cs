using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Request attributes for creating a credit note reconciliation.
/// </summary>
public sealed class CreateCreditNoteReconciliationRequest
{
    internal decimal GrossValue { get; private init; }

    internal InvoiceReference Invoice { get; private init; }

    internal CreditNoteReference CreditNote { get; private init; }

    internal DateOnly? DatedOn { get; private init; }

    internal decimal? ExchangeRate { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    /// <summary>
    /// Creates a request to reconcile a credit note against an invoice.
    /// </summary>
    /// <param name="grossValue">Amount reconciled between the credit note and invoice.</param>
    /// <param name="invoice">Invoice being reconciled.</param>
    /// <param name="creditNote">Credit note being reconciled.</param>
    /// <param name="datedOn">Optional date the reconciliation takes effect.</param>
    /// <param name="exchangeRate">Optional exchange rate into the company's native currency.</param>
    /// <param name="currency">Optional reconciliation currency code.</param>
    /// <returns>A create-credit-note-reconciliation request.</returns>
    public static CreateCreditNoteReconciliationRequest Create(
        decimal grossValue,
        InvoiceReference invoice,
        CreditNoteReference creditNote,
        DateOnly? datedOn = null,
        decimal? exchangeRate = null,
        CurrencyCode? currency = null)
    {
        if (invoice == default)
        {
            throw new ArgumentException("Invoice reference is required.", nameof(invoice));
        }

        if (creditNote == default)
        {
            throw new ArgumentException("Credit note reference is required.", nameof(creditNote));
        }

        return new CreateCreditNoteReconciliationRequest
        {
            GrossValue = grossValue,
            Invoice = invoice,
            CreditNote = creditNote,
            DatedOn = datedOn,
            ExchangeRate = exchangeRate,
            Currency = currency
        };
    }
}
