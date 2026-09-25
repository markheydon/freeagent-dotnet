---
title: Invoices
parent: API coverage
nav_order: 6
---

# Invoices

List, create, update, and delete invoices; status transitions; email and PDF actions; company default additional text.

| | |
|---|---|
| **SDK service** | `client.Invoices` (`InvoiceService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/invoices](https://dev.freeagent.com/docs/invoices) |
| **Sample app** | `/invoices` (list), `/invoices/detail` (CRUD and transitions) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Contact, project, category, and bank account links: see [Linked resources](../explanation/linked-resources.md). Sales cluster patterns: see [Sales entity map](../explanation/api-entity-map/sales.md#sales-cluster-sdk-patterns).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Invoice>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    string? sort = null,
    DateTimeOffset? updatedSince = null,
    ContactReference? contact = null,
    long? contactId = null,
    ProjectReference? project = null,
    long? projectId = null,
    bool? nestedInvoiceItems = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `sort` | `string?` | No | `null` | `sort` |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` |
| `contact` | `ContactReference?` | No | `null` | `contact` (URI) |
| `contactId` | `long?` | No | `null` | `contact` (URI from `client.Urls.Contact(id)`) |
| `project` | `ProjectReference?` | No | `null` | `project` (URI) |
| `projectId` | `long?` | No | `null` | `project` (URI from `client.Urls.Project(id)`) |
| `nestedInvoiceItems` | `bool?` | No | `null` | `nested_invoice_items` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/invoices`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `contact` **or** `contactId`, not both; supply `project` **or** `projectId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.Invoices;

var page = await client.Invoices.ListAsync(
    view: InvoiceViews.Open,
    contactId: 42,
    nestedInvoiceItems: true);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Invoice> ListAutoPagingAsync(...)
```

Same query parameters as `ListAsync` except `page`.

---

### GetInvoiceAsync

```csharp
Task<Invoice> GetInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default)

Task<Invoice> GetInvoiceAsync(
    long invoiceId,
    InvoiceGetOptions? options,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/invoices/:id`

When `options.IncludeContact` or `options.IncludeProject` is `true`, the SDK fetches linked resources when the response contains only URIs.

---

### GetInvoicePdfAsync

```csharp
Task<byte[]> GetInvoicePdfAsync(long invoiceId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/invoices/:id/pdf`

Decodes the base64 `pdf.content` wrapper returned by FreeAgent.

---

### CreateInvoiceAsync

```csharp
Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/invoices`

Required on create: `ContactId`, `DatedOn`, `PaymentTermsInDays`, and at least one line item with `Description`.

---

### DuplicateInvoiceAsync

```csharp
Task<Invoice> DuplicateInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/invoices/:id/duplicate`

---

### UpdateInvoiceAsync

```csharp
Task<Invoice> UpdateInvoiceAsync(
    long invoiceId,
    Invoice invoice,
    InvoiceUpdateOptions? options = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/invoices/:id`

Update existing line items with `ItemId`. Delete line items with `ItemId` and `Destroy = 1`. Pass `new InvoiceUpdateOptions { OmitLineItems = true }` to update scalar fields without sending line items. `ShowProjectName` is not sent on update — FreeAgent locks `show_project_name` once an invoice leaves draft; set it on create instead.

---

### DeleteInvoiceAsync

```csharp
Task DeleteInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/invoices/:id`

---

### SendInvoiceEmailAsync

```csharp
Task SendInvoiceEmailAsync(
    long invoiceId,
    SendInvoiceEmailRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/invoices/:id/send_email`

Set `request.Email.UseTemplate = true` to send using an existing template.

---

### MarkInvoiceAsSentAsync / MarkInvoiceAsScheduledAsync / MarkInvoiceAsDraftAsync / MarkInvoiceAsCancelledAsync

```csharp
Task<Invoice> MarkInvoiceAsSentAsync(long invoiceId, CancellationToken cancellationToken = default)
Task<Invoice> MarkInvoiceAsScheduledAsync(long invoiceId, CancellationToken cancellationToken = default)
Task<Invoice> MarkInvoiceAsDraftAsync(long invoiceId, CancellationToken cancellationToken = default)
Task<Invoice> MarkInvoiceAsCancelledAsync(long invoiceId, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/invoices/:id/transitions/{action}`

**Behaviour:** The SDK sends `PUT .../transitions/{action}` with an empty JSON body (`{}`), then fetches the updated invoice with `GET .../invoices/:id` because FreeAgent transition endpoints return no response body. If the transition succeeds but the follow-up GET fails, the invoice state has already changed on the server; retry the GET to obtain the updated resource.

---

### TakeDirectDebitPaymentAsync

```csharp
Task TakeDirectDebitPaymentAsync(long invoiceId, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/invoices/:id/direct_debit`

---

### ListTimelineAsync

```csharp
Task<IReadOnlyList<InvoiceTimelineItem>> ListTimelineAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/invoices/timeline`

Company-wide invoice timeline (not scoped to a single invoice).

---

### ConvertToCreditNoteAsync

```csharp
Task<CreditNote> ConvertToCreditNoteAsync(long invoiceId, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/invoices/:id/transitions/convert_to_credit_note`

Returns the converted credit note. Use `client.CreditNotes` for full credit note operations — see [Credit notes](credit-notes.md).

---

### GetDefaultAdditionalTextAsync / UpdateDefaultAdditionalTextAsync / DeleteDefaultAdditionalTextAsync

**HTTP:** `GET`, `PUT`, and `DELETE` `/v2/invoices/default_additional_text`

Company-level default text shown on all invoices.

## List filters

### view (`InvoiceViews`)

| Constant | Wire value |
|----------|------------|
| `InvoiceViews.All` | `all` |
| `InvoiceViews.RecentOpenOrOverdue` | `recent_open_or_overdue` |
| `InvoiceViews.Open` | `open` |
| `InvoiceViews.Overdue` | `overdue` |
| `InvoiceViews.OpenOrOverdue` | `open_or_overdue` |
| `InvoiceViews.Draft` | `draft` |
| `InvoiceViews.Paid` | `paid` |
| `InvoiceViews.ScheduledToEmail` | `scheduled_to_email` |
| `InvoiceViews.ThankYouEmails` | `thank_you_emails` |
| `InvoiceViews.ReminderEmails` | `reminder_emails` |

### sort (`InvoiceSortOptions`)

`created_at`, `updated_at` — prefix with `-` for descending.

## InvoiceGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeContact` | `bool` | Fetch contact when only a URI is returned |
| `IncludeProject` | `bool` | Fetch project when only a URI is returned |

## InvoiceUpdateOptions

| Property | Type | Purpose |
|----------|------|---------|
| `OmitLineItems` | `bool` | Exclude line items from the update payload |
| `OmitContact` | `bool` | Exclude the contact link from the update payload |
| `OmitProject` | `bool` | Exclude the project link from the update payload |
| `OmitBankAccount` | `bool` | Exclude the bank account link from the update payload |

## Invoice model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Reference` | `string?` | `reference` | |
| `Status` | `InvoiceStatus?` | `status` | |
| `DatedOn` | `DateOnly?` | `dated_on` | Required on create |
| `DueOn` | `DateOnly?` | `due_on` | |
| `PaymentTermsInDays` | `int?` | `payment_terms_in_days` | Required on create |
| `ContactId` | `long?` | `contact` | Required on create |
| `ProjectId` | `long?` | `project` | Optional project link |
| `BankAccountId` | `long?` | `bank_account` | Optional remittance bank account |
| `InvoiceItems` | `List<InvoiceItem>?` | `invoice_items` | Nested line items |
| `ShowProjectName` | `bool?` | `show_project_name` | Writable on create; not sent on update (FreeAgent locks it once the invoice leaves draft) |
| `ItemId` | `long?` | `id` (line items) | Required to update or delete existing line items |
| `CategoryNominalCode` | `string?` | `category` (line items) | Category link on line items |
| `Url` | `string` | `url` | |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
