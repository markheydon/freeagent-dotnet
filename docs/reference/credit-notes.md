---
title: Credit notes
parent: API coverage
nav_order: 8
---

# Credit notes

List, create, update, and delete credit notes; status transitions; email and PDF actions; nested credit note items on the parent resource.

| | |
|---|---|
| **SDK service** | `client.CreditNotes` (`CreditNotesService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/credit_notes](https://dev.freeagent.com/docs/credit_notes) |
| **Sample app** | `/credit-notes` (list), `/credit-notes/detail` (CRUD and transitions) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Contact, project, category, and bank account links: see [Linked resources](../explanation/linked-resources.md). Sales cluster patterns: see [Sales entity map](../explanation/api-entity-map/sales.md#sales-cluster-sdk-patterns).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<CreditNote>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    string? sort = null,
    DateTimeOffset? updatedSince = null,
    ContactReference? contact = null,
    long? contactId = null,
    ProjectReference? project = null,
    long? projectId = null,
    bool? nestedCreditNoteItems = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `sort` | `string?` | No | `null` | `sort` (`created_at`, `updated_at`; prefix `-` for descending) |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` |
| `contact` | `ContactReference?` | No | `null` | `contact` (URI) |
| `contactId` | `long?` | No | `null` | `contact` (URI from `client.Urls.Contact(id)`) |
| `project` | `ProjectReference?` | No | `null` | `project` (URI) |
| `projectId` | `long?` | No | `null` | `project` (URI from `client.Urls.Project(id)`) |
| `nestedCreditNoteItems` | `bool?` | No | `null` | `nested_credit_note_items` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/credit_notes`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `contact` **or** `contactId`, not both; supply `project` **or** `projectId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.CreditNotes;

var page = await client.CreditNotes.ListAsync(
    view: CreditNoteViews.Open,
    contactId: 42,
    nestedCreditNoteItems: true);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<CreditNote> ListAutoPagingAsync(...)
```

Same query parameters as `ListAsync` except `page`.

---

### GetCreditNoteAsync

```csharp
Task<CreditNote> GetCreditNoteAsync(long creditNoteId, CancellationToken cancellationToken = default)

Task<CreditNote> GetCreditNoteAsync(
    long creditNoteId,
    CreditNoteGetOptions? options,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/credit_notes/:id`

When `options.IncludeContact` or `options.IncludeProject` is `true`, the SDK fetches linked resources when the response contains only URIs.

---

### GetCreditNotePdfAsync

```csharp
Task<byte[]> GetCreditNotePdfAsync(long creditNoteId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/credit_notes/:id/pdf`

Decodes the base64 `pdf.content` wrapper returned by FreeAgent.

---

### CreateCreditNoteAsync

```csharp
Task<CreditNote> CreateCreditNoteAsync(CreditNote creditNote, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/credit_notes`

Required on create: `ContactId`, `DatedOn`, `PaymentTermsInDays`, and at least one line item with `Description`. Credit notes are created in `Draft` status.

---

### UpdateCreditNoteAsync

```csharp
Task<CreditNote> UpdateCreditNoteAsync(
    long creditNoteId,
    CreditNote creditNote,
    CreditNoteUpdateOptions? options = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/credit_notes/:id`

Update existing line items with `ItemId`. Delete line items with `ItemId` and `Destroy = 1`. Pass `new CreditNoteUpdateOptions { OmitLineItems = true }` to update scalar fields without sending line items. `ShowProjectName` is sent on update only when `Status` is `Draft` — FreeAgent locks `show_project_name` once a credit note leaves draft.

---

### DeleteCreditNoteAsync

```csharp
Task DeleteCreditNoteAsync(long creditNoteId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/credit_notes/:id`

---

### SendCreditNoteEmailAsync

```csharp
Task SendCreditNoteEmailAsync(
    long creditNoteId,
    SendCreditNoteEmailRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/credit_notes/:id/send_email`

Set `request.Email.UseTemplate = true` to send using an existing template.

---

### MarkCreditNoteAsSentAsync / MarkCreditNoteAsDraftAsync

```csharp
Task<CreditNote> MarkCreditNoteAsSentAsync(long creditNoteId, CancellationToken cancellationToken = default)
Task<CreditNote> MarkCreditNoteAsDraftAsync(long creditNoteId, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/credit_notes/:id/transitions/{action}`

**Behaviour:** The SDK sends `PUT .../transitions/{action}` with an empty JSON body (`{}`), then fetches the updated credit note with `GET .../credit_notes/:id` because FreeAgent transition endpoints return no response body.

---

## List filters

### view (`CreditNoteViews`)

| Constant | Wire value |
|----------|------------|
| `CreditNoteViews.All` | `all` |
| `CreditNoteViews.RecentOpenOrOverdue` | `recent_open_or_overdue` |
| `CreditNoteViews.Open` | `open` |
| `CreditNoteViews.Overdue` | `overdue` |
| `CreditNoteViews.OpenOrOverdue` | `open_or_overdue` |
| `CreditNoteViews.Draft` | `draft` |
| `CreditNoteViews.Refunded` | `refunded` |

Use `last_N_months` (for example `last_3_months`) as a documented view string when filtering by recency.

### sort (`CreditNoteSortOptions`)

| Constant | Wire value |
|----------|------------|
| `CreditNoteSortOptions.CreatedAt` | `created_at` |
| `CreditNoteSortOptions.UpdatedAt` | `updated_at` |

Prefix with `-` for descending order.

## CreditNoteGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeContact` | `bool` | Fetch contact when only a URI is returned |
| `IncludeProject` | `bool` | Fetch project when only a URI is returned |

## CreditNoteUpdateOptions

| Property | Type | Purpose |
|----------|------|---------|
| `OmitLineItems` | `bool` | Exclude line items from the update payload |
| `OmitContact` | `bool` | Exclude the contact link from the update payload |
| `OmitProject` | `bool` | Exclude the project link from the update payload |
| `OmitBankAccount` | `bool` | Exclude the bank account link from the update payload |

## Credit note model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Reference` | `string?` | `reference` | |
| `Status` | `CreditNoteStatus?` | `status` | Read-only on create |
| `DatedOn` | `DateOnly?` | `dated_on` | Required on create |
| `DueOn` | `DateOnly?` | `due_on` | |
| `PaymentTermsInDays` | `int?` | `payment_terms_in_days` | Required on create; `0` for due on receipt |
| `ContactId` | `long?` | `contact` | Required on create |
| `ProjectId` | `long?` | `project` | Optional project link |
| `BankAccountId` | `long?` | `bank_account` | Optional remittance bank account |
| `CreditNoteItems` | `List<CreditNoteItem>?` | `credit_note_items` | Nested line items |
| `ShowProjectName` | `bool?` | `show_project_name` | Writable on create and on draft updates; omitted once the credit note leaves draft |
| `ItemId` | `long?` | `id` (line items) | Required to update or delete existing line items |
| `CategoryNominalCode` | `string?` | `category` (line items) | Category link on line items |
| `RefundedValue` | `decimal?` | `refunded_value` | Read-only |
| `DueValue` | `decimal?` | `due_value` | Read-only |
| `Url` | `string` | `url` | |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
