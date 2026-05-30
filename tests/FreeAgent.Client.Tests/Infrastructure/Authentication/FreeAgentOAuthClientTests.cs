using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using Xunit;

namespace FreeAgent.Client.Tests.Infrastructure.Authentication;

public class FreeAgentOAuthClientTests
{
    [Fact]
    public void Constructor_WithNullClientId_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new FreeAgentOAuthClient(null!, "secret", "http://localhost"));
    }

    [Fact]
    public void Constructor_WithNullClientSecret_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new FreeAgentOAuthClient("clientId", null!, "http://localhost"));
    }

    [Fact]
    public void Constructor_WithNullRedirectUri_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new FreeAgentOAuthClient("clientId", "secret", null!));
    }

    [Fact]
    public void GetAuthorizationUrl_ReturnsCorrectUrl()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback");

        var url = client.GetAuthorizationUrl();

        Assert.Contains("https://api.freeagent.com/v2/approve_app", url);
        Assert.Contains("response_type=code", url);
        Assert.Contains("client_id=test-client-id", url);
        Assert.Contains("redirect_uri=", url);
    }

    [Fact]
    public void GetAuthorizationUrl_WithState_IncludesStateParameter()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback");

        var url = client.GetAuthorizationUrl("test-state-123");

        Assert.Contains("state=test-state-123", url);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithNullCode_ThrowsArgumentNullException()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback");

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.ExchangeCodeForTokenAsync(null!));
    }

    [Fact]
    public async Task RefreshTokenAsync_WithNullRefreshToken_ThrowsArgumentNullException()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback");

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.RefreshTokenAsync(null!));
    }

    [Fact]
    public void GetAuthorizationUrl_WithProductionEnvironment_UsesProductionEndpoint()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback", FreeAgentEnvironment.Production);

        var url = client.GetAuthorizationUrl();

        Assert.Contains("https://api.freeagent.com/v2/approve_app", url);
    }

    [Fact]
    public void GetAuthorizationUrl_WithSandboxEnvironment_UsesSandboxEndpoint()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback", FreeAgentEnvironment.Sandbox);

        var url = client.GetAuthorizationUrl();

        Assert.Contains("https://api.sandbox.freeagent.com/v2/approve_app", url);
        Assert.DoesNotContain("https://api.freeagent.com/v2/approve_app", url);
    }

    [Fact]
    public void GetAuthorizationUrl_DefaultEnvironment_UsesProductionEndpoint()
    {
        var client = new FreeAgentOAuthClient("test-client-id", "test-secret", "http://localhost/callback");

        var url = client.GetAuthorizationUrl();

        Assert.Contains("https://api.freeagent.com/v2/approve_app", url);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_InitialisesExpiresAtUtc()
    {
        using var httpClient = new HttpClient(new LambdaHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"access_token\":\"new-access\",\"token_type\":\"Bearer\",\"refresh_token\":\"refresh\",\"expires_in\":3600}",
                    Encoding.UTF8,
                    "application/json")
            }));

        var client = new FreeAgentOAuthClient(
            "test-client-id",
            "test-secret",
            "http://localhost/callback",
            httpClient,
            FreeAgentEnvironment.Production);

        var before = DateTimeOffset.UtcNow;
        var token = await client.ExchangeCodeForTokenAsync("auth-code");
        var after = DateTimeOffset.UtcNow;

        Assert.NotNull(token.ExpiresAtUtc);
        Assert.True(token.ExpiresAtUtc >= before.AddSeconds(3599));
        Assert.True(token.ExpiresAtUtc <= after.AddSeconds(3601));
    }

    [Fact]
    public async Task RefreshTokenAsync_InitialisesExpiresAtUtc()
    {
        using var httpClient = new HttpClient(new LambdaHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"access_token\":\"new-access\",\"token_type\":\"Bearer\",\"refresh_token\":\"refresh\",\"expires_in\":1800}",
                    Encoding.UTF8,
                    "application/json")
            }));

        var client = new FreeAgentOAuthClient(
            "test-client-id",
            "test-secret",
            "http://localhost/callback",
            httpClient,
            FreeAgentEnvironment.Production);

        var token = await client.RefreshTokenAsync("refresh-token");

        Assert.NotNull(token.ExpiresAtUtc);
        Assert.False(token.IsExpired);
    }

    private sealed class LambdaHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public LambdaHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_handler(request));
        }
    }
}
