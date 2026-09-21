using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Tests;

public class FreeAgentResourceExtensionsTests
{
    [Fact]
    public void TryGetResourceId_ValidUrl_ReturnsTrue()
    {
        var contact = new Contact
        {
            Url = "https://api.freeagent.com/v2/contacts/42"
        };

        Assert.True(contact.TryGetResourceId(out var id));
        Assert.Equal(42, id);
    }

    [Fact]
    public void TryGetResourceId_InvalidUrl_ReturnsFalse()
    {
        var contact = new Contact
        {
            Url = "https://api.freeagent.com/v2/contacts/"
        };

        Assert.False(contact.TryGetResourceId(out var id));
        Assert.Equal(0, id);
    }

    [Fact]
    public void GetResourceId_InvalidUrl_Throws()
    {
        var contact = new Contact
        {
            Url = string.Empty
        };

        var exception = Assert.Throws<InvalidOperationException>(() => contact.GetResourceId());
        Assert.Contains("Could not parse a valid resource identifier", exception.Message, StringComparison.Ordinal);
    }
}
