using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class BankAccountReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = BankAccountReference.ForEnvironment(FreeAgentEnvironment.Production, 4);

        Assert.Equal("https://api.freeagent.com/v2/bank_accounts/4", reference.Uri);
        Assert.Equal(4, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = BankAccountReference.Parse("https://api.sandbox.freeagent.com/v2/bank_accounts/9");

        Assert.Equal(9, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            BankAccountReference.Parse("https://api.freeagent.com/v2/contacts/12"));

        Assert.Contains("bank_accounts", exception.Message, StringComparison.Ordinal);
    }
}
