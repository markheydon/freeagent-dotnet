using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class TimeslipReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = TimeslipReference.ForEnvironment(FreeAgentEnvironment.Production, 25);

        Assert.Equal("https://api.freeagent.com/v2/timeslips/25", reference.Uri);
        Assert.Equal(25, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = TimeslipReference.Parse("https://api.sandbox.freeagent.com/v2/timeslips/3");

        Assert.Equal(3, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            TimeslipReference.Parse("https://api.freeagent.com/v2/tasks/12"));

        Assert.Contains("timeslips", exception.Message, StringComparison.Ordinal);
    }
}
