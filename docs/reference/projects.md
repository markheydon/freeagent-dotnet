---
title: Projects
parent: API coverage
nav_order: 5
---

# Projects

List, create, update, and delete projects.

| | |
|---|---|
| **SDK service** | `client.Projects` (`ProjectService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/projects](https://dev.freeagent.com/docs/projects) |
| **Sample app** | `/projects` (list), `/projects/detail` (CRUD) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Billing contact links and hydration: see [Linked resources](../explanation/linked-resources.md). Child tasks: see [Tasks](tasks.md).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Project>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    string? sort = null,
    ContactReference? contact = null,
    long? contactId = null,
    bool? nested = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `sort` | `string?` | No | `null` | `sort` |
| `contact` | `ContactReference?` | No | `null` | `contact` (URI) |
| `contactId` | `long?` | No | `null` | `contact` (URI from `client.Urls.Contact(id)`) |
| `nested` | `bool?` | No | `null` | `nested` (`true` / `false`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/projects`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `contact` **or** `contactId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.Projects;

var page = await client.Projects.ListAsync(
    view: ProjectViews.Active,
    contactId: 42,
    nested: true);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Project> ListAutoPagingAsync(
    int perPage = 25,
    string? view = null,
    string? sort = null,
    ContactReference? contact = null,
    long? contactId = null,
    bool? nested = null,
    CancellationToken cancellationToken = default)
```

Same query parameters as `ListAsync` except `page`.

---

### GetProjectAsync

```csharp
Task<Project> GetProjectAsync(long projectId, CancellationToken cancellationToken = default)

Task<Project> GetProjectAsync(
    long projectId,
    ProjectGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectId` | `long` | Yes | - | path `:id` |
| `options` | `ProjectGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/projects/:id`

When `options.IncludeContact` is `true`, the SDK fetches the billing contact if the project response contains only a contact URI.

**Sample:**

```csharp
var project = await client.Projects.GetProjectAsync(
    123,
    new ProjectGetOptions { IncludeContact = true });
```

---

### CreateProjectAsync

```csharp
Task<Project> CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `project` | `Project` | Yes | - | `project` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/projects`

Set `project.ContactId` to assign a billing contact on create.

---

### UpdateProjectAsync

```csharp
Task<Project> UpdateProjectAsync(long projectId, Project project, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectId` | `long` | Yes | - | path `:id` |
| `project` | `Project` | Yes | - | `project` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/projects/:id`

On update, the SDK round-trips `ContactId` from the read model when present.

---

### DeleteProjectAsync

```csharp
Task DeleteProjectAsync(long projectId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/projects/:id`

## List filters

### view (`ProjectViews`)

| Constant | Wire value |
|----------|------------|
| `ProjectViews.Active` | `active` |
| `ProjectViews.Completed` | `completed` |
| `ProjectViews.Cancelled` | `cancelled` |
| `ProjectViews.Hidden` | `hidden` |

### sort

`name`, `contact_name`, `contact_display_name`, `created_at`, `updated_at` - prefix with `-` for descending.

## ProjectGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeContact` | `bool` | Fetch billing contact when only a URI is returned |

## Project model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Name` | `string?` | `name` | |
| `Status` | `ProjectStatus?` | `status` | |
| `Currency` | `CurrencyCode?` | `currency` | |
| `Budget` | `decimal?` | `budget` | |
| `BudgetUnits` | `ProjectBudgetUnits?` | `budget_units` | |
| `BillingPeriod` | `ProjectBillingPeriod?` | `billing_period` | |
| `ContactId` | `long?` | `contact` | Read: parsed from URI; write: billing contact id |
| `ContactName` | `string?` | `contact_name` | Denormalised display name |
| `Contact` | `Contact?` | `contact` | Nested or hydrated contact |
| `Url` | `string` | `url` | |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
