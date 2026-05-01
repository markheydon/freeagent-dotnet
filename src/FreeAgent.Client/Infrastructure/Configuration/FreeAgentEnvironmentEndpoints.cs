namespace FreeAgent.Client.Infrastructure.Configuration;

/// <summary>
/// Provides canonical API and OAuth endpoint URLs for each FreeAgent environment.
/// </summary>
public static class FreeAgentEnvironmentEndpoints
{
    /// <summary>
    /// Gets the REST API base URL for the selected environment.
    /// </summary>
    /// <param name="environment">Target FreeAgent environment.</param>
    /// <returns>API base URL ending with <c>/v2/</c>.</returns>
    public static string GetApiBaseUrl(FreeAgentEnvironment environment) =>
        environment == FreeAgentEnvironment.Sandbox
            ? "https://api.sandbox.freeagent.com/v2/"
            : "https://api.freeagent.com/v2/";

    /// <summary>
    /// Gets the OAuth authorization endpoint for the selected environment.
    /// </summary>
    /// <param name="environment">Target FreeAgent environment.</param>
    /// <returns>OAuth authorization endpoint URL.</returns>
    public static string GetOAuthAuthorizationEndpoint(FreeAgentEnvironment environment) =>
        environment == FreeAgentEnvironment.Sandbox
            ? "https://api.sandbox.freeagent.com/v2/approve_app"
            : "https://api.freeagent.com/v2/approve_app";

    /// <summary>
    /// Gets the OAuth token endpoint for the selected environment.
    /// </summary>
    /// <param name="environment">Target FreeAgent environment.</param>
    /// <returns>OAuth token endpoint URL.</returns>
    public static string GetOAuthTokenEndpoint(FreeAgentEnvironment environment) =>
        environment == FreeAgentEnvironment.Sandbox
            ? "https://api.sandbox.freeagent.com/v2/token_endpoint"
            : "https://api.freeagent.com/v2/token_endpoint";
}
