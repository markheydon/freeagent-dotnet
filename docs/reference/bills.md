---
title: Bills
parent: API coverage
nav_order: 8.2
---

# Bills

List, create, update, and delete bills; nested bill items on the parent resource; optional contact and project hydration on get.

| | |
|---|---|
| **SDK service** | `client.Bills` (`BillService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/bills](https://dev.freeagent.com/docs/bills) |
| **Sample app** | `/bills` (list), `/bills/detail` (CRUD) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Contact, project, and category links: see [Linked resources](../explanation/linked-resources.md). Purchases cluster patterns: see [Purchases entity map](../explanation/api-entity-map/purchases.md).

Nested `attachment` on bill responses may omit thumbnail URLs and `expires_at`. For the full attachment payload, call [Attachments](attachments.md) (`client.Attachments.GetAttachmentAsync`) using the attachment ID from the nested `url`.

## Sales tax on bill items

Per [Sales tax and VAT](https://dev.freeagent.com/docs/sales_tax): on each `BillItem`, use `PurchaseSalesTaxRate` for `SalesTaxRate` (`Auto` or a percentage), optional `ManualSalesTaxAmount`, and `SalesTaxStatus` (`BillSalesTaxStatus`). `SalesTaxValue` is calculated by FreeAgent on read.

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Bill>> ListAsync(
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
    bool? nestedBillItems = null,
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
| `nestedBillItems` | `bool?` | No | `null` | `nested_bill_items` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bills`

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; supply `contact` **or** `contactId`, not both; supply `project` **or** `projectId`, not both.

**Sample:**

```csharp
using FreeAgent.Client.Models.Bills;

var page = await client.Bills.ListAsync(
    view: BillViews.Open,
    contactId: 42,
    nestedBillItems: true);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Bill> ListAutoPagingAsync(...)
```

Same query parameters as `ListAsync` except `page`.

---

### GetBillAsync

```csharp
Task<Bill> GetBillAsync(long billId, CancellationToken cancellationToken = default)

Task<Bill> GetBillAsync(
    long billId,
    BillGetOptions? options,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/bills/:id`

When `options.IncludeContact`, `options.IncludeProject`, `options.IncludeRebillToProject`, or `options.IncludeRebilledOnInvoice` is `true`, the SDK fetches linked resources when the response contains only URIs. `IncludeRebilledOnInvoice` applies when `rebilled_on_invoice_item` is an invoice URI (not an invoice item URI).

---

### CreateBillAsync

```csharp
Task<Bill> CreateBillAsync(Bill bill, CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bills`

Required on create: `ContactId`, `DatedOn`, and at least one line item with `Description`, `CategoryNominalCode`, and value fields appropriate to your account.

---

### UpdateBillAsync

```csharp
Task<Bill> UpdateBillAsync(
    long billId,
    Bill bill,
    BillUpdateOptions? options = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/bills/:id`

Update existing line items using the bill item resource URL (set `BillItem.Url` or rely on `ItemId` parsed from a prior GET). Delete line items with a bill item URL and `Destroy = 1`. Pass `new BillUpdateOptions { OmitLineItems = true }` to update scalar fields without sending line items.

---

### DeleteBillAsync

```csharp
Task DeleteBillAsync(long billId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/bills/:id`

---

## List filters

### view (`BillViews`)

| Constant | Wire value |
|----------|------------|
| `BillViews.All` | `all` |
| `BillViews.Open` | `open` |
| `BillViews.Overdue` | `overdue` |
| `BillViews.OpenOrOverdue` | `open_or_overdue` |
| `BillViews.OpenOrOverduePayments` | `open_or_overdue_payments` |
| `BillViews.OpenOrOverdueRefunds` | `open_or_overdue_refunds` |
| `BillViews.Paid` | `paid` |
| `BillViews.Recurring` | `recurring` |
| `BillViews.HirePurchase` | `hire_purchase` |
| `BillViews.Cis` | `cis` |

## BillGetOptions

| Property | Type | Purpose |
|----------|------|---------|
| `IncludeContact` | `bool` | Fetch contact when only a URI is returned |
| `IncludeProject` | `bool` | Fetch project when only a URI is returned |

## BillUpdateOptions

| Property | Type | Purpose |
|----------|------|---------|
| `OmitLineItems` | `bool` | Exclude line items from the update payload |
| `OmitContact` | `bool` | Exclude the contact link from the update payload |
| `OmitProject` | `bool` | Exclude the project link from the update payload |
| `OmitRebillToProject` | `bool` | Exclude the rebill target project link from the update payload |

## Bill model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Reference` | `string?` | `reference` | |
| `Status` | `BillStatus?` | `status` | Read-only payment status |
| `DatedOn` | `DateOnly?` | `dated_on` | Required on create |
| `DueOn` | `DateOnly?` | `due_on` | |
| `ContactId` | `long?` | `contact` | Required on create |
| `ProjectId` | `long?` | `project` | Optional project link |
| `IsPaidByHirePurchase` | `bool?` | `is_paid_by_hire_purchase` | Hire purchase flag |
| `BillItems` | `List<BillItem>?` | `bill_items` | Nested line items |
| `ItemId` | `long?` | (from `url` on line items) | Parsed from bill item URL |
| `CategoryNominalCode` | `string?` | `category` (line items) | Category link on line items |
| `TotalValue` | `decimal?` | `total_value` | Read-only on bill; writable on line items |
| `Url` | `string` | `url` | |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
