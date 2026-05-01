using FreeAgent.Client.Infrastructure.Authentication;
using Xunit;

namespace FreeAgent.Client.Tests.Infrastructure.Authentication;

public class OAuthTokenResponseTests
{
    [Fact]
    public void IsExpired_WithExpiredToken_ReturnsTrue()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresIn = 3600,
            IssuedAt = DateTime.UtcNow.AddHours(-2)
        };

        Assert.True(token.IsExpired);
    }

    [Fact]
    public void IsExpired_WithValidToken_ReturnsFalse()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresIn = 3600,
            IssuedAt = DateTime.UtcNow
        };

        Assert.False(token.IsExpired);
    }

    [Fact]
    public void IsExpiringSoon_WithTokenExpiringIn4Minutes_ReturnsTrue()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresIn = 240, // 4 minutes
            IssuedAt = DateTime.UtcNow
        };

        Assert.True(token.IsExpiringSoon);
    }

    [Fact]
    public void IsExpiringSoon_WithTokenExpiringIn10Minutes_ReturnsFalse()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresIn = 600, // 10 minutes
            IssuedAt = DateTime.UtcNow
        };

        Assert.False(token.IsExpiringSoon);
    }
}
