using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class AttachmentReferenceTests
{
    [Fact]
    public void Parse_ValidUri_ReturnsReference()
    {
        var reference = AttachmentReference.Parse("https://api.freeagent.com/v2/attachments/7");

        Assert.Equal(7, reference.Id);
        Assert.Equal("https://api.freeagent.com/v2/attachments/7", reference.Uri);
    }

    [Fact]
    public void ForEnvironment_Production_BuildsUri()
    {
        var reference = AttachmentReference.ForEnvironment(FreeAgentEnvironment.Production, 12);

        Assert.Equal(12, reference.Id);
        Assert.Equal("https://api.freeagent.com/v2/attachments/12", reference.Uri);
    }
}
