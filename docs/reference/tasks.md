---
title: Tasks
parent: API coverage
nav_order: 6
---

# Tasks

List, create, update, and delete tasks. Tasks belong to a project; create and list-by-project operations scope the parent via a `project` query parameter.

| | |
|---|---|
| **SDK service** | `client.Tasks` (`TaskService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/tasks](https://dev.freeagent.com/docs/tasks) |
| **Sample app** | `/tasks` (list), `/tasks/detail` (CRUD) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Parent project links and hydration: see [Linked resources](../explanation/linked-resources.md). Related: [Projects](projects.md).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Task>> ListAsync(
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

var page = await client.Tasks.ListAsync(
    view: TaskViews.Active,
    projectId: 42,
    updatedSince: new DateOnly(2024, 1, 1));
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Task> ListAutoPagingAsync(
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

### GetTaskAsync

```csharp
Task<Task> GetTaskAsync(long taskId, CancellationToken cancellationToken = default)

Task<Task> GetTaskAsync(
    long taskId,
    TaskGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `taskId` | `long` | Yes | - | path `:id` |
| `options` | `TaskGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/tasks/:id`

When `options.IncludeProject` is `true`, the SDK fetches the parent project if the task response contains only a project URI.

**Sample:**

```csharp
var task = await client.Tasks.GetTaskAsync(
    123,
    new TaskGetOptions { IncludeProject = true });
```

---

### CreateTaskAsync

```csharp
Task<Task> CreateTaskAsync(long projectId, Task task, CancellationToken cancellationToken = default)

Task<Task> CreateTaskAsync(ProjectReference project, Task task, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectId` / `project` | `long` / `ProjectReference` | Yes | - | `project` query param (URI) |
| `task` | `Task` | Yes | - | `task` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/tasks?project=:project`

The parent project is supplied as a query parameter, not in the request body.

**Sample:**

```csharp
var task = await client.Tasks.CreateTaskAsync(42, new Task
{
    Name = "Planning",
    IsBillable = true,
    Status = TaskStatus.Active
});
```

---

### UpdateTaskAsync

```csharp
Task<Task> UpdateTaskAsync(long taskId, Task task, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `taskId` | `long` | Yes | - | path `:id` |
| `task` | `Task` | Yes | - | `task` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/tasks/:id`

---

### DeleteTaskAsync

```csharp
Task DeleteTaskAsync(long taskId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `taskId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/tasks/:id`

## List filters

### view (`TaskViews`)

| Constant | Wire value |
|----------|------------|
| `TaskViews.All` | `all` |
| `TaskViews.Active` | `active` |
| `TaskViews.Completed` | `completed` |
| `TaskViews.Hidden` | `hidden` |

### sort

`name`, `project`, `billing_rate`, `created_at`, `updated_at` — prefix with `-` for descending.

## TaskGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeProject` | `bool` | Fetch parent project when only a URI is returned |

## Task model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Name` | `string?` | `name` | Required on create |
| `Status` | `TaskStatus?` | `status` | |
| `Currency` | `CurrencyCode?` | `currency` | Read-only (from project) |
| `IsBillable` | `bool?` | `is_billable` | |
| `BillingRate` | `decimal?` | `billing_rate` | |
| `BillingPeriod` | `TaskBillingPeriod?` | `billing_period` | |
| `ProjectId` | `long?` | - | Parsed from `project` URI |
| `Project` | `Project?` | `project` | Nested or hydrated project |
| `IsDeletable` | `bool?` | `is_deletable` | Single GET only |
| `Url` | `string` | `url` | |

## TaskReference

Use `client.Urls.Task(id)` or `TaskReference.ForEnvironment(environment, id)` to build environment-correct task URIs for downstream resources such as timeslips.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
