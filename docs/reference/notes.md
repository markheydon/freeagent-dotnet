---
title: Notes
parent: API coverage
nav_order: 9
---

# Notes

List, create, update, and delete notes on contacts or projects. List and create operations scope the parent via `contact` or `project` query parameters.

| | |
|---|---|
| **SDK service** | `client.Notes` (`NoteService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/notes](https://dev.freeagent.com/docs/notes) |
| **Sample app** | `/notes` (list), `/notes/detail` (CRUD) |

Parent links and hydration: see [Linked resources](../explanation/linked-resources.md). Related: [Contacts](contacts.md), [Projects](projects.md).

## Methods

### ListContactNotesAsync

```csharp
Task<IReadOnlyList<Note>> ListContactNotesAsync(
    ContactReference? contact = null,
    long? contactId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `contact` | `ContactReference?` | One of `contact` / `contactId` | `null` | `contact` (URI) |
| `contactId` | `long?` | One of `contact` / `contactId` | `null` | `contact` (URI from `client.Urls.Contact(id)`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/notes?contact=:contact`

**Client validation:** supply `contact` **or** `contactId`, not both.

**Sample:**

```csharp
var notes = await client.Notes.ListContactNotesAsync(contactId: 42);
```

---

### ListProjectNotesAsync

```csharp
Task<IReadOnlyList<Note>> ListProjectNotesAsync(
    ProjectReference? project = null,
    long? projectId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `project` | `ProjectReference?` | One of `project` / `projectId` | `null` | `project` (URI) |
| `projectId` | `long?` | One of `project` / `projectId` | `null` | `project` (URI from `client.Urls.Project(id)`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/notes?project=:project`

**Client validation:** supply `project` **or** `projectId`, not both.

**Sample:**

```csharp
var notes = await client.Notes.ListProjectNotesAsync(projectId: 7);
```

---

### GetNoteAsync

```csharp
Task<Note> GetNoteAsync(long noteId, CancellationToken cancellationToken = default)

Task<Note> GetNoteAsync(
    long noteId,
    NoteGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `noteId` | `long` | Yes | - | path `:id` |
| `options` | `NoteGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/notes/:id`

When `options.IncludeParentContact` or `options.IncludeParentProject` is `true`, the SDK fetches the parent resource if the note response contains only a `parent_url` URI.

**Sample:**

```csharp
var note = await client.Notes.GetNoteAsync(
    123,
    new NoteGetOptions { IncludeParentProject = true });
```

---

### CreateContactNoteAsync

```csharp
Task<Note> CreateContactNoteAsync(
    long contactId,
    CreateContactNoteRequest request,
    CancellationToken cancellationToken = default)

Task<Note> CreateContactNoteAsync(
    ContactReference contact,
    CreateContactNoteRequest request,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `contactId` / `contact` | `long` / `ContactReference` | Yes | - | `contact` query param (URI) |
| `request` | `CreateContactNoteRequest` | Yes | - | `note` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/notes?contact=:contact`

**Sample:**

```csharp
using FreeAgent.Client.Models.Notes;

var note = await client.Notes.CreateContactNoteAsync(
    42,
    CreateContactNoteRequest.Create("Follow up next week"));
```

---

### CreateProjectNoteAsync

```csharp
Task<Note> CreateProjectNoteAsync(
    long projectId,
    CreateProjectNoteRequest request,
    CancellationToken cancellationToken = default)

Task<Note> CreateProjectNoteAsync(
    ProjectReference project,
    CreateProjectNoteRequest request,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `projectId` / `project` | `long` / `ProjectReference` | Yes | - | `project` query param (URI) |
| `request` | `CreateProjectNoteRequest` | Yes | - | `note` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/notes?project=:project`

**Sample:**

```csharp
var note = await client.Notes.CreateProjectNoteAsync(
    7,
    CreateProjectNoteRequest.Create("Kick-off meeting completed"));
```

---

### UpdateNoteAsync

```csharp
Task<Note> UpdateNoteAsync(
    long noteId,
    UpdateNoteRequest request,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `noteId` | `long` | Yes | - | path `:id` |
| `request` | `UpdateNoteRequest` | Yes | - | `note` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/notes/:id`

**Sample:**

```csharp
var updated = await client.Notes.UpdateNoteAsync(
    123,
    UpdateNoteRequest.Create("Revised note text"));
```

---

### DeleteNoteAsync

```csharp
Task DeleteNoteAsync(long noteId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `noteId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/notes/:id`

## NoteGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeParentContact` | `bool` | Fetch parent contact when `parent_url` refers to a contact |
| `IncludeParentProject` | `bool` | Fetch parent project when `parent_url` refers to a project |

## Note model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Content` | `string?` | `note` | Required on create/update |
| `Author` | `string?` | `author` | Read-only |
| `ContactId` | `long?` | - | Parsed from `parent_url` when parent is a contact |
| `ProjectId` | `long?` | - | Parsed from `parent_url` when parent is a project |
| `Contact` | `Contact?` | - | Hydrated parent contact |
| `Project` | `Project?` | - | Hydrated parent project |
| `CreatedAt` | `DateTimeOffset?` | `created_at` | |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` | |
| `Url` | `string` | `url` | |

## NoteReference

Use `client.Urls.Note(id)` or `NoteReference.ForEnvironment(environment, id)` to build environment-correct note URIs.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
