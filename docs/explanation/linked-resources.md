---
title: Linked resources
parent: Explanation
nav_order: 2
---

# Linked resources in the SDK

FreeAgent links resources with URI strings on the wire. The SDK hides that shape behind typed references, flat read properties, and optional hydration on single-resource GET.

## Read properties

For a link such as `contact` on a project, the public model exposes:

| Property | When it is set |
|----------|----------------|
| `ContactId` | Always when the API returns a contact URI |
| `ContactName` | When the API returns a denormalised display name |
| `Contact` | When the API returns a nested contact object **or** when you request hydration on single GET |

You do not need to parse URLs or know about internal wire adapters.

```csharp
var project = await client.Projects.GetProjectAsync(
    123,
    new ProjectGetOptions { IncludeBillingContact = true });

var organisation = project.Contact!.OrganisationName;
```

For display text only, one HTTP call is enough:

```csharp
var project = await client.Projects.GetProjectAsync(123);
var label = project.ContactName;
```

Tasks follow the same pattern for the parent project link:

| Property | When it is set |
|----------|----------------|
| `ProjectId` | Always when the API returns a project URI |
| `Project` | When the API returns a nested project object **or** when you request hydration on single GET |

```csharp
var task = await client.Tasks.GetTaskAsync(
    456,
    new TaskGetOptions { IncludeProject = true });

var projectName = task.Project!.Name;
```

Timeslips link to task, project, and user resources:

| Property | When it is set |
|----------|----------------|
| `TaskId`, `ProjectId`, `UserId` | Always when the API returns URI links |
| `Task`, `Project`, `User` | When the API returns nested objects **or** when you request hydration on single GET |

```csharp
using FreeAgent.Client.Models.Timeslips;

var timeslip = await client.Timeslips.CreateTimeslipAsync(new Timeslip
{
    LinkedTask = client.Urls.Task(1),
    LinkedProject = client.Urls.Project(1),
    LinkedUser = client.Urls.User(1),
    DatedOn = new DateOnly(2024, 3, 18),
    Hours = 2.0m
});

var hydrated = await client.Timeslips.GetTimeslipAsync(
    timeslip.ResourceId,
    new TimeslipGetOptions { IncludeTask = true });
var taskName = hydrated.Task!.Name;
```

`BilledOnInvoiceId` is read-only and is populated when time has been invoiced.

## Write properties

Assign typed references - never raw URI strings:

```csharp
await client.Projects.CreateProjectAsync(new Project
{
    Name = "Website redesign",
    BillingContact = client.Urls.Contact(42),
    Status = ProjectStatus.Active
});
```

Use `client.Urls` so sandbox and production hosts stay correct.

When updating a project retrieved from the API, the SDK round-trips the existing billing contact link unless you set `OmitBillingContactFromWrite = true`. Assign `BillingContact` explicitly when changing the billing contact.

Task create operations scope the parent project via the `project` query parameter (`CreateTaskAsync`), not the request body. Task updates do not accept a parent project on the wire.

## Resource identifiers

`IFreeAgentResource.ResourceId` returns `0` when parsing fails. Use `TryGetResourceId()` or `GetResourceId()` when you need a valid identifier for service calls.

Categories are keyed by nominal code on the wire - use `Category.NominalCode` for API calls, not `ResourceId`.

## Optional hydration (`*GetOptions`)

Single-resource GET methods may accept options such as `ProjectGetOptions.IncludeBillingContact`. When `true`, the SDK performs an additional GET to the linked resource **only if** the first response did not already include a nested object.

- Default is `false` (one HTTP call).
- Hydration is explicit and discoverable in IntelliSense - not lazy-loading on property access.
- List methods do **not** auto-hydrate linked resources (that would cause N+1 requests).

## List endpoints and `nested`

Some list endpoints (for example projects) can return nested linked objects when you pass a documented query parameter such as `nested=true`. When present on the wire, nested data populates the same `Contact` read property.

For list pages, prefer `ContactName` unless you genuinely need full linked objects on every row.

## Implementing new resources

When adding URI link fields to a new endpoint:

1. Use `*Reference` types on write payloads and list filters.
2. Use flat read properties (`Contact?`, `ContactId`, denormalised display fields).
3. Keep `ExpandableField<T>` internal for JSON only.
4. Add `*GetOptions` with `Include*` flags on single GET when consumers commonly need the linked resource.
5. Document the choice in the entity map and API coverage reference.

See [ADR-0011](../../adr/adr-0011-linked-resource-identity-and-expandable-references.md).
