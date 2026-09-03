# Getting started

This tutorial walks through installing the SDK, obtaining an access token, and making your first API call.

## Prerequisites

- .NET 8.0 or .NET 10.0
- A FreeAgent developer application with OAuth credentials ([FreeAgent developer portal](https://dev.freeagent.com))

## Install the package

```bash
dotnet add package FreeAgent.Client
```

> The package is currently in **alpha** prerelease. Pin an exact version if stability matters. See [Upgrading](../how-to/upgrading.md) and [Prerelease and OAuth scope](../explanation/prerelease-and-oauth.md).

## OAuth protocol helpers

The SDK provides protocol-level OAuth utilities (`FreeAgentOAuthClient`): authorisation URL construction, code exchange, and token refresh. It does **not** host callback endpoints or orchestrate browser flows — your application owns that.

Both `FreeAgentOAuthClient` and `FreeAgentClient` default to **production**. Pass `FreeAgentEnvironment.Sandbox` on **both** when you are using a sandbox application — otherwise authorisation and API calls go to production.

```csharp
using FreeAgent.Client;

var environment = FreeAgentEnvironment.Sandbox; // or FreeAgentEnvironment.Production

var oauthClient = new FreeAgentOAuthClient(
    clientId: "your-client-id",
    clientSecret: "your-client-secret",
    redirectUri: "https://localhost:5001/callback",
    environment);

// Redirect the user to FreeAgent
var authUrl = oauthClient.GetAuthorizationUrl(state: "optional-csrf-state");

// In your callback handler, exchange the code for tokens
var token = await oauthClient.ExchangeCodeForTokenAsync(authorizationCode);
```

## Create a client and call the API

```csharp
using var client = new FreeAgentClient(token.AccessToken, environment);

var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

For automatic token refresh within a session:

```csharp
using var client = new FreeAgentClient(oauthClient, token, environment);
var company = await client.Company.GetCompanyAsync();
```

Always dispose the client when finished (`using` recommended).

## Next steps

- [Pagination](../how-to/pagination.md) — list contacts and other paginated resources
- [Error handling](../how-to/error-handling.md) — catch typed exceptions
- [API coverage](../reference/api-coverage.md) — see what is implemented today
- [Sample app](../../samples/README.md) — interactive Blazor workbench (contributors)
