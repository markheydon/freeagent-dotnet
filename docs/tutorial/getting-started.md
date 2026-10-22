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
dotnet add package FreeAgent.Client --version 0.1.0-beta.1
```

> The package is currently in **beta** prerelease (`0.1.0-beta.1`). Pin an exact version if stability matters. See [Upgrading](../how-to/upgrading.md) and [Prerelease and OAuth scope](../explanation/prerelease-and-oauth.md).

## Host compatibility

| Host | Status |
|------|--------|
| SDK-style `.csproj` apps (web, worker, console) | **Supported** — recommended path |
| File-based apps (`#:package`, .NET 10+) | **Supported** — source-generated JSON metadata |
| `PublishTrimmed` / aggressive trimming | **Not supported yet** |
| Native AOT publish (`PublishAot`) | **Not supported yet** |

The SDK serialises and deserialises API traffic with `System.Text.Json` source generation, so file-based apps work without extra MSBuild properties. Trimmed and Native AOT publish modes are still out of scope during beta; see [#173](https://github.com/markheydon/freeagent-dotnet/issues/173).

### File-based apps (.NET 10+)

[File-based apps](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps) default to reflection-disabled JSON. The client ships source-generated wire metadata, so you do **not** need `JsonSerializerIsReflectionEnabledByDefault`.

```csharp
#:package FreeAgent.Client@0.1.0-beta.1
#:property PublishAot=false

using FreeAgent.Client;

// OAuth setup omitted — use FreeAgentClient once you have an access token.
```

For a repo smoke script that deserialises fixture JSON (and optionally calls the live API when `FREEAGENT_ACCESS_TOKEN` is set), see [samples/FileBasedQuickStart/CompanyDeserializeSmoke.cs](https://github.com/markheydon/freeagent-dotnet/blob/main/samples/FileBasedQuickStart/CompanyDeserializeSmoke.cs). After installing from NuGet, replace `#:project` in that sample with `#:package FreeAgent.Client@<version>`.

### Trimmed and Native AOT publish

`PublishTrimmed` and `PublishAot` are **not supported** for apps using this client during beta. Use a standard SDK-style project without aggressive trim or Native AOT until [#173](https://github.com/markheydon/freeagent-dotnet/issues/173) defines a proven path.

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

A single `using FreeAgent.Client;` covers the client, list options, OAuth types, and all request/response models — you do not need per-resource model namespaces.

```csharp
using FreeAgent.Client;

using var client = new FreeAgentClient(token.AccessToken, environment);

var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");

// Model types such as Contact are in the same namespace — no extra usings:
var page = await client.Contacts.ListAsync();
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
