---
title: Getting started
parent: Tutorial
nav_order: 1
---

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

## Host compatibility

| Host | Status |
|------|--------|
| SDK-style `.csproj` apps (web, worker, console) | **Supported** — recommended path |
| File-based apps (`#:package`, .NET 10+) | **Workaround required** until [#175](https://github.com/markheydon/freeagent-dotnet/issues/175) |
| `PublishTrimmed` / aggressive trimming | **Not supported yet** |
| Native AOT publish (`PublishAot`) | **Not supported yet** |

The SDK serialises and deserialises API traffic with reflection-based `System.Text.Json`. That works in normal SDK-style projects; other host modes need care until [trim-safe JSON](https://github.com/markheydon/freeagent-dotnet/issues/175) ships.

### File-based apps (.NET 10+)

[File-based apps](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps) default to reflection-disabled JSON. Without a workaround, calls that hit JSON (including OAuth token exchange) can fail with `InvalidOperationException` until [#175](https://github.com/markheydon/freeagent-dotnet/issues/175) lands.

Add `#:property JsonSerializerIsReflectionEnabledByDefault=true` before `#:package`:

```csharp
#:property JsonSerializerIsReflectionEnabledByDefault=true
#:package FreeAgent.Client@0.1.0-alpha.5

using FreeAgent.Client;

// OAuth setup omitted — any API call that hits JSON needs the property above until trim-safe JSON ships.
```

This unblocks single-file scripts; it does **not** make the library reliable under Native AOT or full trimming.

### Trimmed and Native AOT publish

`PublishTrimmed` and `PublishAot` are **not supported** for apps using this client during alpha. Use a standard SDK-style project without aggressive trim or Native AOT until [#175](https://github.com/markheydon/freeagent-dotnet/issues/175) and [#173](https://github.com/markheydon/freeagent-dotnet/issues/173) define a proven path.

## OAuth protocol helpers

The SDK provides protocol-level OAuth utilities (`FreeAgentOAuthClient`): authorisation URL construction, code exchange, and token refresh. It does **not** host callback endpoints or orchestrate browser flows - your application owns that.

Both `FreeAgentOAuthClient` and `FreeAgentClient` default to **production**. Pass `FreeAgentEnvironment.Sandbox` on **both** when you are using a sandbox application - otherwise authorisation and API calls go to production.

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

- [Pagination](../how-to/pagination.md) - list contacts and other paginated resources
- [Error handling](../how-to/error-handling.md) - catch typed exceptions
- [API coverage](../reference/api-coverage.md) - browse the SDK reference for every implemented resource
- [Contacts](../reference/contacts.md) - list filters, CRUD methods, and the `Contact` model
- [Sample app](../../samples/README.md) - interactive Blazor workbench (contributors)
