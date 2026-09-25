---
title: Timeslips
parent: API coverage
nav_order: 7
---

# Timeslips

List, create, update, and delete timeslips. Log time against tasks with required `task`, `project`, and `user` links on create. Start and stop running timers on individual timeslips.

| | |
|---|---|
| **SDK service** | `client.Timeslips` (`TimeslipService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/timeslips](https://dev.freeagent.com/docs/timeslips) |
| **Sample app** | `/timeslips` (list), `/timeslips/detail` (CRUD + timers) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Linked resources and hydration: see [Linked resources](../explanation/linked-resources.md). Related: [Tasks](tasks.md), [Projects](projects.md), [Users](users.md).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Timeslip>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    bool? nested = null,
    UserReference? user = null,
    long? userId = null,
    ProjectTaskReference? projectTask = null,
    long? projectTaskId = null,
    ProjectReference? project = null,
    long? projectId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `fromDate` | `DateOnly?` | No | `null` | `from_date` (`YYYY-MM-DD`) |
| `toDate` | `DateOnly?` | No | `null` | `to_date` (`YYYY-MM-DD`) |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` (ISO 8601) |
| `nested` | `bool?` | No | `null` | `nested` (`true` / `false`) |
| `user` | `UserReference?` | No | `null` | `user` (URI) |
| `userId` | `long?` | No | `null` | `user` (URI from `client.Urls.User(id)`) |
| `projectTask` | `ProjectTaskReference?` | No | `null` | `task` (URI) |
| `projectTaskId` | `long?` | No | `null` | `task` (URI from `client.Urls.ProjectTask(id)`) |
| `project` | `ProjectReference?` | No | `null` | `project` (URI) |
| `projectId` | `long?` | No | `null` | `project` (URI from `client.Urls.Project(id)`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/timeslips`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply reference **or** `*Id` for each filter, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.Timeslips;

var page = await client.Timeslips.ListAsync(
    view: TimeslipViews.Unbilled,
    fromDate: new DateOnly(2024, 1, 1),
    toDate: new DateOnly(2024, 3, 31),
    projectTaskId: 42,
    nested: true);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Timeslip> ListAutoPagingAsync(
    int perPage = 25,
    string? view = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    bool? nested = null,
    UserReference? user = null,
    long? userId = null,
    ProjectTaskReference? projectTask = null,
    long? projectTaskId = null,
    ProjectReference? project = null,
    long? projectId = null,
    CancellationToken cancellationToken = default)
```

Same query parameters as `ListAsync` except `page`.

---

### GetTimeslipAsync

```csharp
Task<Timeslip> GetTimeslipAsync(long timeslipId, CancellationToken cancellationToken = default)

Task<Timeslip> GetTimeslipAsync(
    long timeslipId,
    TimeslipGetOptions? options,
    CancellationToken cancellationToken = default)

Task<Timeslip> GetTimeslipAsync(
    long timeslipId,
    bool? nested,
    TimeslipGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `timeslipId` | `long` | Yes | - | path `:id` |
| `nested` | `bool?` | No | `null` | `nested` query param |
| `options` | `TimeslipGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/timeslips/:id`

When `options.IncludeProjectTask`, `IncludeProject`, or `IncludeUser` is `true`, the SDK fetches the linked resource if the response contains only a URI.

**Sample:**

```csharp
var timeslip = await client.Timeslips.GetTimeslipAsync(
    25,
    new TimeslipGetOptions
    {
        IncludeProjectTask = true,
        IncludeProject = true,
        IncludeUser = true
    });
```

---

### CreateTimeslipAsync

```csharp
Task<Timeslip> CreateTimeslipAsync(Timeslip timeslip, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `timeslip` | `Timeslip` | Yes | - | `timeslip` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/timeslips`

**Sample:**

```csharp
using FreeAgent.Client.Models.Timeslips;

var timeslip = await client.Timeslips.CreateTimeslipAsync(new Timeslip
{
    ProjectTaskId = 1,
    ProjectId = 1,
    UserId = 1,
    DatedOn = new DateOnly(2024, 3, 18),
    Hours = 1.5m,
    Comment = "Planning session"
});
```

---

### CreateTimeslipsAsync

```csharp
Task<IReadOnlyList<Timeslip>> CreateTimeslipsAsync(
    IReadOnlyList<Timeslip> timeslips,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `timeslips` | `IReadOnlyList<Timeslip>` | Yes | - | `timeslips` array envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/timeslips` (batch create variant)

---

### UpdateTimeslipAsync

```csharp
Task<Timeslip> UpdateTimeslipAsync(
    long timeslipId,
    Timeslip timeslip,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `timeslipId` | `long` | Yes | - | path `:id` |
| `timeslip` | `Timeslip` | Yes | - | `timeslip` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/timeslips/:id`

`billed_on_invoice` is read-only and is not included in write payloads.

---

### DeleteTimeslipAsync

```csharp
Task DeleteTimeslipAsync(long timeslipId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/timeslips/:id`

---

### StartTimerAsync

```csharp
Task<Timeslip> StartTimerAsync(long timeslipId, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/timeslips/:id/timer`

---

### StopTimerAsync

```csharp
Task<Timeslip> StopTimerAsync(long timeslipId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/timeslips/:id/timer`

## List filters

### view (`TimeslipViews`)

| Constant | Wire value |
|----------|------------|
| `TimeslipViews.All` | `all` |
| `TimeslipViews.Unbilled` | `unbilled` |
| `TimeslipViews.Running` | `running` |

## TimeslipGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeProjectTask` | `bool` | Fetch linked task when only a URI is returned |
| `IncludeProject` | `bool` | Fetch linked project when only a URI is returned |
| `IncludeUser` | `bool` | Fetch linked user when only a URI is returned |

## Timeslip model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `ProjectTaskId` | `long?` | `task` | Read: parsed from URI; write: task id |
| `ProjectId` | `long?` | `project` | Read: parsed from URI; write: project id |
| `UserId` | `long?` | `user` | Read: parsed from URI; write: user id |
| `ProjectTask` | `ProjectTask?` | `task` | Nested or hydrated task |
| `Project` | `Project?` | `project` | Nested or hydrated project |
| `User` | `User?` | `user` | Nested or hydrated user |
| `DatedOn` | `DateOnly?` | `dated_on` | Required on create |
| `Hours` | `decimal?` | `hours` | Required on create |
| `Comment` | `string?` | `comment` | |
| `BilledOnInvoiceId` | `long?` | - | Read-only; parsed from `billed_on_invoice` |
| `Timer` | `TimeslipTimer?` | `timer` | Present when a timer is running |
| `CreatedAt` | `DateTimeOffset?` | `created_at` | Read-only |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` | Read-only |
| `Url` | `string` | `url` | |

## Reference types (list filters and inbound URLs)

Use `client.Urls` or `*Reference.Parse` for list filters and webhook URL parsing — not for create or update payloads:

- `client.Urls.ProjectTask(id)` → `ProjectTaskReference`
- `client.Urls.Project(id)` → `ProjectReference`
- `client.Urls.User(id)` → `UserReference`
- `client.Urls.Timeslip(id)` → `TimeslipReference`

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
