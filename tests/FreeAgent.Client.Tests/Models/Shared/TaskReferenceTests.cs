using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class TaskReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = TaskReference.ForEnvironment(FreeAgentEnvironment.Production, 12);

        Assert.Equal("https://api.freeagent.com/v2/tasks/12", reference.Uri);
        Assert.Equal(12, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = TaskReference.Parse("https://api.sandbox.freeagent.com/v2/tasks/3");

        Assert.Equal(3, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            TaskReference.Parse("https://api.freeagent.com/v2/projects/12"));

        Assert.Contains("tasks", exception.Message, StringComparison.Ordinal);
    }
}
