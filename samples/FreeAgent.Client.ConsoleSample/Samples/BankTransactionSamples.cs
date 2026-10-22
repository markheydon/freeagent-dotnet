using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Samples.Shared.Probes;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Bank transactions endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Bank transactions")]
internal sealed class BankTransactionSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List bank transactions")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBankTransactionsAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is null)
        {
            return;
        }

        var page = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Bank transactions for account {bankAccountId} ({page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(
            page.Items,
            transaction => $"{transaction.ResourceId,8}  {transaction.DatedOn:yyyy-MM-dd}  {transaction.Amount,10}  {transaction.Description}");

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "Stream all bank transactions", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StreamAllBankTransactionsAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is null)
        {
            return;
        }

        var count = 0;
        await foreach (var transaction in context.Client.BankTransactions.ListAutoPagingAsync(
                           bankAccountId: bankAccountId,
                           cancellationToken: cancellationToken))
        {
            count++;
            if (count <= 5)
            {
                Console.WriteLine(
                    $"  {transaction.ResourceId,8}  {transaction.DatedOn:yyyy-MM-dd}  {transaction.Amount,10}  {transaction.Description}");
            }
        }

        SampleOutput.WriteHeader("Streamed bank transactions via ListAutoPagingAsync");
        Console.WriteLine($"  Total transactions streamed: {count}");
    }

    [ConsoleSample(Name = "List unexplained bank transactions")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListUnexplainedBankTransactionsAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is null)
        {
            return;
        }

        var page = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            view: BankTransactionView.Unexplained,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Unexplained bank transactions ({page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(
            page.Items,
            transaction => $"{transaction.ResourceId,8}  {transaction.UnexplainedAmount,10}  {transaction.Description}");
    }

    [ConsoleSample(Name = "Get bank transaction detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBankTransactionDetailAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is null)
        {
            return;
        }

        var page = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            perPage: 1,
            cancellationToken: cancellationToken);

        if (page.Items.Count == 0)
        {
            SampleOutput.WriteHeader("Bank transaction detail");
            SampleOutput.WriteField("Result", "No bank transactions returned.");
            return;
        }

        var detail = await context.Client.BankTransactions.GetBankTransactionAsync(
            page.Items[0].ResourceId,
            cancellationToken);

        SampleOutput.WriteHeader("Bank transaction detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Amount", detail.Amount);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Unexplained amount", detail.UnexplainedAmount);
        SampleOutput.WriteField("Manual", detail.IsManual);
        SampleOutput.WriteField("Explanations", detail.BankTransactionExplanations?.Count ?? 0);
    }

    [ConsoleSample(Name = "Upload statement line and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UploadStatementAndDeleteAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is not { } accountId)
        {
            return;
        }

        const decimal amount = -0.01m;
        var fitId = ProbeBankTransactionLifecycleSupport.CreateProbeFitId("sdk-console-line");
        await context.Client.BankTransactions.UploadBankStatementAsync(
            new UploadBankStatementRequest(
            [
                new BankStatementTransaction
                {
                    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                    Amount = amount,
                    Description = fitId,
                    FitId = fitId,
                    TransactionType = BankStatementTransactionType.Other
                }
            ]),
            bankAccountId: accountId,
            cancellationToken: cancellationToken);

        var (createdId, skipReason) = await ResolveUploadedProbeTransactionAsync(
            accountId,
            fitId,
            cancellationToken);

        SampleOutput.WriteHeader("Uploaded statement line");
        SampleOutput.WriteField("Fit id", fitId);
        SampleOutput.WriteField("Matched id", createdId);

        if (createdId is null)
        {
            SampleOutput.WriteField("Deleted", skipReason ?? "No matching transaction to delete.");
            return;
        }

        await ProbeBankTransactionLifecycleSupport.DeleteExplanationsAndBankTransactionAsync(
            context.Client,
            createdId.Value,
            cancellationToken);
        SampleOutput.WriteField("Deleted", createdId.Value);
    }

    [ConsoleSample(Name = "Upload statement file and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UploadStatementFileAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is not { } accountId)
        {
            return;
        }

        const decimal amount = -0.02m;
        var fitId = ProbeBankTransactionLifecycleSupport.CreateProbeFitId("sdk-console-file");
        var posted = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var ofx = $"""
            OFXHEADER:100
            DATA:OFXSGML
            VERSION:102
            SECURITY:NONE
            ENCODING:USASCII
            CHARSET:1252
            COMPRESSION:NONE
            OLDFILEUID:NONE
            NEWFILEUID:NONE

            <OFX>
            <BANKMSGSRSV1>
            <STMTTRNRS>
            <STMTRS>
            <BANKTRANLIST>
            <STMTTRN>
            <TRNTYPE>OTHER
            <DTPOSTED>{posted}
            <TRNAMT>{amount.ToString(CultureInfo.InvariantCulture)}
            <FITID>{fitId}
            <NAME>{fitId}
            </STMTTRN>
            </BANKTRANLIST>
            </STMTRS>
            </STMTTRNRS>
            </BANKMSGSRSV1>
            </OFX>
            """;

        await using var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(ofx));
        await context.Client.BankTransactions.UploadBankStatementFileAsync(
            new UploadBankStatementFileRequest(stream, "sdk-probe.ofx"),
            bankAccountId: accountId,
            cancellationToken: cancellationToken);

        var (createdId, skipReason) = await ResolveUploadedProbeTransactionAsync(
            accountId,
            fitId,
            cancellationToken);

        SampleOutput.WriteHeader("Uploaded statement file");
        SampleOutput.WriteField("File", "sdk-probe.ofx");
        SampleOutput.WriteField("Fit id", fitId);
        SampleOutput.WriteField("Matched id", createdId);

        if (createdId is null)
        {
            SampleOutput.WriteField("Deleted", skipReason ?? "No matching transaction to delete.");
            return;
        }

        await ProbeBankTransactionLifecycleSupport.DeleteExplanationsAndBankTransactionAsync(
            context.Client,
            createdId.Value,
            cancellationToken);
        SampleOutput.WriteField("Deleted", createdId.Value);
    }

    private async Task<(long? TransactionId, string? SkipReason)> ResolveUploadedProbeTransactionAsync(
        long bankAccountId,
        string fitId,
        CancellationToken cancellationToken)
    {
        var uploaded = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            lastUploaded: true,
            cancellationToken: cancellationToken);

        var createdId = BankTransactionExplanationWriteProbeSupport.TryResolveUploadedBankTransactionId(
            uploaded.Items,
            fitId);
        if (createdId is null)
        {
            foreach (var orphan in BankTransactionExplanationWriteProbeSupport.FindUploadOrphans(uploaded.Items, fitId))
            {
                await ProbeBankTransactionLifecycleSupport.DeleteExplanationsAndBankTransactionAsync(
                    context.Client,
                    orphan.ResourceId,
                    cancellationToken);
            }

            return (null, "bank statement upload did not return a matching transaction id");
        }

        return (createdId, null);
    }

    private async Task<long?> GetFirstBankAccountIdAsync(CancellationToken cancellationToken)
    {
        var bankAccounts = await context.Client.BankAccounts.ListAsync(cancellationToken: cancellationToken);
        if (bankAccounts.Count == 0 || bankAccounts[0].ResourceId <= 0)
        {
            SampleOutput.WriteHeader("Bank transactions");
            SampleOutput.WriteField("Result", "No bank accounts returned.");
            return null;
        }

        return bankAccounts[0].ResourceId;
    }
}
