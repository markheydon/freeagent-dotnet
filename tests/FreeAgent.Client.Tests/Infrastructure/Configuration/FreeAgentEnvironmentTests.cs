using FreeAgent.Client.Infrastructure.Authentication;
using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client.Tests.Infrastructure.Configuration;

public class FreeAgentEnvironmentTests
{
    [Fact]
    public void FreeAgentClient_DefaultConstructor_DoesNotThrow()
    {
        using var client = new FreeAgentClient("test-token");

        Assert.NotNull(client);
    }

    [Fact]
    public void FreeAgentClient_WithProductionEnvironment_DoesNotThrow()
    {
        var client = new FreeAgentClient("test-token", FreeAgentEnvironment.Production);

        Assert.NotNull(client);
        client.Dispose();
    }

    [Fact]
    public void FreeAgentClient_WithSandboxEnvironment_DoesNotThrow()
    {
        var client = new FreeAgentClient("test-token", FreeAgentEnvironment.Sandbox);

        Assert.NotNull(client);
        client.Dispose();
    }

    [Fact]
    public void FreeAgentClient_WithOAuthAndSandboxEnvironment_DoesNotThrow()
    {
        var oauthClient = new FreeAgentOAuthClient("client-id", "client-secret", "http://localhost", FreeAgentEnvironment.Sandbox);
        var token = new OAuthTokenResponse { AccessToken = "test-token", TokenType = "Bearer", ExpiresIn = 3600 };

        var client = new FreeAgentClient(oauthClient, token, FreeAgentEnvironment.Sandbox);

        Assert.NotNull(client);
        client.Dispose();
    }
}
