using System.Text.Json;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Reads <c>dotnet user-secrets</c> values without pulling in Microsoft.Extensions.Configuration.
/// </summary>
/// <remarks>
/// <para>
/// User secrets store developer credentials outside the repository (under your user profile).
/// Keys use the same colon syntax as ASP.NET configuration, e.g. <c>FreeAgent:ClientId</c>.
/// </para>
/// <para>
/// We share <see cref="UserSecretsId"/> with <c>FreeAgent.Client.BlazorSample</c> so secrets set
/// for the Blazor workbench also work here. Run from either project directory:
/// <c>dotnet user-secrets set "FreeAgent:ClientId" "..."</c>
/// </para>
/// </remarks>
internal static class UserSecretsConfiguration
{
    /// <summary>
    /// Shared with <c>FreeAgent.Client.BlazorSample</c> so the same <c>dotnet user-secrets</c> values work in both apps.
    /// </summary>
    public const string UserSecretsId = "freeagent-client-sample-v1";

    public static AppSettings? Load()
    {
        var secretsPath = ResolveSecretsPath();
        if (secretsPath is null)
        {
            return null;
        }

        using var stream = File.OpenRead(secretsPath);
        using var document = JsonDocument.Parse(stream);

        return new AppSettings
        {
            ClientId = document.RootElement.GetPropertyOrDefault("FreeAgent:ClientId"),
            ClientSecret = document.RootElement.GetPropertyOrDefault("FreeAgent:ClientSecret"),
            RedirectUri = document.RootElement.GetPropertyOrDefault("FreeAgent:RedirectUri"),
        };
    }

    private static string? ResolveSecretsPath()
    {
        // Standard user-secrets location written by the .NET SDK.
        var root = OperatingSystem.IsWindows()
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft",
                "UserSecrets")
            : Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".microsoft",
                "usersecrets");

        var path = Path.Combine(root, UserSecretsId, "secrets.json");
        return File.Exists(path) ? path : null;
    }
}
