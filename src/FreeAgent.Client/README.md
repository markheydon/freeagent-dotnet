# FreeAgent.Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

> **Prerelease software.** This package is in alpha. Public APIs may change between releases. As of September 2026 this library is in active development and I am aiming to have full FreeAgent API coverage over coming weeks and months. See [versioning policy](https://github.com/markheydon/freeagent-dotnet/blob/main/VERSIONING.md).

## Features

- OAuth 2.0 protocol helpers with automatic token refresh.
- Rate limiting and bounded retries for transient failures.
- Typed exception model (`FreeAgentApiException`, `FreeAgentRateLimitException`, …).
- Pagination (single-page and auto-pagination).
- Company, Contacts, Categories, Users, Projects, Tasks, Timeslips, and Email addresses API support.
- `CurrencyCode` enum for documented ISO 4217 codes (reference type; not a REST resource).
- Targets .NET 8.0 and .NET 10.0.
- Fully async/await with XML documentation.

## Installation

```bash
dotnet add package FreeAgent.Client
```

## Quick start

```csharp
using FreeAgent.Client;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Tasks;

// Sandbox or Production - use the same value for OAuth and API calls.
var environment = FreeAgentEnvironment.Sandbox;
var oauthClient = new FreeAgentOAuthClient(clientId, clientSecret, redirectUri, environment);
var authUrl = oauthClient.GetAuthorizationUrl(state: "optional-state");

// Redirect the user to authUrl, then exchange the callback code:
var token = await oauthClient.ExchangeCodeForTokenAsync(code);

using var client = new FreeAgentClient(oauthClient, token, environment);
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

The SDK provides protocol-level OAuth utilities only, your application owns callback endpoints and browser flows.

## Linked resources

FreeAgent links resources with URI strings. The SDK types those links so you do not parse URLs or hard-code API hosts:

```csharp
// Create a project for a known contact ID (no manual URI construction)
await client.Projects.CreateProjectAsync(new Project
{
    Name = "Website redesign",
    BillingContact = client.Urls.Contact(42),
    Status = ProjectStatus.Active
});

// Read full billing contact fields when you need them
var project = await client.Projects.GetProjectAsync(
    123,
    new ProjectGetOptions { IncludeBillingContact = true });
var organisation = project.Contact!.OrganisationName;

// Display name only - one HTTP call
var summary = await client.Projects.GetProjectAsync(123);
var label = summary.ContactName;

// Create a task under a project (parent scoped via query param)
await client.Tasks.CreateTaskAsync(123, new Task
{
    Name = "Planning",
    IsBillable = true,
    Status = TaskStatus.Active
});

// Log time against a task
using FreeAgent.Client.Models.Timeslips;

await client.Timeslips.CreateTimeslipAsync(new Timeslip
{
    LinkedTask = client.Urls.Task(456),
    LinkedProject = client.Urls.Project(123),
    LinkedUser = client.Urls.User(1),
    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
    Hours = 1.5m
});
```

See [linked resources](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/explanation/linked-resources.md) for the full pattern.

## Documentation

Published documentation: [markheydon.me.uk/freeagent-dotnet](https://markheydon.me.uk/freeagent-dotnet/)

- [Getting started](https://markheydon.me.uk/freeagent-dotnet/tutorial/getting-started)
- [Pagination](https://markheydon.me.uk/freeagent-dotnet/how-to/pagination)
- [Error handling](https://markheydon.me.uk/freeagent-dotnet/how-to/error-handling)
- [API coverage](https://markheydon.me.uk/freeagent-dotnet/reference) - SDK reference for every implemented resource

## Licence

MIT - see [LICENSE](https://github.com/markheydon/freeagent-dotnet/blob/main/LICENSE).
