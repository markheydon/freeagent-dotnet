namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Builds browser-facing FreeAgent URLs for the sample app.
/// </summary>
internal static class FreeAgentWebUrls
{
    public static string GetLoginUrl(FreeAgentEnvironment environment) =>
        environment == FreeAgentEnvironment.Sandbox
            ? "https://login.sandbox.freeagent.com/login"
            : "https://login.freeagent.com/login";

    public static string GetSignupUrl(FreeAgentEnvironment environment) =>
        environment == FreeAgentEnvironment.Sandbox
            ? "https://signup.sandbox.freeagent.com/signup"
            : "https://signup.freeagent.com/signup";

    public static string? GetAccountUrl(FreeAgentEnvironment environment, string? subdomain)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
        {
            return null;
        }

        return environment == FreeAgentEnvironment.Sandbox
            ? $"https://{subdomain}.sandbox.freeagent.com/"
            : $"https://{subdomain}.freeagent.com/";
    }

    public static string GetEnvironmentLabel(FreeAgentEnvironment environment) =>
        environment == FreeAgentEnvironment.Sandbox ? "Sandbox" : "Production";
}
