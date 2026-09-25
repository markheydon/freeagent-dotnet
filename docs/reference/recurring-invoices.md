---
title: Recurring invoices
parent: API coverage
nav_order: 7
---

# Recurring invoices

List and get recurring invoice profiles. The FreeAgent API documents **read-only** access for this resource (no create, update, or delete).

| | |
|---|---|
| **SDK service** | `client.RecurringInvoices` (`RecurringInvoiceService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/recurring_invoices](https://dev.freeagent.com/docs/recurring_invoices) |
| **Sample app** | `/recurring-invoices` (list), `/recurring-invoices/detail` (GET) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Contact, project, category, and bank account links: see [Linked resources](../explanation/linked-resources.md). Invoices generated from a recurring profile expose a read-only `recurring_invoice` URI on `Invoice`.

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<RecurringInvoice>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    ContactReference? contact = null,
    long? contactId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `contact` | `ContactReference?` | No | `null` | `contact` (URI) |
| `contactId` | `long?` | No | `null` | `contact` (URI from `client.Urls.Contact(id)`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/recurring_invoices`

**View filters:** `RecurringInvoiceViews.Draft`, `RecurringInvoiceViews.Active`, `RecurringInvoiceViews.Inactive`.

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `contact` **or** `contactId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.RecurringInvoices;

var page = await client.RecurringInvoices.ListAsync(
    view: RecurringInvoiceViews.Active,
    contactId: 42);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<RecurringInvoice> ListAutoPagingAsync(...)
```

Same query parameters as `ListAsync` except `page`.

---

### GetRecurringInvoiceAsync

```csharp
Task<RecurringInvoice> GetRecurringInvoiceAsync(
    long recurringInvoiceId,
    CancellationToken cancellationToken = default)

Task<RecurringInvoice> GetRecurringInvoiceAsync(
    long recurringInvoiceId,
    RecurringInvoiceGetOptions? options,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/recurring_invoices/:id`

When `options.IncludeContact` or `options.IncludeProject` is `true`, the SDK fetches linked resources when the response contains only URIs.

**Sample:**

```csharp
var profile = await client.RecurringInvoices.GetRecurringInvoiceAsync(
    123,
    new RecurringInvoiceGetOptions { IncludeContact = true });
```

---

## Models

### RecurringInvoice

Read model combining invoice attributes with scheduling fields:

| Property | Type | Notes |
|----------|------|-------|
| `Frequency` | `RecurringInvoiceFrequency?` | Schedule frequency |
| `RecurringStatus` | `RecurringInvoiceStatus?` | `Draft` or `Active` |
| `RecurringEndDate` | `DateOnly?` | Blank when recurring forever |
| `NextRecursOn` | `DateOnly?` | Next generation date |
| `Contact`, `ContactId`, `ContactName` | | Billing contact |
| `Project`, `ProjectId` | | Optional project link |
| `InvoiceItems` | `List<InvoiceItem>?` | Line items (typically on GET by id) |
| `PaymentMethods` | `InvoicePaymentMethods?` | Online payment flags |

See also shared invoice read fields (`Reference`, `DatedOn`, `Currency`, `NetValue`, `TotalValue`, and others) on the type.

### Enums and constants

- `RecurringInvoiceFrequency` — wire values include `Two Weekly`, `Four Weekly`, `2-Yearly`, and others per FreeAgent docs
- `RecurringInvoiceStatus` — `Draft`, `Active`
- `RecurringInvoiceViews` — list filter constants

### References

- `RecurringInvoiceReference` — typed URI for filters and links (`client.Urls.RecurringInvoice(id)`)

---

## Sample apps

**Blazor:** `/recurring-invoices` lists profiles with view and contact filters; `/recurring-invoices/detail?id=` loads a single profile with optional linked-resource hydration.

**Console:** `RecurringInvoiceSamples` — list, stream all pages, list by contact, get by id.

Probe pages use **live sandbox data**; there is no Turpinverse seeder because the API cannot create recurring profiles.
