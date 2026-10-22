namespace FreeAgent.Client;

/// <summary>
/// Identifies the FreeAgent resource type referenced by <see cref="AccountingTransaction.SourceItemUrl"/>.
/// </summary>
public enum AccountingTransactionSourceKind
{
    /// <summary>
    /// Source item URL is missing or does not match a known resource segment.
    /// </summary>
    Unknown,

    /// <summary>
    /// Invoice resource (<c>invoices</c>).
    /// </summary>
    Invoice,

    /// <summary>
    /// Bill resource (<c>bills</c>).
    /// </summary>
    Bill,

    /// <summary>
    /// Bank transaction explanation resource (<c>bank_transaction_explanations</c>).
    /// </summary>
    BankTransactionExplanation,

    /// <summary>
    /// Expense resource (<c>expenses</c>).
    /// </summary>
    Expense,

    /// <summary>
    /// Credit note resource (<c>credit_notes</c>).
    /// </summary>
    CreditNote,

    /// <summary>
    /// Estimate resource (<c>estimates</c>).
    /// </summary>
    Estimate,

    /// <summary>
    /// Journal entry resource (<c>journal_entries</c>).
    /// </summary>
    JournalEntry,
}
