using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Parses <see cref="AccountingTransaction.SourceItemUrl"/> into a resource kind and identifier.
/// </summary>
public static class AccountingTransactionSourceItem
{
    /// <summary>
    /// Attempts to parse a source item URL into a resource kind and numeric identifier.
    /// </summary>
    /// <param name="sourceItemUrl">Source item URL from the transactions report.</param>
    /// <param name="kind">Parsed resource kind.</param>
    /// <param name="id">Parsed resource identifier.</param>
    /// <returns><see langword="true"/> when the URL contains a recognised resource segment and numeric id.</returns>
    public static bool TryParse(string? sourceItemUrl, out AccountingTransactionSourceKind kind, out long id)
    {
        kind = AccountingTransactionSourceKind.Unknown;
        id = 0;

        if (!FreeAgentResourceId.TryParse(sourceItemUrl, out id))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(sourceItemUrl))
        {
            return false;
        }

        kind = ResolveKind(sourceItemUrl);
        return kind != AccountingTransactionSourceKind.Unknown;
    }

    private static AccountingTransactionSourceKind ResolveKind(string sourceItemUrl)
    {
        if (ContainsSegment(sourceItemUrl, "invoices"))
        {
            return AccountingTransactionSourceKind.Invoice;
        }

        if (ContainsSegment(sourceItemUrl, "bills"))
        {
            return AccountingTransactionSourceKind.Bill;
        }

        if (ContainsSegment(sourceItemUrl, "bank_transaction_explanations"))
        {
            return AccountingTransactionSourceKind.BankTransactionExplanation;
        }

        if (ContainsSegment(sourceItemUrl, "expenses"))
        {
            return AccountingTransactionSourceKind.Expense;
        }

        if (ContainsSegment(sourceItemUrl, "credit_notes"))
        {
            return AccountingTransactionSourceKind.CreditNote;
        }

        if (ContainsSegment(sourceItemUrl, "estimates"))
        {
            return AccountingTransactionSourceKind.Estimate;
        }

        if (ContainsSegment(sourceItemUrl, "journal_entries"))
        {
            return AccountingTransactionSourceKind.JournalEntry;
        }

        return AccountingTransactionSourceKind.Unknown;
    }

    private static bool ContainsSegment(string uri, string resourceSegment) =>
        uri.Contains($"/{resourceSegment}/", StringComparison.OrdinalIgnoreCase)
        || uri.EndsWith($"/{resourceSegment}", StringComparison.OrdinalIgnoreCase);
}
