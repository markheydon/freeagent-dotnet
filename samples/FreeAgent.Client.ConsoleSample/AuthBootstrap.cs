using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Authenticates the console sample using browser OAuth or non-interactive token environment variables.
/// </summary>
internal static class AuthBootstrap
{
    /// <summary>
    /// Default sandbox environment for the console sample.
    /// </summary>
    public const FreeAgentEnvironment DefaultEnvironment = FreeAgentEnvironment.Sandbox;

    /// <summary>
    /// Obtains an OAuth token for API calls.
    /// </summary>
    /// <param name="settings">Application settings.</param>
    /// <param name="oauthClient">OAuth client.</param>
    /// <param name="runAll">When <see langword="true"/>, browser OAuth is not attempted.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OAuth token response.</returns>
    public static async Task<OAuthTokenResponse> AuthenticateAsync(
        AppSettings settings,
        FreeAgentOAuthClient oauthClient,
        bool runAll,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(settings.RefreshToken))
        {
            Console.WriteLine("Using refresh token from configuration...");
            return await oauthClient.RefreshTokenAsync(settings.RefreshToken, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(settings.AccessToken))
        {
            Console.WriteLine("Using access token from configuration...");
            return new OAuthTokenResponse
            {
                AccessToken = settings.AccessToken,
            };
        }

        if (runAll)
        {
            throw new InvalidOperationException(
                "Non-interactive mode requires FREEAGENT_REFRESH_TOKEN or FREEAGENT_ACCESS_TOKEN.");
        }

        var authorizationCode = await ConsoleOAuthHelper.GetAuthorizationCodeAsync(
            oauthClient,
            settings.RedirectUri,
            cancellationToken);

        Console.WriteLine("Exchanging authorisation code for an access token...");
        return await oauthClient.ExchangeCodeForTokenAsync(authorizationCode, cancellationToken);
    }
}
