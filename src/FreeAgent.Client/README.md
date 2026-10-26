# FreeAgent.Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

> **Prerelease software.** This package is in beta. Company FreeAgent API resources documented at [dev.freeagent.com/docs](https://dev.freeagent.com/docs) are implemented in the SDK (excluding the separate Accountancy Practice API). See [versioning policy](https://github.com/markheydon/freeagent-dotnet/blob/main/VERSIONING.md).

## Features

- OAuth 2.0 protocol helpers with automatic token refresh.
- Rate limiting and bounded retries for transient failures.
- Typed exception model (`FreeAgentApiException`, `FreeAgentRateLimitException`, …).
- Pagination (single-page and auto-pagination).
- Company, Contacts, Categories, Users, Projects, Invoices, Estimates, Recurring invoices, Credit notes, Credit note reconciliations, Bills, Bank accounts, Bank feeds, Bank transaction explanations, Bank transactions, Hire purchases, Expenses, Attachments, Stock items, Price list items, Capital assets, Capital asset types, Journal sets, Payroll, Payroll profiles, Properties, CIS bands, CIS settings, Profit and loss, Balance sheet, Trial balance, Cashflow, Transactions, VAT returns, Corporation tax returns, Sales tax periods, EC VAT MOSS sales tax rates, Tasks, Timeslips, Notes, and Email addresses API support.
- `CurrencyCode` enum for documented ISO 4217 codes (reference type; not a REST resource).
- Targets .NET 8.0 and .NET 10.0.
- Fully async/await with XML documentation.

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

API and OAuth traffic use `System.Text.Json` with source-generated metadata. File-based apps do not need `JsonSerializerIsReflectionEnabledByDefault`. Trim and Native AOT publish are tracked under [#173](https://github.com/markheydon/freeagent-dotnet/issues/173). See [getting started — host compatibility](https://markheydon.me.uk/freeagent-dotnet/tutorial/getting-started/#host-compatibility).

## Quick start

Once you have an OAuth access token, calling the API is a few lines:

```csharp
using FreeAgent.Client;

// Access token from your OAuth authorisation flow (do not commit real tokens).
var accessToken = "paste-your-access-token";

// Must match where the token was issued (sandbox app → Sandbox, production app → Production).
var environment = FreeAgentEnvironment.Sandbox;

using var client = new FreeAgentClient(accessToken, environment);
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

**First run from scratch?** Copy-paste runnable scripts (get a sandbox token, then call the API) are in the [repository README Quick start](https://github.com/markheydon/freeagent-dotnet#quick-start). The same files live under `samples/FileBasedQuickStart/` (use `#:project` instead of `#:package` when building from a clone).

The SDK provides protocol-level OAuth utilities (`FreeAgentOAuthClient`); your application owns browser redirects and callback endpoints. See [Getting started](https://markheydon.me.uk/freeagent-dotnet/tutorial/getting-started/).

## Linked resources

FreeAgent links resources with URI strings on the wire. For writes, set `*Id` properties on models (or `CategoryNominalCode` for categories) — the SDK builds environment-correct URIs internally. For reads, use flat properties such as `ContactId`, `ContactName`, and optional hydration.

The following flow chains ids from each response into the next create call:

```csharp
// 1. Contact id from a prior list or create call
const long contactId = 42;

// 2. Create a project for that contact
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
var task = await client.ProjectTasks.CreateProjectTaskAsync(projectId, new ProjectTask
{
    Name = "Planning",
    IsBillable = true,
    Status = ProjectTaskStatus.Active
});
var taskId = task.ResourceId;

// 4. Log time against the task
await client.Timeslips.CreateTimeslipAsync(new Timeslip
{
    ProjectTaskId = taskId,
    ProjectId = projectId,
    UserId = 1,
    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
    Hours = 1.5m
});

// 5. Raise a draft invoice for the contact
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

Further resources that link to contacts and projects include [recurring invoices](https://markheydon.me.uk/freeagent-dotnet/reference/recurring-invoices), [credit notes](https://markheydon.me.uk/freeagent-dotnet/reference/credit-notes), [credit note reconciliations](https://markheydon.me.uk/freeagent-dotnet/reference/credit-note-reconciliations), [bank accounts](https://markheydon.me.uk/freeagent-dotnet/reference/bank-accounts) (optional remittance on invoices and credit notes via `BankAccountId`), [bank feeds](https://markheydon.me.uk/freeagent-dotnet/reference/bank-feeds) (read-only feeds linked to a bank account via `BankAccountId`), [bank transactions](https://markheydon.me.uk/freeagent-dotnet/reference/bank-transactions) (list and statement upload scoped to a bank account), and [notes](https://markheydon.me.uk/freeagent-dotnet/reference/notes).

See [linked resources](https://github.com/markheydon/freeagent-dotnet/blob/main/docs/explanation/linked-resources.md) for the full pattern, including list filters, webhook URL parsing, and update options.

**Replicating data?** Use [incremental sync](https://markheydon.me.uk/freeagent-dotnet/how-to/incremental-sync/) for `ListAutoPagingAsync` watermarks, nested line-item flags (`nestedInvoiceItems`, and similar), and per-resource parameter notes.

## Documentation

Published documentation: [markheydon.me.uk/freeagent-dotnet](https://markheydon.me.uk/freeagent-dotnet/)

- [Getting started](https://markheydon.me.uk/freeagent-dotnet/tutorial/getting-started)
- [Pagination](https://markheydon.me.uk/freeagent-dotnet/how-to/pagination)
- [Incremental sync](https://markheydon.me.uk/freeagent-dotnet/how-to/incremental-sync)
- [Error handling](https://markheydon.me.uk/freeagent-dotnet/how-to/error-handling)
- [API coverage](https://markheydon.me.uk/freeagent-dotnet/reference) - SDK reference for every implemented resource

## Licence

MIT - see [LICENSE](https://github.com/markheydon/freeagent-dotnet/blob/main/LICENSE).
