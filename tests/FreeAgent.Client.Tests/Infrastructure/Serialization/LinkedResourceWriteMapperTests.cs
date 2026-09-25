using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

public class LinkedResourceWriteMapperTests
{
    [Fact]
    public void ResolveProjectReference_ExplicitClear_ReturnsClearedWriteLink()
    {
        var backing = default(SettableLinkId);
        backing.Set(null);

        var link = LinkedResourceWriteMapper.ResolveProjectReference(
            FreeAgentEnvironment.Production,
            backing,
            linkId: 5);

        Assert.NotNull(link);
        Assert.True(link!.IsCleared);
    }

    [Fact]
    public void ResolveProjectReference_Omit_ReturnsNull()
    {
        var backing = default(SettableLinkId);
        backing.Set(3);

        var link = LinkedResourceWriteMapper.ResolveProjectReference(
            FreeAgentEnvironment.Production,
            backing,
            linkId: 5,
            omit: true);

        Assert.Null(link);
    }

    [Fact]
    public void ResolveCategoryReference_ExplicitClear_ReturnsClearedWriteLink()
    {
        var backing = default(SettableLinkValue);
        backing.Set(null);

        var link = LinkedResourceWriteMapper.ResolveCategoryReference(
            FreeAgentEnvironment.Production,
            backing,
            linkNominalCode: "001");

        Assert.NotNull(link);
        Assert.True(link!.IsCleared);
    }

    [Fact]
    public void ResolveContactReference_RoundTripsFromWireLink()
    {
        var backing = default(SettableLinkId);

        var link = LinkedResourceWriteMapper.ResolveContactReference(
            FreeAgentEnvironment.Production,
            backing,
            linkId: 9);

        Assert.NotNull(link);
        Assert.False(link!.IsCleared);
        Assert.Equal("https://api.freeagent.com/v2/contacts/9", link.Value.Uri);
    }
}
