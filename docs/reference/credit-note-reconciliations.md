---
title: Credit note reconciliations
parent: API coverage
nav_order: 8.1
---

# Credit note reconciliations

List, create, update, and delete credit note reconciliations that link an invoice to a credit note.

| | |
|---|---|
| **SDK service** | `client.CreditNoteReconciliations` (`CreditNoteReconciliationsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/credit_note_reconciliations](https://dev.freeagent.com/docs/credit_note_reconciliations) |
| **Sample app** | `/credit-note-reconciliations` (list), `/credit-note-reconciliations/detail` (CRUD) |

Invoice and credit note links: see [Linked resources](../explanation/linked-resources.md). Sales cluster patterns: see [Sales entity map](../explanation/api-entity-map/sales.md#credit-note-reconciliation). Related: [Invoices](invoices.md), [Credit notes](credit-notes.md).

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<CreditNoteReconciliation>> ListAsync(
    DateTimeOffset? updatedSince = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` |
| `fromDate` | `DateOnly?` | No | `null` | `from_date` |
| `toDate` | `DateOnly?` | No | `null` | `to_date` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/credit_note_reconciliations`

This endpoint returns the full collection in one response (no pagination parameters).

**Sample:**

```csharp
var reconciliations = await client.CreditNoteReconciliations.ListAsync(
    fromDate: new DateOnly(2024, 1, 1));
```

---

### GetCreditNoteReconciliationAsync

```csharp
Task<CreditNoteReconciliation> GetCreditNoteReconciliationAsync(
    long creditNoteReconciliationId,
    CancellationToken cancellationToken = default)

Task<CreditNoteReconciliation> GetCreditNoteReconciliationAsync(
    long creditNoteReconciliationId,
    CreditNoteReconciliationGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `creditNoteReconciliationId` | `long` | Yes | - | path `:id` |
| `options` | `CreditNoteReconciliationGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/credit_note_reconciliations/:id`

When `options.IncludeInvoice` or `options.IncludeCreditNote` is `true`, the SDK fetches the linked resource if the response contains only a URI.

**Sample:**

```csharp
var reconciliation = await client.CreditNoteReconciliations.GetCreditNoteReconciliationAsync(
    123,
    new CreditNoteReconciliationGetOptions
    {
        IncludeInvoice = true,
        IncludeCreditNote = true
    });
```

---

### CreateCreditNoteReconciliationAsync

```csharp
Task<CreditNoteReconciliation> CreateCreditNoteReconciliationAsync(
    CreateCreditNoteReconciliationRequest request,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `request` | `CreateCreditNoteReconciliationRequest` | Yes | - | `credit_note_reconciliation` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/credit_note_reconciliations`

**Sample:**

```csharp
using FreeAgent.Client.Models.CreditNoteReconciliations;

var created = await client.CreditNoteReconciliations.CreateCreditNoteReconciliationAsync(
    CreateCreditNoteReconciliationRequest.Create(
        grossValue: 100m,
        invoiceId: 1,
        creditNoteId: 2,
        datedOn: new DateOnly(2024, 3, 18)));
```

---

### UpdateCreditNoteReconciliationAsync

```csharp
Task<CreditNoteReconciliation> UpdateCreditNoteReconciliationAsync(
    long creditNoteReconciliationId,
    UpdateCreditNoteReconciliationRequest request,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `creditNoteReconciliationId` | `long` | Yes | - | path `:id` |
| `request` | `UpdateCreditNoteReconciliationRequest` | Yes | - | `credit_note_reconciliation` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/credit_note_reconciliations/:id`

**Sample:**

```csharp
var updated = await client.CreditNoteReconciliations.UpdateCreditNoteReconciliationAsync(
    123,
    UpdateCreditNoteReconciliationRequest.Create(grossValue: 50m));
```

---

### DeleteCreditNoteReconciliationAsync

```csharp
Task DeleteCreditNoteReconciliationAsync(
    long creditNoteReconciliationId,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `creditNoteReconciliationId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/credit_note_reconciliations/:id`

## CreditNoteReconciliationGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeInvoice` | `bool` | Fetch linked invoice when the response contains only an invoice URI |
| `IncludeCreditNote` | `bool` | Fetch linked credit note when the response contains only a credit note URI |

## CreditNoteReconciliation model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `GrossValue` | `decimal?` | `gross_value` | Required on create |
| `DatedOn` | `DateOnly?` | `dated_on` | |
| `ExchangeRate` | `decimal?` | `exchange_rate` | |
| `Currency` | `CurrencyCode?` | `currency` | |
| `InvoiceId` | `long?` | - | Parsed from `invoice` link |
| `CreditNoteId` | `long?` | - | Parsed from `credit_note` link |
| `Invoice` | `Invoice?` | - | Hydrated invoice |
| `CreditNote` | `CreditNote?` | - | Hydrated credit note |
| `CreatedAt` | `DateTimeOffset?` | `created_at` | Read-only |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` | Read-only |
| `Url` | `string` | `url` | |

## Write requests

`CreateCreditNoteReconciliationRequest.Create(...)` requires `grossValue`, `invoiceId`, and `creditNoteId`.

`UpdateCreditNoteReconciliationRequest.Create(...)` accepts optional fields; only supplied values are serialised.

## CreditNoteReconciliationReference

Use `client.Urls.CreditNoteReconciliation(id)` or `CreditNoteReconciliationReference.ForEnvironment(environment, id)` to build environment-correct reconciliation URIs.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
