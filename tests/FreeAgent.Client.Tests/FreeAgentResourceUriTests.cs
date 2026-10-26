namespace FreeAgent.Client.Tests;

public class FreeAgentResourceUriTests
{
    [Fact]
    public void TryParseResourceId_ParsesTrailingSegment()
    {
        Assert.True(FreeAgentResourceUri.TryParseResourceId(
            "https://api.freeagent.com/v2/projects/42",
            out var id));
        Assert.Equal(42, id);
    }

    [Fact]
    public void ParseResourceId_ThrowsWhenInvalid()
    {
        Assert.Throws<ArgumentException>(() => FreeAgentResourceUri.ParseResourceId("not-a-url"));
    }
}
