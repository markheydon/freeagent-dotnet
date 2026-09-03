using System.Text.Json;
using FreeAgent.Client;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Persists the sample app OAuth session in a short-lived browser cookie for local development.
/// Tokens are stored as plaintext JSON and must not be used outside local single-user scenarios.
/// </summary>
internal static class OAuthSessionPersistence
{
    private const string CookieName = "freeagent.sample.oauth";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromHours(1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static void Save(HttpResponse response, OAuthTokenResponse token, FreeAgentEnvironment environment)
    {
        if (token.ExpiresAtUtc is null)
        {
            token.InitialiseExpiryUtc();
        }

        var payload = new PersistedOAuthSession(
            token,
            environment,
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(payload, JsonOptions);

        response.Cookies.Append(CookieName, json, CreateCookieOptions(response.HttpContext.Request.IsHttps));
    }

    public static bool TryLoad(HttpRequest request, out OAuthTokenResponse? token, out FreeAgentEnvironment environment)
    {
        token = null;
        environment = FreeAgentEnvironment.Production;

        if (!request.Cookies.TryGetValue(CookieName, out var json) || string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            var payload = JsonSerializer.Deserialize<PersistedOAuthSession>(json, JsonOptions);
            if (payload?.Token is null)
            {
                return false;
            }

            if (DateTimeOffset.UtcNow - payload.StoredAtUtc.ToUniversalTime() > CookieLifetime)
            {
                return false;
            }

            if (payload.Token.ExpiresAtUtc is null)
            {
                payload.Token.InitialiseExpiryUtc();
            }

            if (payload.Token.IsExpired)
            {
                return false;
            }

            token = payload.Token;
            environment = payload.Environment;
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static void Clear(HttpResponse response)
    {
        response.Cookies.Delete(CookieName, CreateCookieOptions(response.HttpContext.Request.IsHttps));
    }

    private static CookieOptions CreateCookieOptions(bool isHttps) =>
        new()
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = CookieLifetime,
            IsEssential = true,
            Path = "/"
        };

    private sealed record PersistedOAuthSession(
        OAuthTokenResponse Token,
        FreeAgentEnvironment Environment,
        DateTimeOffset StoredAtUtc);
}
