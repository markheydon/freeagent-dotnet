using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Prints a refresh token after interactive OAuth for CI and local non-interactive setup.
/// </summary>
internal static class RefreshTokenBootstrap
{
    /// <summary>
    /// Writes the refresh token and GitHub Actions secret setup instructions to the console.
    /// </summary>
    /// <param name="token">Token response from the authorisation code exchange.</param>
    public static void WriteInstructions(OAuthTokenResponse token)
    {
        if (string.IsNullOrWhiteSpace(token.RefreshToken))
        {
            throw new InvalidOperationException(
                "FreeAgent did not return a refresh_token. Re-run the authorisation flow and approve access again.");
        }

        Console.WriteLine();
        Console.WriteLine("=== Refresh token (store securely; treat like a password) ===");
        Console.WriteLine();
        Console.WriteLine(token.RefreshToken);
        Console.WriteLine();
        Console.WriteLine("=== Local non-interactive smoke test ===");
        Console.WriteLine();
        Console.WriteLine("  export FREEAGENT_REFRESH_TOKEN=\"<paste refresh token above>\"");
        Console.WriteLine("  dotnet run --project samples/FreeAgent.Client.ConsoleSample -- --run-all");
        Console.WriteLine();
        Console.WriteLine("=== GitHub Actions repository secrets ===");
        Console.WriteLine();
        Console.WriteLine("  gh secret set FREEAGENT_CLIENT_ID --repo markheydon/freeagent-dotnet");
        Console.WriteLine("  gh secret set FREEAGENT_CLIENT_SECRET --repo markheydon/freeagent-dotnet");
        Console.WriteLine("  gh secret set FREEAGENT_REFRESH_TOKEN --repo markheydon/freeagent-dotnet");
        Console.WriteLine();
        Console.WriteLine("Use the same sandbox OAuth client ID and secret already configured in user-secrets.");
        Console.WriteLine("See docs/contributing/ci-sandbox-smoke.md for the full CI setup guide.");
        Console.WriteLine();
    }
}
