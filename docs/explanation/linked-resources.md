---
title: Linked resources
parent: Explanation
nav_order: 2
---

# Linked resources in the SDK

FreeAgent links resources with URI strings on the wire. The SDK hides that shape behind flat read properties, `*Id` write properties, optional hydration on single-resource GET, and typed `*Reference` types for list filters and inbound URL parsing.

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
    new ProjectGetOptions { IncludeContact = true });

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
var task = await client.ProjectTasks.GetProjectTaskAsync(
    456,
    new ProjectTaskGetOptions { IncludeProject = true });

var projectName = task.Project!.Name;
```

Timeslips link to task, project, and user resources:

| Property | When it is set |
|----------|----------------|
| `ProjectTaskId`, `ProjectId`, `UserId` | Always when the API returns URI links |
| `ProjectTask`, `Project`, `User` | When the API returns nested objects **or** when you request hydration on single GET |

```csharp
using FreeAgent.Client.Models.Timeslips;

var timeslip = await client.Timeslips.CreateTimeslipAsync(new Timeslip
{
    ProjectTaskId = 1,
    ProjectId = 1,
    UserId = 1,
    DatedOn = new DateOnly(2024, 3, 18),
    Hours = 2.0m
});

var hydrated = await client.Timeslips.GetTimeslipAsync(
    timeslip.ResourceId,
    new TimeslipGetOptions { IncludeProjectTask = true });
var taskName = hydrated.ProjectTask!.Name;
```

`BilledOnInvoiceId` is read-only and is populated when time has been invoiced.

Notes link to a parent contact or project via `parent_url`:

| Property | When it is set |
|----------|----------------|
| `ContactId`, `ProjectId` | Always when the API returns a parent URI |
| `Contact`, `Project` | When you request hydration on single GET |

```csharp
var note = await client.Notes.GetNoteAsync(
    123,
    new NoteGetOptions { IncludeParentContact = true });

var organisation = note.Contact!.OrganisationName;
```

Note create operations scope the parent via `contact` or `project` query parameters, not the request body.

## Write properties

Set `*Id` on models for linked resources. Use `CategoryNominalCode` for category links on line items and price list entries. Do not assign `client.Urls` or `*Reference` types when creating or updating resources.

```csharp
await client.Projects.CreateProjectAsync(new Project
{
    Name = "Website redesign",
    ContactId = 42,
    Status = ProjectStatus.Active
});
```

After a GET, `ContactId` (and other `*Id` properties) round-trip from the response. Change a link by setting the id explicitly; leave it unset on a freshly constructed model when the API allows the field to be omitted.

Task create operations scope the parent project via the `project` query parameter (`CreateProjectTaskAsync`), not the request body. Task updates do not accept a parent project on the wire.

Invoices link to contacts, projects, bank accounts, and line-item categories:

| Property | When it is set |
|----------|----------------|
| `ContactId`, `ContactName`, `Contact` | Contact link on the invoice |
| `ProjectId`, `Project` | Optional project link on the invoice |
| `BankAccountId`, `BankAccount` | Optional remittance bank account |
| `CategoryNominalCode` | Category link on line items |

```csharp
using FreeAgent.Client.Models.Invoices;

var invoice = await client.Invoices.CreateInvoiceAsync(new Invoice
{
    ContactId = 42,
    DatedOn = new DateOnly(2024, 3, 18),
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

When updating an invoice, estimate, or credit note retrieved from the API, the SDK round-trips existing link ids from the read model. Set a link id to `null` to clear it on the wire. To update scalar fields without re-sending linked resources or line items, use update options:

```csharp
await client.Invoices.UpdateInvoiceAsync(
    invoiceId,
    invoice,
    new InvoiceUpdateOptions
    {
        OmitLineItems = true,
        OmitProject = true
    });

await client.Projects.UpdateProjectAsync(
    projectId,
    project,
    new ProjectUpdateOptions { OmitContact = true });
```

`EstimateUpdateOptions` and `CreditNoteUpdateOptions` expose the same `OmitLineItems`, `OmitContact`, and `OmitProject` flags. `InvoiceUpdateOptions` and `CreditNoteUpdateOptions` also support `OmitBankAccount`.

Line item updates require `InvoiceItem.ItemId`. The SDK maps this from the wire `id` attribute or from a line-item `url` when present.

## List filters and inbound URLs

List methods accept typed `*Reference` parameters **or** `long? *Id` sugar parameters — supply one, not both. Use `client.Urls` to build references when you have an id from a webhook payload or external system:

```csharp
// Filter projects by contact
var page = await client.Projects.ListAsync(contactId: 42);

// Parse a webhook contact URI into a typed reference
var contactRef = ContactReference.Parse(webhookContactUri);
var invoices = await client.Invoices.ListAsync(contact: contactRef);
```

`*Reference` types and `client.Urls` are for filters and inbound URL handling, not for write payloads on models.

## Resource identifiers

`IFreeAgentResource.ResourceId` returns `0` when parsing fails. Use `TryGetResourceId()` or `GetResourceId()` when you need a valid identifier for service calls.

Categories are keyed by nominal code on the wire — use `Category.NominalCode` for API calls, not `ResourceId`. For category links on line items, set `CategoryNominalCode` on the item model.

## Optional hydration (`*GetOptions`)

Single-resource GET methods may accept options such as `ProjectGetOptions.IncludeContact`. When `true`, the SDK performs an additional GET to the linked resource **only if** the first response did not already include a nested object.

- Default is `false` (one HTTP call).
- Hydration is explicit and discoverable in IntelliSense — not lazy-loading on property access.
- List methods do **not** auto-hydrate linked resources (that would cause N+1 requests).

## List endpoints and `nested`

Some list endpoints (for example projects) can return nested linked objects when you pass a documented query parameter such as `nested=true`. When present on the wire, nested data populates the same `Contact` read property.

For list pages, prefer `ContactName` unless you genuinely need full linked objects on every row.

## Implementing new resources

When adding URI link fields to a new endpoint:

1. Use `long? *Id` (or `CategoryNominalCode` for categories) on write payloads; use `*Reference` types on list filters and for inbound URL parsing only.
2. Use flat read properties (`Contact?`, `ContactId`, denormalised display fields).
3. Keep `ExpandableField<T>` internal for JSON only.
4. Add `*GetOptions` with `Include*` flags on single GET when consumers commonly need the linked resource.
5. Add `*UpdateOptions` with `OmitLineItems` when line items should be excludable from update payloads.
6. Document the choice in the entity map and API coverage reference.

See [ADR-0011](../../adr/adr-0011-linked-resource-identity-and-expandable-references.md).
