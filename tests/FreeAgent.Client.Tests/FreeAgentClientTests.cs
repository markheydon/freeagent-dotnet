using FreeAgent.Client.Infrastructure.Authentication;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Company;
using FreeAgent.Client.Services.Contacts;
using Xunit;

namespace FreeAgent.Client.Tests;

public class FreeAgentClientTests
{
    [Fact]
    public void Constructor_WithNullAccessToken_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FreeAgentClient((string)null!));
    }

    [Fact]
    public void Constructor_WithAccessToken_InitializesCompanyService()
    {
        var client = new FreeAgentClient("test-token");

        Assert.NotNull(client.Company);
        Assert.NotNull(client.Contacts);
    }

    [Fact]
    public void Constructor_WithOAuthClientAndToken_InitializesCompanyService()
    {
        var oauthClient = new FreeAgentOAuthClient("client-id", "client-secret", "http://localhost");
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            TokenType = "Bearer",
            ExpiresIn = 3600
        };

        var client = new FreeAgentClient(oauthClient, token);

        Assert.NotNull(client.Company);
        Assert.NotNull(client.Contacts);
    }

    [Fact]
    public void HttpPlumbing_IsNotPublicApi()
    {
        Assert.False(typeof(FreeAgentHttpClient).IsPublic);
    }

    [Fact]
    public void ServiceTypes_AreSealed_WithNoPublicConstructors()
    {
        Assert.True(typeof(CompanyService).IsSealed);
        Assert.True(typeof(ContactService).IsSealed);

        Assert.Empty(typeof(CompanyService).GetConstructors());
        Assert.Empty(typeof(ContactService).GetConstructors());
    }
}
