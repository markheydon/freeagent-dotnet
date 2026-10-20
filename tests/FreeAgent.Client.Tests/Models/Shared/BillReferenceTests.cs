using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class BillReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = BillReference.ForEnvironment(FreeAgentEnvironment.Production, 7);

        Assert.Equal("https://api.freeagent.com/v2/bills/7", reference.Uri);
        Assert.Equal(7, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = BillReference.Parse("https://api.sandbox.freeagent.com/v2/bills/3");

        Assert.Equal(3, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            BillReference.Parse("https://api.freeagent.com/v2/projects/12"));

        Assert.Contains("bills", exception.Message, StringComparison.Ordinal);
    }
}
