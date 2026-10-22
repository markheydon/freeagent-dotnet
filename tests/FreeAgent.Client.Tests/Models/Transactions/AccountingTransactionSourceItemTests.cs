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

    [Fact]
    public void TryParse_RecognisesJournalEntry()
    {
        var url = "https://api.freeagent.com/v2/journal_sets/37/journal_entries/17";

        Assert.True(AccountingTransactionSourceItem.TryParse(url, out var kind, out var id));
        Assert.Equal(AccountingTransactionSourceKind.JournalEntry, kind);
        Assert.Equal(17, id);
    }

    [Fact]
    public void TryParse_ReturnsFalseForUnrecognisedSegmentWithNumericId()
    {
        var url = "https://api.freeagent.com/v2/manual_journals/99";

        Assert.False(AccountingTransactionSourceItem.TryParse(url, out var kind, out var id));
        Assert.Equal(AccountingTransactionSourceKind.Unknown, kind);
        Assert.Equal(99, id);
    }
}
