# FreeAgent .NET Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

[![CI](https://github.com/markheydon/freeagent-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/markheydon/freeagent-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/FreeAgent.Client.svg)](https://www.nuget.org/packages/FreeAgent.Client/)
[![NuGet (prerelease)](https://img.shields.io/nuget/vpre/FreeAgent.Client.svg?label=nuget%20prerelease)](https://www.nuget.org/packages/FreeAgent.Client/)

> **Prerelease software.** This package is currently in beta. Breaking changes are avoided where possible; see [VERSIONING.md](VERSIONING.md).

**Documentation:** [markheydon.me.uk/freeagent-dotnet](https://markheydon.me.uk/freeagent-dotnet/) - tutorials, how-to guides, and SDK reference ([source](docs/)).

## Features

- OAuth 2.0 protocol helpers with automatic token refresh
- Rate limiting and bounded retries for transient failures
- Typed exception model (`FreeAgentApiException`, `FreeAgentRateLimitException`, …)
- Pagination (single-page and auto-pagination)
- Company, Contacts, Categories, Users, Projects, Invoices, Estimates, Recurring invoices, Credit notes, Credit note reconciliations, Bills, Bank accounts, Bank feeds, Bank transaction explanations, Bank transactions, Hire purchases, Expenses, Attachments, Stock items, Price list items, Capital assets, Capital asset types, Journal sets, Payroll, Payroll profiles, Properties, Profit and loss, Balance sheet, Trial balance, Cashflow, Transactions, VAT returns, Corporation tax returns, Sales tax periods, EC VAT MOSS sales tax rates, Tasks, Timeslips, Notes, and Email addresses API support
- Targets .NET 8.0 and .NET 10.0
- Fully async/await with XML documentation

## Installation

```bash
dotnet add package FreeAgent.Client
```

## Compatibility

| Host | Status |
|------|--------|
| SDK-style `.csproj` apps (web, worker, console) | **Supported** — recommended path |
| File-based apps (`#:package`, .NET 10+) | **Supported** — source-generated JSON metadata |
| `PublishTrimmed` / aggressive trimming | **Not supported yet** |
| Native AOT publish (`PublishAot`) | **Not supported yet** |

API and OAuth traffic use `System.Text.Json` with source-generated metadata, so [file-based apps](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps) work without `JsonSerializerIsReflectionEnabledByDefault`. Trimmed and Native AOT publish modes are not supported yet; see [#173](https://github.com/markheydon/freeagent-dotnet/issues/173). Details: [Getting started — host compatibility](https://markheydon.me.uk/freeagent-dotnet/tutorial/getting-started/#host-compatibility).

## Quick start

These steps use a [file-based app](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps) (**.NET 10 SDK**). Create a folder, save each script below as a `.cs` file, and run it with `dotnet run <file>.cs`.

Use a **sandbox** OAuth application from the [sandbox developer portal](https://dev.sandbox.freeagent.com). In that app’s settings, register this redirect URI exactly:

`http://127.0.0.1:8765/callback`

The SDK provides OAuth **protocol** helpers only (authorisation URL, code exchange, refresh). Your app must obtain the authorisation `code` from the browser redirect — the first script below does that by asking you to paste the redirect URL (works on WSL and plain Linux without a local callback server).

Canonical copies of these scripts (for cloning this repo) live under [`samples/FileBasedQuickStart/`](samples/FileBasedQuickStart/).

### 1. Get an access token

```bash
export FREEAGENT_CLIENT_ID='your-sandbox-client-id'
export FREEAGENT_CLIENT_SECRET='your-sandbox-client-secret'
```

Save as `get-token.cs` and run `dotnet run get-token.cs`. Open the printed URL, approve access, then paste the full redirect URL from the browser.

```csharp
#:package FreeAgent.Client@0.1.0-beta.1
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
Console.WriteLine("Access token (use for step 2):");
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
```

### 2. Call the API

```bash
export FREEAGENT_ACCESS_TOKEN='paste-token-from-step-1'
```

Save as `company.cs` and run `dotnet run company.cs`.

```csharp
#:package FreeAgent.Client@0.1.0-beta.1
#:property PublishAot=false

using FreeAgent.Client;

var accessToken = Environment.GetEnvironmentVariable("FREEAGENT_ACCESS_TOKEN")
    ?? throw new InvalidOperationException("Set FREEAGENT_ACCESS_TOKEN (complete step 1 first).");

var environment = FreeAgentEnvironment.Sandbox;

using var client = new FreeAgentClient(accessToken, environment);
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

For a normal SDK-style project, run `dotnet add package FreeAgent.Client` and use the same logic in `Program.cs` (omit the `#:` lines). For automatic token refresh, construct `FreeAgentClient` with `FreeAgentOAuthClient` and `OAuthTokenResponse` — see [Getting started](docs/tutorial/getting-started.md).

More detail: [Getting started](docs/tutorial/getting-started.md) · [Pagination](docs/how-to/pagination.md) · [Error handling](docs/how-to/error-handling.md) · [API coverage](docs/reference/api-coverage.md)

## Building from source

**You do not need this to use the library.** Install from NuGet (`dotnet add package FreeAgent.Client`) and follow [Quick start](#quick-start) above.

Clone and build the repository only if you want to **contribute**, run the full test suite locally, or work against the SDK project instead of the published package:

```bash
git clone https://github.com/markheydon/freeagent-dotnet.git
cd freeagent-dotnet
dotnet clean FreeAgent.slnx && \
dotnet restore FreeAgent.slnx && \
dotnet build FreeAgent.slnx --no-restore --configuration Release -warnaserror && \
dotnet test FreeAgent.slnx --no-build --configuration Release
```

Contributor setup (SDK versions, WSL): [docs/contributing-setup.md](docs/contributing-setup.md)

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). By participating, you agree to [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).

## Support and security

- [SUPPORT.md](SUPPORT.md) - questions and issue routing
- [SECURITY.md](SECURITY.md) - private vulnerability reporting

## Licence

MIT - see [LICENSE](LICENSE).

## Versioning

[VERSIONING.md](VERSIONING.md) - prerelease policy and path to stable `1.0.0`.

## Resources

- [FreeAgent API documentation](https://dev.freeagent.com/docs)
- [Sample app](samples/README.md) - interactive SDK workbench (contributors)
