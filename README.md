# FreeAgent .NET Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

[![CI](https://github.com/markheydon/freeagent-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/markheydon/freeagent-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/FreeAgent.Client.svg)](https://www.nuget.org/packages/FreeAgent.Client/)
[![NuGet (prerelease)](https://img.shields.io/nuget/vpre/FreeAgent.Client.svg?label=nuget%20prerelease)](https://www.nuget.org/packages/FreeAgent.Client/)

> **Prerelease software.** This package is currently in alpha. Public APIs may change between releases. See [VERSIONING.md](VERSIONING.md).

**Documentation:** [docs/](docs/) — tutorials, how-to guides, and API coverage for SDK consumers.

## Features

- OAuth 2.0 protocol helpers with automatic token refresh
- Rate limiting and bounded retries for transient failures
- Typed exception model (`FreeAgentApiException`, `FreeAgentRateLimitException`, …)
- Pagination (single-page and auto-pagination)
- Company, Contacts, and Categories API support
- Targets .NET 8.0 and .NET 10.0
- Fully async/await with XML documentation

## Installation

```bash
dotnet add package FreeAgent.Client
```

## Quick start

```csharp
using FreeAgent.Client;

var environment = FreeAgentEnvironment.Sandbox; // or Production — pass the same value to both constructors
var oauthClient = new FreeAgentOAuthClient(clientId, clientSecret, redirectUri, environment);
var authUrl = oauthClient.GetAuthorizationUrl(state: "optional-state");
// Redirect user to authUrl, then exchange the callback code:
var token = await oauthClient.ExchangeCodeForTokenAsync(code);

using var client = new FreeAgentClient(oauthClient, token, environment);
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

More detail: [Getting started](docs/tutorial/getting-started.md) · [Pagination](docs/how-to/pagination.md) · [Error handling](docs/how-to/error-handling.md) · [API coverage](docs/reference/api-coverage.md)

## Building from source

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

- [SUPPORT.md](SUPPORT.md) — questions and issue routing
- [SECURITY.md](SECURITY.md) — private vulnerability reporting

## Licence

MIT — see [LICENSE](LICENSE).

## Versioning

[VERSIONING.md](VERSIONING.md) — prerelease policy and path to stable `1.0.0`.

## Resources

- [FreeAgent API documentation](https://dev.freeagent.com/docs)
- [Sample app](samples/README.md) — interactive SDK workbench (contributors)
