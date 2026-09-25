using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Request attributes for updating a credit note reconciliation.
/// </summary>
public sealed class UpdateCreditNoteReconciliationRequest
{
    internal decimal? GrossValue { get; private init; }

    internal InvoiceReference? Invoice { get; private init; }

    internal CreditNoteReference? CreditNote { get; private init; }

    internal DateOnly? DatedOn { get; private init; }

    internal decimal? ExchangeRate { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    /// <summary>
    /// Creates a request to update credit note reconciliation attributes.
    /// </summary>
    /// <param name="grossValue">Updated amount reconciled between the credit note and invoice.</param>
    /// <param name="invoice">Updated invoice reference.</param>
    /// <param name="creditNote">Updated credit note reference.</param>
    /// <param name="datedOn">Updated reconciliation date.</param>
    /// <param name="exchangeRate">Updated exchange rate.</param>
    /// <param name="currency">Updated currency code.</param>
    /// <returns>An update-credit-note-reconciliation request.</returns>
    public static UpdateCreditNoteReconciliationRequest Create(
        decimal? grossValue = null,
        InvoiceReference? invoice = null,
        CreditNoteReference? creditNote = null,
        DateOnly? datedOn = null,
        decimal? exchangeRate = null,
        CurrencyCode? currency = null)
    {
        return new UpdateCreditNoteReconciliationRequest
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
