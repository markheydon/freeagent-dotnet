---
title: Incremental sync
parent: How-to
nav_order: 5
---

# Incremental sync

Use this guide when you replicate FreeAgent data into your own store (database, cache, or search index). It complements [Pagination](pagination.md) with ingest-specific flags and parameter order for `ListAutoPagingAsync`.

## Pattern

1. Choose a watermark (`updatedSince` where the API supports it, or `fromDate` / `toDate` for date-scoped lists).
2. Stream pages with `ListAutoPagingAsync` and pass a `CancellationToken` so shutdown can abort long runs.
3. Persist the highest `updated_at` (or your chosen cursor) after each successful batch.
4. On the next run, pass that watermark to fetch only changes.

```csharp
static DateTimeOffset? MaxUpdated(DateTimeOffset? cursor, DateTimeOffset? candidate) =>
    candidate is null ? cursor
    : cursor is null || candidate > cursor ? candidate : cursor;

var watermark = await db.GetContactsWatermarkAsync();

await foreach (var contact in client.Contacts.ListAutoPagingAsync(
    perPage: 100,
    updatedSince: watermark,
    cancellationToken: cancellationToken))
{
    await db.UpsertContactAsync(contact, cancellationToken);
    watermark = MaxUpdated(watermark, contact.UpdatedAt);
}

await db.SaveContactsWatermarkAsync(watermark);
```

Rows with a null `UpdatedAt` do not advance the watermark.

`ListAutoPagingAsync` parameter order **varies by resource** (for example `updatedSince` appears after date filters on some services). Use IntelliSense or the matrix below — do not assume one signature fits all resources.

## Nested line items and linked objects

List responses often omit child data unless you pass a documented nested flag:

| Service | Parameter | Wire query | Use when |
|---------|-----------|------------|----------|
| `client.Invoices` | `nestedInvoiceItems: true` | `nested_invoice_items` | You need `Invoice.InvoiceItems` on each list row |
| `client.CreditNotes` | `nestedCreditNoteItems: true` | `nested_credit_note_items` | You need credit note line items on list rows |
| `client.Estimates` | `nestedEstimateItems: true` | `nested_estimate_items` | You need estimate line items on list rows |
| `client.Bills` | `nestedBillItems: true` | `nested_bill_items` | You need bill line items on list rows |
| `client.Projects` | `nested: true` | `nested` | You need nested billing contact objects on list rows |
| `client.Timeslips` | `nested: true` | `nested` | You need nested task/project/user objects on list rows |

Without these flags, ingest pipelines that expect line items on every invoice will see empty collections.

## `ListAutoPagingAsync` matrix

Resources that support auto-pagination today. Parameter order matches the SDK method signature (named arguments are recommended).

| Service | `updatedSince` | Type | Other notable parameters |
|---------|----------------|------|---------------------------|
| `Contacts` | Yes | `DateTimeOffset?` | `view`, `sort` |
| `Invoices` | Yes | `DateTimeOffset?` | `view`, `sort`, `contactId` / `projectId`, **`nestedInvoiceItems`** |
| `CreditNotes` | Yes | `DateTimeOffset?` | `view`, `sort`, `contactId` / `projectId`, **`nestedCreditNoteItems`** |
| `Estimates` | Yes | `DateTimeOffset?` | `fromDate`, `toDate` (before `updatedSince`), `contactId` / `projectId` / `invoiceId`, **`nestedEstimateItems`** |
| `Bills` | Yes | `DateTimeOffset?` | `fromDate`, `toDate`, `contactId` / `projectId`, **`nestedBillItems`** |
| `Expenses` | Yes | `DateTimeOffset?` | `fromDate`, `toDate`, `projectId` |
| `Projects` | No | — | `view`, `sort`, `contactId`, **`nested`** |
| `RecurringInvoices` | No | — | `view`, `contactId` |
| `ProjectTasks` | Yes | **`DateOnly?`** (date, not timestamp) | `view`, `sort`, **`projectId`** (scope tasks to a project) |
| `Timeslips` | Yes | `DateTimeOffset?` | `fromDate`, `toDate`, **`nested`**, `userId` / `projectTaskId` / `projectId` |
| `BankTransactions` | Yes | `DateTimeOffset?` | **`bankAccountId`** (required scope), `view`, `fromDate`, `toDate`, `lastUploaded` |

`client.Users.ListAsync` returns all users for a view in one response (not paginated). Use `UserView.ActiveStaff` for the `active_staff` wire filter — there is no separate `UserView.Active` member.

### Billing rates and other derived fields

`ProjectTask.BillingRate` may be null on list or GET even when the project defines `NormalBillingRate`. The SDK does not compute an "effective" rate; join task and project data in your application if you need that rule.

## Nullable reads vs writes

Several properties are nullable because the API omits them or they are optional on write:

- **`ProjectTask.IsBillable`** (`bool?`) — omit on create when the API default applies; set explicitly when you need a known value.
- **`InvoiceItem.Position`** (`decimal?`) — line order from the API on read; optional on write depending on operation.
- **`InvoiceItem.ItemId`** (`long?`) — required when updating existing lines; populated from wire `id` or item `url` on read. See [Linked resources](../explanation/linked-resources.md).

Do not coerce nulls to defaults in the SDK — that would hide wire semantics.

## What this guide does not cover

- Entity Framework, Aspire, or other host package version alignment (the `FreeAgent.Client` package has no NuGet dependencies).
- Business rules (VAT, effective billing rates, accounting policy).
- Reordering `ListAutoPagingAsync` parameters for consistency (would be a breaking API change).

## See also

- [Pagination](pagination.md)
- [Linked resources](../explanation/linked-resources.md) — `ResourceId`, `Url`, and reference types for tests and webhooks
- [API coverage](../reference/) — per-resource list filters and models
