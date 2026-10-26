namespace FreeAgent.Client.Tests;

public class FreeAgentResourceUrlsTests
{
    [Fact]
    public void ForEnvironment_BuildsSameContactUriAsContactReference()
    {
        const long contactId = 42;
        var environment = FreeAgentEnvironment.Sandbox;

        var fromUrls = FreeAgentResourceUrls.ForEnvironment(environment).Contact(contactId).Uri;
        var fromReference = ContactReference.ForEnvironment(environment, contactId).Uri;

        Assert.Equal(fromReference, fromUrls);
    }
}
