---
title: Tasks
parent: API coverage
nav_order: 6
---

# Tasks

List, create, update, and delete tasks. Tasks belong to a project; create and list-by-project operations scope the parent via a `project` query parameter.

| | |
|---|---|
| **SDK service** | `client.ProjectTasks` (`ProjectTaskService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/tasks](https://dev.freeagent.com/docs/tasks) |
| **Sample app** | `/tasks` (list), `/tasks/detail` (CRUD) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Parent project links and hydration: see [Linked resources](../explanation/linked-resources.md). Related: [Projects](projects.md).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<ProjectTask>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    string? sort = null,
    DateOnly? updatedSince = null,
    ProjectReference? project = null,
    long? projectId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `sort` | `string?` | No | `null` | `sort` |
| `updatedSince` | `DateOnly?` | No | `null` | `updated_since` (`YYYY-MM-DD`) |
| `project` | `ProjectReference?` | No | `null` | `project` (URI) |
| `projectId` | `long?` | No | `null` | `project` (URI from `client.Urls.Project(id)`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/tasks`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `project` **or** `projectId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.Tasks;

var page = await client.ProjectTasks.ListAsync(
    view: ProjectTaskViews.Active,
    projectId: 42,
    updatedSince: new DateOnly(2024, 1, 1));
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<ProjectTask> ListAutoPagingAsync(
    int perPage = 25,
    string? view = null,
    string? sort = null,
    DateOnly? updatedSince = null,
    ProjectReference? project = null,
    long? projectId = null,
    CancellationToken cancellationToken = default)
```

Same query parameters as `ListAsync` except `page`.

---

### GetProjectTaskAsync

```csharp
Task<ProjectTask> GetProjectTaskAsync(long projectTaskId, CancellationToken cancellationToken = default)

Task<ProjectTask> GetProjectTaskAsync(
    long projectTaskId,
    ProjectTaskGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectTaskId` | `long` | Yes | - | path `:id` |
| `options` | `ProjectTaskGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/tasks/:id`

When `options.IncludeProject` is `true`, the SDK fetches the parent project if the task response contains only a project URI.

**Sample:**

```csharp
var task = await client.ProjectTasks.GetProjectTaskAsync(
    123,
    new ProjectTaskGetOptions { IncludeProject = true });
```

---

### CreateProjectTaskAsync

```csharp
Task<ProjectTask> CreateProjectTaskAsync(long projectId, ProjectTask task, CancellationToken cancellationToken = default)

Task<ProjectTask> CreateProjectTaskAsync(ProjectReference project, ProjectTask task, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectId` / `project` | `long` / `ProjectReference` | Yes | - | `project` query param (URI) |
| `task` | `ProjectTask` | Yes | - | `task` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/tasks?project=:project`

The parent project is supplied as a query parameter, not in the request body.

**Sample:**

```csharp
var task = await client.ProjectTasks.CreateProjectTaskAsync(42, new ProjectTask
{
    Name = "Planning",
    IsBillable = true,
    Status = ProjectTaskStatus.Active
});
```

---

### UpdateProjectTaskAsync

```csharp
Task<ProjectTask> UpdateProjectTaskAsync(long projectTaskId, ProjectTask task, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectTaskId` | `long` | Yes | - | path `:id` |
| `task` | `ProjectTask` | Yes | - | `task` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/tasks/:id`

---

### DeleteProjectTaskAsync

```csharp
Task DeleteProjectTaskAsync(long projectTaskId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectTaskId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/tasks/:id`

## List filters

### view (`ProjectTaskViews`)

| Constant | Wire value |
|----------|------------|
| `ProjectTaskViews.All` | `all` |
| `ProjectTaskViews.Active` | `active` |
| `ProjectTaskViews.Completed` | `completed` |
| `ProjectTaskViews.Hidden` | `hidden` |

### sort

`name`, `project`, `billing_rate`, `created_at`, `updated_at` — prefix with `-` for descending.

## ProjectTaskGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeProject` | `bool` | Fetch parent project when only a URI is returned |

## ProjectTask model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Name` | `string?` | `name` | Required on create |
| `Status` | `ProjectTaskStatus?` | `status` | |
| `Currency` | `CurrencyCode?` | `currency` | Read-only (from project) |
| `IsBillable` | `bool?` | `is_billable` | |
| `BillingRate` | `decimal?` | `billing_rate` | |
| `BillingPeriod` | `ProjectTaskBillingPeriod?` | `billing_period` | |
| `ProjectId` | `long?` | - | Parsed from `project` URI |
| `Project` | `Project?` | `project` | Nested or hydrated project |
| `IsDeletable` | `bool?` | `is_deletable` | Single GET only |
| `Url` | `string` | `url` | |

## ProjectTaskReference

Use `client.Urls.ProjectTask(id)` or `ProjectTaskReference.ForEnvironment(environment, id)` to build environment-correct task URIs for list filters and inbound URL parsing. For timeslip writes, set `Timeslip.ProjectTaskId` instead.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
