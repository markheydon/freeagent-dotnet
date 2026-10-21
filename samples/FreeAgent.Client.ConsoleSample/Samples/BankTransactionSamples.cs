using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client.Models.BankTransactions;

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
        if (bankAccountId is null)
        {
            return;
        }

        var fitId = $"sdk-console-{DateTime.UtcNow:yyyyMMddHHmmss}";
        await context.Client.BankTransactions.UploadBankStatementAsync(
            new UploadBankStatementRequest(
            [
                new BankStatementTransaction
                {
                    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                    Amount = -0.01m,
                    Description = fitId,
                    FitId = fitId,
                    TransactionType = BankStatementTransactionType.Other
                }
            ]),
            bankAccountId: bankAccountId,
            cancellationToken: cancellationToken);

        var uploaded = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            lastUploaded: true,
            cancellationToken: cancellationToken);

        var created = uploaded.Items.FirstOrDefault(transaction => transaction.TransactionId == fitId)
            ?? uploaded.Items.FirstOrDefault(transaction => transaction.Description == fitId);

        SampleOutput.WriteHeader("Uploaded statement line");
        SampleOutput.WriteField("Fit id", fitId);
        SampleOutput.WriteField("Matched id", created?.ResourceId);

        if (created is null || created.ResourceId <= 0)
        {
            SampleOutput.WriteField("Deleted", "No matching transaction to delete.");
            return;
        }

        await context.Client.BankTransactions.DeleteBankTransactionAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteField("Deleted", created.ResourceId);
    }

    [ConsoleSample(Name = "Upload statement file and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UploadStatementFileAsync(CancellationToken cancellationToken)
    {
        var bankAccountId = await GetFirstBankAccountIdAsync(cancellationToken);
        if (bankAccountId is null)
        {
            return;
        }

        var fitId = $"sdk-console-file-{DateTime.UtcNow:yyyyMMddHHmmss}";
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
            <TRNAMT>-0.02
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
            bankAccountId: bankAccountId,
            cancellationToken: cancellationToken);

        var uploaded = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            lastUploaded: true,
            cancellationToken: cancellationToken);

        var created = uploaded.Items.FirstOrDefault(transaction => transaction.TransactionId == fitId)
            ?? uploaded.Items.FirstOrDefault(transaction => transaction.Description?.Contains(fitId, StringComparison.Ordinal) == true);

        SampleOutput.WriteHeader("Uploaded statement file");
        SampleOutput.WriteField("File", "sdk-probe.ofx");
        SampleOutput.WriteField("Fit id", fitId);
        SampleOutput.WriteField("Matched id", created?.ResourceId);

        if (created is null || created.ResourceId <= 0)
        {
            SampleOutput.WriteField("Deleted", "No matching transaction to delete.");
            return;
        }

        await context.Client.BankTransactions.DeleteBankTransactionAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteField("Deleted", created.ResourceId);
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
