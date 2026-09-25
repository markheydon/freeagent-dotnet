using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class StockItemReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = StockItemReference.ForEnvironment(FreeAgentEnvironment.Production, 4);

        Assert.Equal("https://api.freeagent.com/v2/stock_items/4", reference.Uri);
        Assert.Equal(4, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = StockItemReference.Parse("https://api.sandbox.freeagent.com/v2/stock_items/9");

        Assert.Equal(9, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            StockItemReference.Parse("https://api.freeagent.com/v2/contacts/12"));

        Assert.Contains("stock_items", exception.Message, StringComparison.Ordinal);
    }
}
