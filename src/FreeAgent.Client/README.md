# FreeAgent.Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

> **Prerelease software.** This package is in alpha. Public APIs may change between releases. As of September 2026 this library is in active development and I am aiming to have full FreeAgent API coverage over coming weeks and months. See [versioning policy](https://github.com/markheydon/freeagent-dotnet/blob/main/VERSIONING.md).

## Features

- OAuth 2.0 protocol helpers with automatic token refresh.
- Rate limiting and bounded retries for transient failures.
- Typed exception model (`FreeAgentApiException`, `FreeAgentRateLimitException`, …).
- Pagination (single-page and auto-pagination).
- Company, Contacts, Categories, Users, Projects, Invoices, Estimates, Recurring invoices, Credit notes, Credit note reconciliations, Stock items, Price list items, Tasks, Timeslips, Notes, and Email addresses API support.
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
using FreeAgent.Client.Models.Invoices;
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

FreeAgent links resources with URI strings on the wire. For writes, set `*Id` properties on models (or `CategoryNominalCode` for categories) — the SDK builds environment-correct URIs internally. For reads, use flat properties such as `ContactId`, `ContactName`, and optional hydration.

The following flow chains ids from each response into the next create call:

```csharp
// 1. Contact id from a prior list or create call
const long contactId = 42;

// 2. Create a project for that contact
using FreeAgent.Client.Models.Projects;

var project = await client.Projects.CreateProjectAsync(new Project
{
    Name = "Website redesign",
    ContactId = contactId,
    Status = ProjectStatus.Active
});
var projectId = project.ResourceId;

// Display name from one GET — no extra HTTP call
var summary = await client.Projects.GetProjectAsync(projectId);
var label = summary.ContactName;

// Full contact fields when you need them
var withContact = await client.Projects.GetProjectAsync(
    projectId,
    new ProjectGetOptions { IncludeContact = true });
var organisation = withContact.Contact!.OrganisationName;

// 3. Create a task under the project (parent scoped via query param)
using FreeAgent.Client.Models.Tasks;

var task = await client.ProjectTasks.CreateProjectTaskAsync(projectId, new ProjectTask
{
    Name = "Planning",
    IsBillable = true,
    Status = ProjectTaskStatus.Active
});
var taskId = task.ResourceId;

// 4. Log time against the task
using FreeAgent.Client.Models.Timeslips;

await client.Timeslips.CreateTimeslipAsync(new Timeslip
{
    ProjectTaskId = projectTaskId,
    ProjectId = projectId,
    UserId = 1,
    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
    Hours = 1.5m
});

// 5. Raise a draft invoice for the contact
using FreeAgent.Client.Models.Invoices;

await client.Invoices.CreateInvoiceAsync(new Invoice
{
    ContactId = contactId,
    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
    PaymentTermsInDays = 14,
    InvoiceItems =
    [
        new InvoiceItem
        {
            Description = "Consulting",
            ItemType = InvoiceItemType.Hours,
            Quantity = 2,
            Price = 100,
            CategoryNominalCode = "001"
        }
    ]
});
```

Further resources that link to contacts and projects include [recurring invoices](https://markheydon.me.uk/freeagent-dotnet/reference/recurring-invoices), [credit notes](https://markheydon.me.uk/freeagent-dotnet/reference/credit-notes), [credit note reconciliations](https://markheydon.me.uk/freeagent-dotnet/reference/credit-note-reconciliations), and [notes](https://markheydon.me.uk/freeagent-dotnet/reference/notes).

See [linked resources](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/explanation/linked-resources.md) for the full pattern, including list filters, webhook URL parsing, and update options.

## Documentation

Published documentation: [markheydon.me.uk/freeagent-dotnet](https://markheydon.me.uk/freeagent-dotnet/)

- [Getting started](https://markheydon.me.uk/freeagent-dotnet/tutorial/getting-started)
- [Pagination](https://markheydon.me.uk/freeagent-dotnet/how-to/pagination)
- [Error handling](https://markheydon.me.uk/freeagent-dotnet/how-to/error-handling)
- [API coverage](https://markheydon.me.uk/freeagent-dotnet/reference) - SDK reference for every implemented resource

## Licence

MIT - see [LICENSE](https://github.com/markheydon/freeagent-dotnet/blob/main/LICENSE).
