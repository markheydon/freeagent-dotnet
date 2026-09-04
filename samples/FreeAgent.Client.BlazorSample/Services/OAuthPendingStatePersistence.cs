using System.Text.Json;
using FreeAgent.Client;

namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Persists the in-flight OAuth CSRF state in a short-lived browser cookie so callbacks
/// still validate after an app restart during local development.
/// </summary>
internal static class OAuthPendingStatePersistence
{
    private const string CookieName = "freeagent.sample.oauth.pending";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromMinutes(15);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static void Save(HttpResponse response, string state, FreeAgentEnvironment environment)
    {
        var payload = new PersistedOAuthPendingState(state, environment, DateTimeOffset.UtcNow);
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        response.Cookies.Append(CookieName, json, CreateCookieOptions(response.HttpContext.Request.IsHttps));
    }

    public static bool TryLoad(HttpRequest request, out string? state, out FreeAgentEnvironment environment)
    {
        state = null;
        environment = FreeAgentEnvironment.Production;

        if (!request.Cookies.TryGetValue(CookieName, out var json) || string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            var payload = JsonSerializer.Deserialize<PersistedOAuthPendingState>(json, JsonOptions);
            if (payload is null || string.IsNullOrWhiteSpace(payload.State))
            {
                return false;
            }

            if (DateTimeOffset.UtcNow - payload.StoredAtUtc.ToUniversalTime() > CookieLifetime)
            {
                return false;
            }

            state = payload.State;
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

    private sealed record PersistedOAuthPendingState(
        string State,
        FreeAgentEnvironment Environment,
        DateTimeOffset StoredAtUtc);
}
