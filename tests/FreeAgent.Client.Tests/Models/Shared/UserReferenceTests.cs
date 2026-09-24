using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class UserReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = UserReference.ForEnvironment(FreeAgentEnvironment.Production, 12);

        Assert.Equal("https://api.freeagent.com/v2/users/12", reference.Uri);
        Assert.Equal(12, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = UserReference.Parse("https://api.sandbox.freeagent.com/v2/users/3");

        Assert.Equal(3, reference.Id);
    }

    [Fact]
    public void Parse_WrongResourceType_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            UserReference.Parse("https://api.freeagent.com/v2/contacts/12"));

        Assert.Contains("users", exception.Message, StringComparison.Ordinal);
    }
}
