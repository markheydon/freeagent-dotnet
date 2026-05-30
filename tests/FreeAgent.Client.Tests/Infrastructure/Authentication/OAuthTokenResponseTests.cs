using System.Text.Json;
using FreeAgent.Client;
using Xunit;

namespace FreeAgent.Client.Tests.Infrastructure.Authentication;

public class OAuthTokenResponseTests
{
    [Fact]
    public void IsExpired_WithPastExpiresAtUtc_ReturnsTrue()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(-1)
        };

        Assert.True(token.IsExpired);
    }

    [Fact]
    public void IsExpired_WithFutureExpiresAtUtc_ReturnsFalse()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        Assert.False(token.IsExpired);
    }

    [Fact]
    public void IsExpiringSoon_WithTokenExpiringIn4Minutes_ReturnsTrue()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(4)
        };

        Assert.True(token.IsExpiringSoon);
    }

    [Fact]
    public void IsExpiringSoon_WithTokenExpiringIn10Minutes_ReturnsFalse()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        Assert.False(token.IsExpiringSoon);
    }

    [Fact]
    public void InitialiseExpiryUtc_WhenExpiresInSet_ComputesPersistedExpiry()
    {
        var token = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            ExpiresIn = 120
        };

        var issuedAt = DateTimeOffset.UtcNow;
        token.InitialiseExpiryUtc(issuedAt);

        Assert.NotNull(token.ExpiresAtUtc);
        Assert.Equal(issuedAt.AddSeconds(120), token.ExpiresAtUtc.Value);
    }

    [Fact]
    public void JsonRoundTrip_PreservesExpiresAtUtc()
    {
        var original = new OAuthTokenResponse
        {
            AccessToken = "test-token",
            TokenType = "Bearer",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(42)
        };

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<OAuthTokenResponse>(json);

        Assert.NotNull(restored);
        Assert.Equal(original.ExpiresAtUtc, restored!.ExpiresAtUtc);
        Assert.False(restored.IsExpired);
    }
}
