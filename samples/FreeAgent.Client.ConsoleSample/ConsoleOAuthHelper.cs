using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Sample-only helper for the OAuth "redirect capture" step.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent.Client SDK does <strong>not</strong> include this class — it is specific to
/// console/CLI apps. Web apps typically add a callback route (see the Blazor sample's
/// <c>/oauth/callback</c> endpoint). Mobile apps often use a custom URL scheme.
/// </para>
/// <para>
/// OAuth 2.0 authorisation code flow (simplified):
/// <list type="number">
/// <item>App builds an authorisation URL via <see cref="FreeAgentOAuthClient.GetAuthorizationUrl"/>.</item>
/// <item>User logs in and approves access in the browser.</item>
/// <item>FreeAgent redirects to the registered <c>redirect_uri</c> with <c>?code=...</c>.</item>
/// <item>App exchanges the code for tokens via <see cref="FreeAgentOAuthClient.ExchangeCodeForTokenAsync"/>.</item>
/// </list>
/// This helper automates step 3 for a desktop/console scenario by running a temporary
/// <see cref="HttpListener"/> on the redirect URI.
/// </para>
/// </remarks>
internal static class ConsoleOAuthHelper
{
    private static readonly TimeSpan CallbackTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Runs the browser-based authorisation step and returns the authorisation code.
    /// </summary>
    /// <param name="oauthClient">Configured OAuth client (used to build the authorisation URL).</param>
    /// <param name="redirectUri">Must match the URI registered in the FreeAgent developer dashboard.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Short-lived authorisation code to exchange for access/refresh tokens.</returns>
    public static async Task<string> GetAuthorizationCodeAsync(
        FreeAgentOAuthClient oauthClient,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        // "state" is a random value you generate and later verify on callback.
        // It helps prevent CSRF attacks: an attacker cannot forge a callback without knowing state.
        var state = Guid.NewGuid().ToString("N");
        var authorizationUrl = oauthClient.GetAuthorizationUrl(state);

        Console.WriteLine();
        Console.WriteLine("Step 1 — open this URL in your browser and approve access:");
        Console.WriteLine();
        Console.WriteLine(authorizationUrl);
        Console.WriteLine();
        Console.WriteLine(
            "Tip: on WSL, copy-paste this URL manually and paste the full redirect URL back — " +
            "auto-capture usually does not work across the WSL/Windows network boundary.");

        // Start the listener before opening the browser so a fast redirect is not missed.
        if (TryCreateListener(redirectUri, out var listener))
        {
            try
            {
                Console.WriteLine($"Step 2 — waiting for FreeAgent to redirect to {redirectUri}");
                Console.WriteLine("(A local page will confirm when authorisation completes.)");
                Console.WriteLine();

                TryOpenBrowser(authorizationUrl);

                return await WaitForBrowserCallbackAsync(listener, state, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not capture the browser callback automatically: {ex.Message}");
                Console.WriteLine("You can paste the redirect URL manually instead.");
                Console.WriteLine();
            }
            finally
            {
                listener.Stop();
                listener.Close();
            }
        }
        else
        {
            Console.WriteLine("Could not start a local callback listener on the redirect URI.");
            Console.WriteLine("Paste the full redirect URL from your browser after you approve access.");
            Console.WriteLine();

            TryOpenBrowser(authorizationUrl);
        }

        return await PromptForAuthorizationCodeAsync(state, cancellationToken);
    }

    private static async Task<string> WaitForBrowserCallbackAsync(
        HttpListener listener,
        string expectedState,
        CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(CallbackTimeout);

        // Blocks until FreeAgent redirects the browser to our redirect URI.
        var context = await listener.GetContextAsync().WaitAsync(timeout.Token);
        var request = context.Request;

        // Show a friendly page so the user knows they can return to the console.
        var responseHtml = """
            <html>
              <body style="font-family: sans-serif; margin: 2rem;">
                <h1>Authorisation complete</h1>
                <p>Return to the console application to continue.</p>
              </body>
            </html>
            """;
        var responseBytes = Encoding.UTF8.GetBytes(responseHtml);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength64 = responseBytes.Length;
        await context.Response.OutputStream.WriteAsync(responseBytes, cancellationToken);
        context.Response.Close();

        return ExtractAuthorizationCode(ParseQueryParameters(request.QueryString), expectedState);
    }

    private static async Task<string> PromptForAuthorizationCodeAsync(
        string expectedState,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Paste the full redirect URL (including ?code=...&state=...) and press Enter:");
        var input = await ReadLineAsync(cancellationToken);
        return ParseAuthorizationCode(input, expectedState);
    }

    private static string ParseAuthorizationCode(string input, string expectedState)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new InvalidOperationException("No authorisation response was provided.");
        }

        var query = ExtractQueryFromRedirectUrl(input.Trim());
        return ExtractAuthorizationCode(ParseQueryParameters(query), expectedState);
    }

    private static string ExtractQueryFromRedirectUrl(string redirectUrl)
    {
        if (!redirectUrl.Contains('?', StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Paste the full redirect URL from your browser (including ?code=...&state=...).");
        }

        var withoutFragment = redirectUrl;
        var fragmentIndex = withoutFragment.IndexOf('#', StringComparison.Ordinal);
        if (fragmentIndex >= 0)
        {
            withoutFragment = withoutFragment[..fragmentIndex];
        }

        return withoutFragment[(withoutFragment.IndexOf('?', StringComparison.Ordinal) + 1)..];
    }

    private static Dictionary<string, string> ParseQueryParameters(string query)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = part.Split('=', 2);
            if (pair.Length != 2)
            {
                continue;
            }

            values[Uri.UnescapeDataString(pair[0])] = Uri.UnescapeDataString(pair[1]);
        }

        return values;
    }

    private static Dictionary<string, string> ParseQueryParameters(NameValueCollection queryString)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var key in queryString.AllKeys)
        {
            if (key is null)
            {
                continue;
            }

            values[key] = queryString[key] ?? string.Empty;
        }

        return values;
    }

    private static string ExtractAuthorizationCode(
        Dictionary<string, string> values,
        string expectedState)
    {
        if (values.TryGetValue("state", out var state)
            && !string.Equals(state, expectedState, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("OAuth state mismatch. Start the sample again and retry.");
        }

        if (values.TryGetValue("error", out var error))
        {
            var description = values.TryGetValue("error_description", out var errorDescription)
                ? errorDescription
                : null;

            throw new InvalidOperationException(
                description is not null
                    ? $"OAuth authorisation failed ({error}): {description}"
                    : $"OAuth authorisation failed: {error}");
        }

        if (!values.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("The callback did not include an authorisation code.");
        }

        return code;
    }

    private static bool TryCreateListener(string redirectUri, out HttpListener listener)
    {
        listener = new HttpListener();

        // HttpListener requires a trailing slash on the prefix, e.g. http://127.0.0.1:8765/callback/
        var prefix = ToListenerPrefix(redirectUri);
        listener.Prefixes.Add(prefix);

        try
        {
            listener.Start();
            return true;
        }
        catch (HttpListenerException)
        {
            // Port in use, permission issue, or invalid prefix — fall back to manual paste.
            listener.Close();
            listener = null!;
            return false;
        }
    }

    private static string ToListenerPrefix(string redirectUri)
    {
        var uri = new Uri(redirectUri);
        var path = uri.AbsolutePath.TrimEnd('/');
        if (string.IsNullOrEmpty(path))
        {
            path = "/";
        }

        return $"{uri.Scheme}://{uri.Authority}{path}/";
    }

    private static void TryOpenBrowser(string url)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return;
            }

            if (OperatingSystem.IsLinux())
            {
                // Under WSL this often forwards to the Windows browser and may mangle long URLs.
                Process.Start("xdg-open", url);
                return;
            }

            if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", url);
            }
        }
        catch
        {
            Console.WriteLine("Could not open a browser automatically. Copy the URL above manually.");
        }
    }

    private static async Task<string> ReadLineAsync(CancellationToken cancellationToken) =>
        await Console.In.ReadLineAsync(cancellationToken) ?? string.Empty;
}
