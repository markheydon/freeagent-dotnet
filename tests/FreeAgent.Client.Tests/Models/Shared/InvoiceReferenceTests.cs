using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class InvoiceReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = InvoiceReference.ForEnvironment(FreeAgentEnvironment.Production, 7);

        Assert.Equal("https://api.freeagent.com/v2/invoices/7", reference.Uri);
        Assert.Equal(7, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = InvoiceReference.Parse("https://api.sandbox.freeagent.com/v2/invoices/3");

        Assert.Equal(3, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            InvoiceReference.Parse("https://api.freeagent.com/v2/projects/12"));

        Assert.Contains("invoices", exception.Message, StringComparison.Ordinal);
    }
}
