# FreeAgent.Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

> **Prerelease software.** This package is in alpha. Public APIs may change between releases. Pin an exact version if stability matters. See [versioning policy](https://github.com/markheydon/freeagent-dotnet/blob/main/VERSIONING.md).

## Features

- OAuth 2.0 protocol helpers with automatic token refresh
- Rate limiting and bounded retries for transient failures
- Typed exception model (`FreeAgentApiException`, `FreeAgentRateLimitException`, …)
- Pagination (single-page and auto-pagination)
- Company, Contacts, and Categories API support
- `CurrencyCode` enum for documented ISO 4217 codes (reference type; not a REST resource)
- Targets .NET 8.0 and .NET 10.0
- Fully async/await with XML documentation

## Installation

```bash
dotnet add package FreeAgent.Client
```

For alpha builds, specify the version explicitly:

```bash
dotnet add package FreeAgent.Client --version 0.1.0-alpha.1
```

## Quick start

```csharp
using FreeAgent.Client;

var environment = FreeAgentEnvironment.Sandbox; // or Production — use the same value for OAuth and API calls
var oauthClient = new FreeAgentOAuthClient(clientId, clientSecret, redirectUri, environment);
var authUrl = oauthClient.GetAuthorizationUrl(state: "optional-state");
// Redirect the user to authUrl, then exchange the callback code:
var token = await oauthClient.ExchangeCodeForTokenAsync(code);

using var client = new FreeAgentClient(oauthClient, token, environment);
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

The SDK provides protocol-level OAuth utilities only — your application owns callback endpoints and browser flows.

## Documentation

Full consumer documentation lives in the repository:

- [Getting started](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/tutorial/getting-started.md)
- [Pagination](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/how-to/pagination.md)
- [Error handling](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/how-to/error-handling.md)
- [API coverage](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/reference/api-coverage.md)

## Licence

MIT — see [LICENSE](https://github.com/markheydon/freeagent-dotnet/blob/main/LICENSE).
