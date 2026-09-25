using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

public class SettableLinkTests
{
    [Fact]
    public void SettableLinkId_Unset_FallsBackToWireLinkId()
    {
        var backing = default(SettableLinkId);

        Assert.False(backing.IsExplicitlySet);
        Assert.Equal(8, backing.Get(8));
        Assert.Null(backing.Get(null));
    }

    [Fact]
    public void SettableLinkId_ExplicitValue_OverridesWireLinkId()
    {
        var backing = default(SettableLinkId);
        backing.Set(3);

        Assert.True(backing.IsExplicitlySet);
        Assert.Equal(3, backing.Get(8));
    }

    [Fact]
    public void SettableLinkId_ExplicitNull_IgnoresWireLinkId()
    {
        var backing = default(SettableLinkId);
        backing.Set(null);

        Assert.True(backing.IsExplicitlySet);
        Assert.Null(backing.Get(8));
    }

    [Fact]
    public void SettableLinkValue_Unset_FallsBackToWireKey()
    {
        var backing = default(SettableLinkValue);

        Assert.False(backing.IsExplicitlySet);
        Assert.Equal("001", backing.Get("001"));
        Assert.Null(backing.Get(null));
    }

    [Fact]
    public void SettableLinkValue_ExplicitValue_OverridesWireKey()
    {
        var backing = default(SettableLinkValue);
        backing.Set("002");

        Assert.True(backing.IsExplicitlySet);
        Assert.Equal("002", backing.Get("001"));
    }

    [Fact]
    public void SettableLinkValue_ExplicitNull_IgnoresWireKey()
    {
        var backing = default(SettableLinkValue);
        backing.Set(null);

        Assert.True(backing.IsExplicitlySet);
        Assert.Null(backing.Get("001"));
    }
}
