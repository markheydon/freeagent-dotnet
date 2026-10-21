---
title: Expenses
parent: API coverage
nav_order: 8.3
---

# Expenses

List, create (including batch), update, and delete expenses; optional linked-resource hydration on get; mileage settings lookup.

| | |
|---|---|
| **SDK service** | `client.Expenses` (`ExpenseService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/expenses](https://dev.freeagent.com/docs/expenses) |
| **Sample app** | `/expenses` (list), `/expenses/detail` (CRUD) |

Pagination patterns: see [Pagination](../how-to/pagination.md). User, category, project, and stock item links: see [Linked resources](../explanation/linked-resources.md). Purchases cluster patterns: see [Purchases entity map](../explanation/api-entity-map/purchases.md).

Nested `attachment` on expense responses may omit thumbnail URLs and `expires_at`. For the full attachment payload (including when signed URLs expire), call [Attachments](attachments.md) (`client.Attachments.GetAttachmentAsync`) using the attachment ID from the nested `url`.

## Methods

### ListAsync / ListAutoPagingAsync

**HTTP:** `GET /v2/expenses`

Supports `page`, `per_page` (max `100`), `view` (`recent`, `recurring`), `from_date`, `to_date`, `updated_since`, and `project` (URI or `projectId`).

### GetExpenseAsync

**HTTP:** `GET /v2/expenses/:id`

Optional hydration via `ExpenseGetOptions` (`IncludeUser`, `IncludeCategory`, `IncludeProject`, `IncludeRebillToProject`, `IncludeStockItem`, `IncludeRebilledOnInvoice`).

### CreateExpenseAsync

**HTTP:** `POST /v2/expenses`

Set `UserId`, `CategoryNominalCode` or `IsMileageCategory`, `DatedOn`, and `Description` on `Expense`. Mileage claims also use `Mileage`, `VehicleType`, and related fields.

### CreateExpensesAsync

**HTTP:** `POST /v2/expenses` (batch envelope)

Posts an `expenses` array per the FreeAgent **Batch create** operation.

### UpdateExpenseAsync / DeleteExpenseAsync

**HTTP:** `PUT /v2/expenses/:id`, `DELETE /v2/expenses/:id`

### GetMileageSettingsAsync

**HTTP:** `GET /v2/expenses/mileage_settings`

Returns `MileageSettings` with `EngineTypeAndSizeOptions` for valid mileage engine types and sizes by date range.

## Category and mileage

The `category` wire field is either a category URI or the literal `Mileage`. On writes, set `CategoryNominalCode` or `IsMileageCategory = true` — the SDK serialises the correct wire shape.

## Sales tax fields

Per [Sales tax and VAT](https://dev.freeagent.com/docs/sales_tax): use `PurchaseSalesTaxRate` on `SalesTaxRate` (`Auto` or a percentage), optional `ManualSalesTaxAmount`, and `SalesTaxStatus` (`BillSalesTaxStatus`). `SalesTaxValue` is calculated by FreeAgent on read.
