using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Transactions;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Accounting transactions report examples.
/// </summary>
[ConsoleSamples(Category = "Transactions")]
internal sealed class TransactionSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List transactions")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTransactionsAsync(CancellationToken cancellationToken)
    {
        var transactions = await context.Client.Transactions.ListAsync(cancellationToken: cancellationToken);

        WriteSummary("Transactions", transactions);
    }

    [ConsoleSample(Name = "List transactions for a date range")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTransactionsForDateRangeAsync(CancellationToken cancellationToken)
    {
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var fromDate = toDate.AddMonths(-1);
        var transactions = await context.Client.Transactions.ListAsync(fromDate, toDate, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Transactions for the last month");
        SampleOutput.WriteField("From", fromDate);
        SampleOutput.WriteField("To", toDate);
        SampleOutput.WriteField("Count", transactions.Count);
        if (transactions.Count > 0)
        {
            SampleOutput.WriteField("First description", transactions[0].Description);
            SampleOutput.WriteField("First debit value", transactions[0].DebitValue);
        }
    }

    [ConsoleSample(Name = "Get transaction by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetTransactionAsync(CancellationToken cancellationToken)
    {
        var transactions = await context.Client.Transactions.ListAsync(cancellationToken: cancellationToken);
        if (transactions.Count == 0)
        {
            SampleOutput.WriteHeader("Transaction detail");
            SampleOutput.WriteField("Note", "No transactions available in the current accounting period.");
            return;
        }

        var transactionId = transactions[0].ResourceId;
        if (transactionId <= 0)
        {
            throw new InvalidOperationException("Listed transaction is missing a resource id.");
        }

        var detail = await context.Client.Transactions.GetAsync(transactionId, cancellationToken);

        SampleOutput.WriteHeader("Transaction detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Nominal code", detail.NominalCode);
        SampleOutput.WriteField("Category", detail.CategoryName);
        SampleOutput.WriteField("Debit value", detail.DebitValue);
        SampleOutput.WriteField("Source item URL", detail.SourceItemUrl);
    }

    private static void WriteSummary(string header, IReadOnlyList<AccountingTransaction> transactions)
    {
        SampleOutput.WriteHeader(header);
        SampleOutput.WriteField("Count", transactions.Count);
        if (transactions.Count > 0)
        {
            SampleOutput.WriteField("First dated on", transactions[0].DatedOn);
            SampleOutput.WriteField("First description", transactions[0].Description);
            SampleOutput.WriteField("First nominal code", transactions[0].NominalCode);
        }
    }
}
