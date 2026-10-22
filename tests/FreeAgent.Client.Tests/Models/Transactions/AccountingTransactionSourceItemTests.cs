namespace FreeAgent.Client.Tests.Models.Transactions;

public class AccountingTransactionSourceItemTests
{
    [Fact]
    public void TryParse_RecognisesBankTransactionExplanation()
    {
        var url = "https://api.freeagent.com/v2/bank_transaction_explanations/1";

        Assert.True(AccountingTransactionSourceItem.TryParse(url, out var kind, out var id));
        Assert.Equal(AccountingTransactionSourceKind.BankTransactionExplanation, kind);
        Assert.Equal(1, id);
    }

    [Fact]
    public void TryParse_RecognisesBill()
    {
        var url = "https://api.freeagent.com/v2/bills/2";

        Assert.True(AccountingTransactionSourceItem.TryParse(url, out var kind, out var id));
        Assert.Equal(AccountingTransactionSourceKind.Bill, kind);
        Assert.Equal(2, id);
    }

    [Fact]
    public void TryParse_ReturnsFalseForMissingUrl()
    {
        Assert.False(AccountingTransactionSourceItem.TryParse(null, out _, out _));
    }
}
