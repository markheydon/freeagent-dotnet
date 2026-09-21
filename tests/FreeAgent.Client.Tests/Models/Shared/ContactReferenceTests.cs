using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class ContactReferenceTests
{
    [Fact]
    public void ForEnvironment_Production_BuildsExpectedUri()
    {
        var reference = ContactReference.ForEnvironment(FreeAgentEnvironment.Production, 42);

        Assert.Equal("https://api.freeagent.com/v2/contacts/42", reference.Uri);
        Assert.Equal(42, reference.Id);
    }

    [Fact]
    public void ForEnvironment_Sandbox_BuildsExpectedUri()
    {
        var reference = ContactReference.ForEnvironment(FreeAgentEnvironment.Sandbox, 7);

        Assert.Equal("https://api.sandbox.freeagent.com/v2/contacts/7", reference.Uri);
        Assert.Equal(7, reference.Id);
    }

    [Fact]
    public void Parse_ExtractsIdFromUri()
    {
        var reference = ContactReference.Parse("https://api.freeagent.com/v2/contacts/99");

        Assert.Equal(99, reference.Id);
        Assert.Equal("https://api.freeagent.com/v2/contacts/99", reference.Uri);
    }

    [Fact]
    public void Parse_InvalidUri_Throws()
    {
        Assert.Throws<ArgumentException>(() => ContactReference.Parse("https://api.freeagent.com/v2/contacts/"));
    }
}
