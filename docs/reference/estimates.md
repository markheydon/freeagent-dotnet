---
title: Estimates
parent: API coverage
nav_order: 6
---

# Estimates

List, create, update, and delete estimates; status transitions; email and PDF actions; standalone estimate items; company default additional text.

| | |
|---|---|
| **SDK service** | `client.Estimates` (`EstimateService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/estimates](https://dev.freeagent.com/docs/estimates) |
| **Sample app** | `/estimates` (list), `/estimates/detail` (CRUD and transitions) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Contact, project, category, and invoice links: see [Linked resources](../explanation/linked-resources.md). Sales cluster patterns: see [Sales entity map](../explanation/api-entity-map/sales.md#sales-cluster-sdk-patterns).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Estimate>> ListAsync(
    int page = 1,
    int perPage = 25,
    string? view = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    ContactReference? contact = null,
    long? contactId = null,
    ProjectReference? project = null,
    long? projectId = null,
    InvoiceReference? invoice = null,
    long? invoiceId = null,
    bool? nestedEstimateItems = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `string?` | No | `null` | `view` |
| `fromDate` | `DateOnly?` | No | `null` | `from_date` |
| `toDate` | `DateOnly?` | No | `null` | `to_date` |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` |
| `contact` | `ContactReference?` | No | `null` | `contact` (URI) |
| `contactId` | `long?` | No | `null` | `contact` (URI from `client.Urls.Contact(id)`) |
| `project` | `ProjectReference?` | No | `null` | `project` (URI) |
| `projectId` | `long?` | No | `null` | `project` (URI from `client.Urls.Project(id)`) |
| `invoice` | `InvoiceReference?` | No | `null` | `invoice` (URI) |
| `invoiceId` | `long?` | No | `null` | `invoice` (URI from `client.Urls.Invoice(id)`) |
| `nestedEstimateItems` | `bool?` | No | `null` | `nested_estimate_items` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/estimates`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `contact` **or** `contactId`, not both; supply `project` **or** `projectId`, not both; supply `invoice` **or** `invoiceId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.Estimates;

var page = await client.Estimates.ListAsync(
    view: EstimateViews.Draft,
    contactId: 42,
    nestedEstimateItems: true);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Estimate> ListAutoPagingAsync(...)
```

Same query parameters as `ListAsync` except `page`.

---

### GetEstimateAsync

```csharp
Task<Estimate> GetEstimateAsync(long estimateId, CancellationToken cancellationToken = default)

Task<Estimate> GetEstimateAsync(
    long estimateId,
    EstimateGetOptions? options,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/estimates/:id`

When `options.IncludeContact` or `options.IncludeProject` is `true`, the SDK fetches linked resources when the response contains only URIs.

---

### GetEstimatePdfAsync

```csharp
Task<byte[]> GetEstimatePdfAsync(long estimateId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/estimates/:id/pdf`

Decodes the base64 `pdf.content` wrapper returned by FreeAgent.

---

### CreateEstimateAsync

```csharp
Task<Estimate> CreateEstimateAsync(Estimate estimate, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/estimates`

Required on create: `ContactId`, `DatedOn`, `EstimateType`, and at least one line item with `Description`.

---

### DuplicateEstimateAsync

```csharp
Task<Estimate> DuplicateEstimateAsync(long estimateId, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/estimates/:id/duplicate`

---

### UpdateEstimateAsync

```csharp
Task<Estimate> UpdateEstimateAsync(
    long estimateId,
    Estimate estimate,
    EstimateUpdateOptions? options = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/estimates/:id`

Update existing line items with `ItemId`. Delete line items with `ItemId` and `Destroy = 1`. Pass `new EstimateUpdateOptions { OmitLineItems = true }` to update scalar fields without sending line items.

---

### DeleteEstimateAsync

```csharp
Task DeleteEstimateAsync(long estimateId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/estimates/:id`

---

### CreateEstimateItemAsync

```csharp
Task<EstimateItem> CreateEstimateItemAsync(
    long estimateId,
    EstimateItem estimateItem,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/estimate_items`

Creates a line item on an existing estimate. The request envelope includes both the parent estimate URI and the item attributes.

---

### UpdateEstimateItemAsync

```csharp
Task<EstimateItem> UpdateEstimateItemAsync(
    long estimateItemId,
    EstimateItem estimateItem,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/estimate_items/:id`

---

### DeleteEstimateItemAsync

```csharp
Task DeleteEstimateItemAsync(long estimateItemId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/estimate_items/:id`

---

### SendEstimateEmailAsync

```csharp
Task SendEstimateEmailAsync(
    long estimateId,
    SendEstimateEmailRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/estimates/:id/send_email`

Set `request.Email.UseTemplate = true` to send using an existing template.

---

### MarkEstimateAsSentAsync / MarkEstimateAsDraftAsync / MarkEstimateAsApprovedAsync / MarkEstimateAsRejectedAsync

```csharp
Task<Estimate> MarkEstimateAsSentAsync(long estimateId, CancellationToken cancellationToken = default)
Task<Estimate> MarkEstimateAsDraftAsync(long estimateId, CancellationToken cancellationToken = default)
Task<Estimate> MarkEstimateAsApprovedAsync(long estimateId, CancellationToken cancellationToken = default)
Task<Estimate> MarkEstimateAsRejectedAsync(long estimateId, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/estimates/:id/transitions/{action}`

**Behaviour:** The SDK sends `PUT .../transitions/{action}` with an empty JSON body (`{}`), then fetches the updated estimate with `GET .../estimates/:id` because FreeAgent transition endpoints return no response body. If the transition succeeds but the follow-up GET fails, the estimate state has already changed on the server; retry the GET to obtain the updated resource.

---

### ConvertToInvoiceAsync

```csharp
Task<Estimate> ConvertToInvoiceAsync(long estimateId, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/estimates/:id/transitions/convert_to_invoice`

Returns the updated estimate with an `invoice` link populated. When the transition response has no estimate body, the SDK fetches the estimate with `GET .../estimates/:id`.

---

### GetDefaultAdditionalTextAsync / UpdateDefaultAdditionalTextAsync / DeleteDefaultAdditionalTextAsync

**HTTP:** `GET`, `PUT`, and `DELETE` `/v2/estimates/default_additional_text`

Company-level default text shown on all estimates.

## List filters

### view (`EstimateViews`)

| Constant | Wire value |
|----------|------------|
| `EstimateViews.All` | `all` |
| `EstimateViews.Recent` | `recent` |
| `EstimateViews.Draft` | `draft` |
| `EstimateViews.NonDraft` | `non_draft` |
| `EstimateViews.Sent` | `sent` |
| `EstimateViews.Approved` | `approved` |
| `EstimateViews.Rejected` | `rejected` |
| `EstimateViews.Invoiced` | `invoiced` |

## EstimateGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeContact` | `bool` | Fetch contact when only a URI is returned |
| `IncludeProject` | `bool` | Fetch project when only a URI is returned |

## EstimateUpdateOptions

| Property | Type | Purpose |
|----------|------|---------|
| `OmitLineItems` | `bool` | Exclude line items from the update payload |
| `OmitContact` | `bool` | Exclude the contact link from the update payload |
| `OmitProject` | `bool` | Exclude the project link from the update payload |

## Estimate model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Reference` | `string?` | `reference` | |
| `Status` | `EstimateStatus?` | `status` | |
| `EstimateType` | `EstimateType?` | `estimate_type` | Required on create |
| `DatedOn` | `DateOnly?` | `dated_on` | Required on create |
| `ContactId` | `long?` | `contact` | Required on create |
| `ProjectId` | `long?` | `project` | Optional project link |
| `EstimateItems` | `List<EstimateItem>?` | `estimate_items` | Nested line items |
| `ItemId` | `long?` | `id` (line items) | Required to update or delete existing line items |
| `CategoryNominalCode` | `string?` | `category` (line items) | Category link on line items |
| `SalesTaxStatus` | `InvoiceSalesTaxStatus?` | `sales_tax_status` | Read-only on list/get responses; set per line item on create/update |
| `SalesTaxValue` | `decimal?` | `sales_tax_value` | Read-only total from line items |
| `Url` | `string` | `url` | |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
