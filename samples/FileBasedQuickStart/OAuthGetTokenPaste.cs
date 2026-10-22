#:project ../../src/FreeAgent.Client/FreeAgent.Client.csproj
#:property PublishAot=false

using FreeAgent.Client;

const string redirectUri = "http://127.0.0.1:8765/callback";
var environment = FreeAgentEnvironment.Sandbox;

var clientId = Environment.GetEnvironmentVariable("FREEAGENT_CLIENT_ID")
    ?? throw new InvalidOperationException("Set FREEAGENT_CLIENT_ID to your sandbox OAuth client ID.");
var clientSecret = Environment.GetEnvironmentVariable("FREEAGENT_CLIENT_SECRET")
    ?? throw new InvalidOperationException("Set FREEAGENT_CLIENT_SECRET to your sandbox OAuth client secret.");

var oauthClient = new FreeAgentOAuthClient(clientId, clientSecret, redirectUri, environment);

var state = Guid.NewGuid().ToString("N");
var authUrl = oauthClient.GetAuthorizationUrl(state);

Console.WriteLine("Register this redirect URI on your sandbox OAuth app if you have not already:");
Console.WriteLine(redirectUri);
Console.WriteLine();
Console.WriteLine("Open this URL in a browser, sign in, and approve access:");
Console.WriteLine(authUrl);
Console.WriteLine();
Console.WriteLine("After redirect, paste the FULL address bar URL here (the page may not load; that is fine):");
var pasted = Console.ReadLine() ?? throw new InvalidOperationException("No redirect URL was pasted.");

var code = GetQueryParameter(pasted, "code")
    ?? throw new InvalidOperationException("No ?code= parameter in the pasted URL.");

var token = await oauthClient.ExchangeCodeForTokenAsync(code);

Console.WriteLine();
Console.WriteLine("Access token (use for the next Quick start step):");
Console.WriteLine(token.AccessToken);
Console.WriteLine();
Console.WriteLine("Example:");
Console.WriteLine($"  export FREEAGENT_ACCESS_TOKEN='{token.AccessToken}'");

static string? GetQueryParameter(string url, string name)
{
    var query = url.Contains('?', StringComparison.Ordinal)
        ? url[(url.IndexOf('?', StringComparison.Ordinal) + 1)..]
        : string.Empty;

    var fragment = query.IndexOf('#', StringComparison.Ordinal);
    if (fragment >= 0)
    {
        query = query[..fragment];
    }

    foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
    {
        var pair = part.Split('=', 2);
        if (pair.Length == 2 && pair[0] == name)
        {
            return Uri.UnescapeDataString(pair[1]);
        }
    }

    return null;
}
