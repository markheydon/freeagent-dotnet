using System.Text.Json;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// OAuth application credentials for the console sample.
/// </summary>
/// <remarks>
/// These are <em>not</em> your FreeAgent login details. They identify your registered
/// OAuth application (client ID + secret) from the FreeAgent developer dashboard.
/// </remarks>
internal sealed class AppSettings
{
    /// <summary>OAuth client identifier from the FreeAgent developer dashboard.</summary>
    public string ClientId { get; init; } = string.Empty;

    /// <summary>OAuth client secret from the FreeAgent developer dashboard.</summary>
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// Redirect URI registered for this OAuth app. Must match exactly when authorising.
    /// The console sample defaults to a local HTTP listener; the Blazor sample uses HTTPS localhost.
    /// </summary>
    public string RedirectUri { get; init; } = "http://127.0.0.1:8765/callback";

    /// <summary>
    /// Loads settings with later sources overriding earlier ones:
    /// appsettings.json → appsettings.local.json → user-secrets → environment variables.
    /// </summary>
    public static AppSettings Load()
    {
        var settings = LoadFromJsonFile("appsettings.json")
            ?? new AppSettings();

        var local = LoadFromJsonFile("appsettings.local.json");
        if (local is not null)
        {
            settings = settings.Merge(local);
        }

        // Same user-secrets ID as the Blazor sample — no need to configure twice.
        var userSecrets = UserSecretsConfiguration.Load();
        if (userSecrets is not null)
        {
            settings = settings.Merge(userSecrets);
        }

        settings = settings.WithEnvironmentOverrides();
        settings.Validate();
        return settings;
    }

    private static AppSettings? LoadFromJsonFile(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);
        if (!document.RootElement.TryGetProperty("FreeAgent", out var section))
        {
            return null;
        }

        return new AppSettings
        {
            ClientId = section.GetPropertyOrDefault(nameof(ClientId)),
            ClientSecret = section.GetPropertyOrDefault(nameof(ClientSecret)),
            RedirectUri = section.GetPropertyOrDefault(nameof(RedirectUri), "http://127.0.0.1:8765/callback"),
        };
    }

    private AppSettings Merge(AppSettings other) =>
        new()
        {
            ClientId = Prefer(other.ClientId, ClientId),
            ClientSecret = Prefer(other.ClientSecret, ClientSecret),
            RedirectUri = Prefer(other.RedirectUri, RedirectUri),
        };

    private AppSettings WithEnvironmentOverrides() =>
        new()
        {
            ClientId = Prefer(Environment.GetEnvironmentVariable("FREEAGENT_CLIENT_ID"), ClientId),
            ClientSecret = Prefer(Environment.GetEnvironmentVariable("FREEAGENT_CLIENT_SECRET"), ClientSecret),
            RedirectUri = Prefer(Environment.GetEnvironmentVariable("FREEAGENT_REDIRECT_URI"), RedirectUri),
        };

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientId)
            || string.IsNullOrWhiteSpace(ClientSecret)
            || string.IsNullOrWhiteSpace(RedirectUri))
        {
            throw new InvalidOperationException(
                "OAuth credentials are missing. Use dotnet user-secrets (see README), appsettings.local.json, " +
                "or FREEAGENT_CLIENT_ID / FREEAGENT_CLIENT_SECRET environment variables.");
        }

        if (!Uri.TryCreate(RedirectUri, UriKind.Absolute, out var redirectUri)
            || redirectUri.Scheme is not ("http" or "https"))
        {
            throw new InvalidOperationException("FreeAgent:RedirectUri must be an absolute http or https URL.");
        }
    }

    private static string Prefer(string? preferred, string fallback) =>
        string.IsNullOrWhiteSpace(preferred) ? fallback : preferred.Trim();
}
