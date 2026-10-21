#if NET10_0
using FreeAgent.Client;
using FreeAgent.Client.Samples.Shared.Probes;

namespace FreeAgent.Client.Tests.Samples.Probes;

public class BankTransactionExplanationWriteProbeSupportTests
{
    [Fact]
    public void TryResolveUploadedBankTransactionId_MatchesTransactionId()
    {
        var items = new[]
        {
            CreateTransaction(10, transactionId: "fit-a", description: "other"),
            CreateTransaction(42, transactionId: "fit-b", description: "probe"),
        };

        Assert.Equal(42, BankTransactionExplanationWriteProbeSupport.TryResolveUploadedBankTransactionId(items, "fit-b"));
    }

    [Fact]
    public void TryResolveUploadedBankTransactionId_FallsBackToDescription()
    {
        var items = new[] { CreateTransaction(7, transactionId: null, description: "sdk-console-bte-123") };

        Assert.Equal(7, BankTransactionExplanationWriteProbeSupport.TryResolveUploadedBankTransactionId(items, "sdk-console-bte-123"));
    }

    [Fact]
    public void FindUploadOrphans_ReturnsCaseSensitiveMatches()
    {
        var items = new[]
        {
            CreateTransaction(1, transactionId: "fit", description: "x"),
            CreateTransaction(2, transactionId: "other", description: "fit"),
            CreateTransaction(3, transactionId: "nope", description: "nope"),
        };

        var orphans = BankTransactionExplanationWriteProbeSupport.FindUploadOrphans(items, "fit");

        Assert.Equal(2, orphans.Count);
        Assert.Contains(orphans, transaction => transaction.ResourceId == 1);
        Assert.Contains(orphans, transaction => transaction.ResourceId == 2);
    }

    [Fact]
    public void TryValidateCreatePrerequisites_TransferRequiresSecondAccount()
    {
        var context = new BankTransactionExplanationWriteProbeContext { TransferBankAccountId = 0 };

        Assert.False(
            BankTransactionExplanationWriteProbeSupport.TryValidateCreatePrerequisites(
                BankTransactionExplanationWriteProbeVariant.CreateTransfer,
                context,
                out var reason));
        Assert.Contains("two bank accounts", reason, StringComparison.Ordinal);
    }

    [Fact]
    public void TryValidateCreatePrerequisites_Category_AllowsMinimalContext()
    {
        var context = new BankTransactionExplanationWriteProbeContext
        {
            BankTransactionId = 1,
            CategoryNominalCode = "285",
        };

        Assert.True(
            BankTransactionExplanationWriteProbeSupport.TryValidateCreatePrerequisites(
                BankTransactionExplanationWriteProbeVariant.CreateCategory,
                context,
                out _));
    }

    [Fact]
    public void ProbeAmounts_MatchMoneyOutAndDisposalSigns()
    {
        Assert.Equal(
            BankTransactionExplanationWriteProbeSupport.ProbeMoneyOutStatementAmount,
            BankTransactionExplanationWriteProbeSupport.ProbeMoneyOutGrossValue);
        Assert.Equal(
            BankTransactionExplanationWriteProbeSupport.ProbeDisposalStatementAmount,
            BankTransactionExplanationWriteProbeSupport.ProbeDisposalGrossValue);
        Assert.True(BankTransactionExplanationWriteProbeSupport.ProbeMoneyOutGrossValue < 0);
        Assert.True(BankTransactionExplanationWriteProbeSupport.ProbeDisposalGrossValue > 0);
    }

    private static BankTransaction CreateTransaction(long id, string? transactionId, string description) =>
        new()
        {
            Url = $"https://api.freeagent.com/v2/bank_transactions/{id}",
            Description = description,
            TransactionId = transactionId,
        };
}
#endif
