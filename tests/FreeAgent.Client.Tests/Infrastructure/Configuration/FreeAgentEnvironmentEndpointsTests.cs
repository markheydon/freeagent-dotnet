using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client.Tests.Infrastructure.Configuration;

public class FreeAgentEnvironmentEndpointsTests
{
    [Fact]
    public void GetApiBaseUrl_ForProduction_ReturnsProductionBaseUrl()
    {
        var value = FreeAgentEnvironmentEndpoints.GetApiBaseUrl(FreeAgentEnvironment.Production);

        Assert.Equal("https://api.freeagent.com/v2/", value);
    }

    [Fact]
    public void GetApiBaseUrl_ForSandbox_ReturnsSandboxBaseUrl()
    {
        var value = FreeAgentEnvironmentEndpoints.GetApiBaseUrl(FreeAgentEnvironment.Sandbox);

        Assert.Equal("https://api.sandbox.freeagent.com/v2/", value);
    }

    [Fact]
    public void GetOAuthAuthorizationEndpoint_ForProduction_ReturnsProductionEndpoint()
    {
        var value = FreeAgentEnvironmentEndpoints.GetOAuthAuthorizationEndpoint(FreeAgentEnvironment.Production);

        Assert.Equal("https://api.freeagent.com/v2/approve_app", value);
    }

    [Fact]
    public void GetOAuthAuthorizationEndpoint_ForSandbox_ReturnsSandboxEndpoint()
    {
        var value = FreeAgentEnvironmentEndpoints.GetOAuthAuthorizationEndpoint(FreeAgentEnvironment.Sandbox);

        Assert.Equal("https://api.sandbox.freeagent.com/v2/approve_app", value);
    }

    [Fact]
    public void GetOAuthTokenEndpoint_ForProduction_ReturnsProductionEndpoint()
    {
        var value = FreeAgentEnvironmentEndpoints.GetOAuthTokenEndpoint(FreeAgentEnvironment.Production);

        Assert.Equal("https://api.freeagent.com/v2/token_endpoint", value);
    }

    [Fact]
    public void GetOAuthTokenEndpoint_ForSandbox_ReturnsSandboxEndpoint()
    {
        var value = FreeAgentEnvironmentEndpoints.GetOAuthTokenEndpoint(FreeAgentEnvironment.Sandbox);

        Assert.Equal("https://api.sandbox.freeagent.com/v2/token_endpoint", value);
    }
}
