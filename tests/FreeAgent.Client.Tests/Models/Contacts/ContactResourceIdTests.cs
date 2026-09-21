using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Tests.Models.Contacts;

public class ContactResourceIdTests
{
    [Fact]
    public void ResourceId_ParsesFromUrl()
    {
        var contact = new Contact
        {
            Url = "https://api.freeagent.com/v2/contacts/123"
        };

        Assert.Equal(123, contact.ResourceId);
    }
}
