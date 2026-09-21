using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class ProjectReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = ProjectReference.ForEnvironment(FreeAgentEnvironment.Production, 12);

        Assert.Equal("https://api.freeagent.com/v2/projects/12", reference.Uri);
        Assert.Equal(12, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = ProjectReference.Parse("https://api.sandbox.freeagent.com/v2/projects/3");

        Assert.Equal(3, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            ProjectReference.Parse("https://api.freeagent.com/v2/contacts/12"));

        Assert.Contains("projects", exception.Message, StringComparison.Ordinal);
    }
}
