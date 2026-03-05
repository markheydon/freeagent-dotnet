using FreeAgent.Client.Authentication;
using Xunit;

namespace FreeAgent.Client.Tests.Authentication;

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
}
