using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class HirePurchaseReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = HirePurchaseReference.ForEnvironment(FreeAgentEnvironment.Production, 4);

        Assert.Equal("https://api.freeagent.com/v2/hire_purchases/4", reference.Uri);
        Assert.Equal(4, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = HirePurchaseReference.Parse("https://api.sandbox.freeagent.com/v2/hire_purchases/9");

        Assert.Equal(9, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            HirePurchaseReference.Parse("https://api.freeagent.com/v2/contacts/12"));

        Assert.Contains("hire_purchases", exception.Message, StringComparison.Ordinal);
    }
}
