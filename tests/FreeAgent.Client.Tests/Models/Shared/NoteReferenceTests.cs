using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class NoteReferenceTests
{
    [Fact]
    public void ForEnvironment_BuildsExpectedUri()
    {
        var reference = NoteReference.ForEnvironment(FreeAgentEnvironment.Production, 42);

        Assert.Equal("https://api.freeagent.com/v2/notes/42", reference.Uri);
        Assert.Equal(42, reference.Id);
    }

    [Fact]
    public void Parse_ValidUri_ReturnsReference()
    {
        var reference = NoteReference.Parse("https://api.freeagent.com/v2/notes/7");

        Assert.Equal(7, reference.Id);
        Assert.Equal("https://api.freeagent.com/v2/notes/7", reference.Uri);
    }
}
