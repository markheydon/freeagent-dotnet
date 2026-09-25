using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Request attributes for updating a credit note reconciliation.
/// </summary>
public sealed class UpdateCreditNoteReconciliationRequest
{
    internal decimal? GrossValue { get; private init; }

    internal long? InvoiceId { get; private init; }

    internal long? CreditNoteId { get; private init; }

    internal DateOnly? DatedOn { get; private init; }

    internal decimal? ExchangeRate { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    /// <summary>
    /// Creates a request to update credit note reconciliation attributes.
    /// </summary>
    /// <param name="grossValue">Updated amount reconciled between the credit note and invoice.</param>
    /// <param name="invoiceId">Updated invoice identifier.</param>
    /// <param name="creditNoteId">Updated credit note identifier.</param>
    /// <param name="datedOn">Updated reconciliation date.</param>
    /// <param name="exchangeRate">Updated exchange rate.</param>
    /// <param name="currency">Updated currency code.</param>
    /// <returns>An update-credit-note-reconciliation request.</returns>
    public static UpdateCreditNoteReconciliationRequest Create(
        decimal? grossValue = null,
        long? invoiceId = null,
        long? creditNoteId = null,
        DateOnly? datedOn = null,
        decimal? exchangeRate = null,
        CurrencyCode? currency = null)
    {
        if (invoiceId is long invoice && invoice <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(invoiceId));
        }

        if (creditNoteId is long creditNote && creditNote <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(creditNoteId));
        }

        return new UpdateCreditNoteReconciliationRequest
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
