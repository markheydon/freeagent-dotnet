namespace FreeAgent.Client.Tests.Models.JournalSets;

public class JournalEntryWriteValidatorTests
{
    [Theory]
    [InlineData("601")]
    [InlineData("607")]
    [InlineData("602-1")]
    public void RequiresCapitalAssetType_ReturnsTrueForDocumentedRange(string nominalCode)
    {
        Assert.True(JournalEntryWriteValidator.RequiresCapitalAssetType(nominalCode));
    }

    [Theory]
    [InlineData("001")]
    [InlineData("600")]
    [InlineData("608")]
    public void RequiresCapitalAssetType_ReturnsFalseOutsideRange(string nominalCode)
    {
        Assert.False(JournalEntryWriteValidator.RequiresCapitalAssetType(nominalCode));
    }

    [Fact]
    public void ValidateForCreate_ThrowsWhenCapitalAssetCategoryMissingType()
    {
        var entries = new List<JournalEntry>
        {
            new() { CategoryNominalCode = "601", DebitValue = 10m },
            new() { CategoryNominalCode = "901", DebitValue = -10m }
        };

        var ex = Assert.Throws<ArgumentException>(() => JournalEntryWriteValidator.ValidateForCreate(entries));
        Assert.Contains("Capital asset type is required", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateForCreate_AllowsCapitalAssetCategoryWithTypeId()
    {
        var entries = new List<JournalEntry>
        {
            new() { CategoryNominalCode = "601", DebitValue = 10m, CapitalAssetTypeId = 3 },
            new() { CategoryNominalCode = "901", DebitValue = -10m }
        };

        JournalEntryWriteValidator.ValidateForCreate(entries);
    }

    [Fact]
    public void ValidateForUpdate_DestroyRequiresUrlOrEntryId()
    {
        var entries = new List<JournalEntry> { new() { Destroy = true } };

        Assert.Throws<ArgumentException>(() => JournalEntryWriteValidator.ValidateForUpdate(entries));
    }

    [Fact]
    public void ValidateForCreate_ThrowsWhenDebitValuesDoNotBalance()
    {
        var entries = new List<JournalEntry>
        {
            new() { CategoryNominalCode = "001", DebitValue = 10m },
            new() { CategoryNominalCode = "002", DebitValue = -5m }
        };

        var ex = Assert.Throws<ArgumentException>(() => JournalEntryWriteValidator.ValidateForCreate(entries));
        Assert.Contains("sum to zero", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateForCreate_ThrowsWhenOpeningBalanceCategoryMissingContact()
    {
        var entries = new List<JournalEntry>
        {
            new() { CategoryNominalCode = "681", DebitValue = 10m },
            new() { CategoryNominalCode = "901", DebitValue = -10m }
        };

        var ex = Assert.Throws<ArgumentException>(() => JournalEntryWriteValidator.ValidateForCreate(entries));
        Assert.Contains("Contact is required", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RequiresOpeningBalanceContact_ReturnsTrueFor681And796()
    {
        Assert.True(JournalEntryWriteValidator.RequiresOpeningBalanceContact("681"));
        Assert.True(JournalEntryWriteValidator.RequiresOpeningBalanceContact("796"));
        Assert.False(JournalEntryWriteValidator.RequiresOpeningBalanceContact("001"));
    }
}
