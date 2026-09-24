using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class CategoryReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = CategoryReference.ForEnvironment(FreeAgentEnvironment.Production, "001");

        Assert.Equal("https://api.freeagent.com/v2/categories/001", reference.Uri);
        Assert.Equal("001", reference.NominalCode);
    }

    [Fact]
    public void Parse_ExtractsNominalCodeFromUri()
    {
        var reference = CategoryReference.Parse("https://api.sandbox.freeagent.com/v2/categories/602-1");

        Assert.Equal("602-1", reference.NominalCode);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CategoryReference.Parse("https://api.freeagent.com/v2/contacts/12"));

        Assert.Contains("categories", exception.Message, StringComparison.Ordinal);
    }
}
