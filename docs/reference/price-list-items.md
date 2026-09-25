---
title: Price list items
parent: API coverage
nav_order: 18
---

# Price list items

List, create, update, and delete price book entries used on invoices and estimates.

| | |
|---|---|
| **SDK service** | `client.PriceListItems` (`PriceListItemsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/price_list_items](https://dev.freeagent.com/docs/price_list_items) |
| **Sample app** | `/price-list-items` (list), `/price-list-items/detail` (CRUD) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<PriceListItem>> ListAsync(
    string? sort = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `sort` | `string` | No | `null` | `sort` (`created_at`, `code`, `updated_at`; prefix `-` for descending) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/price_list_items`

**Returns:** All price list items for the company (not paginated).

---

### GetPriceListItemAsync

```csharp
Task<PriceListItem> GetPriceListItemAsync(long priceListItemId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/price_list_items/:id`

---

### CreatePriceListItemAsync

```csharp
Task<PriceListItem> CreatePriceListItemAsync(
    CreatePriceListItemRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/price_list_items`

Required request fields: `code`, `quantity`, `item_type`, `description`, `price`.

**Sample:**

```csharp
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.PriceListItems;

var item = await client.PriceListItems.CreatePriceListItemAsync(new CreatePriceListItemRequest
{
    Code = "A001",
    Quantity = 1,
    ItemType = InvoiceItemType.Products,
    Description = "Apple",
    Price = 1.99m,
    VatStatus = PriceListItemVatStatus.Standard
});
```

---

### UpdatePriceListItemAsync

```csharp
Task<PriceListItem> UpdatePriceListItemAsync(
    long priceListItemId,
    UpdatePriceListItemRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/price_list_items/:id`

---

### DeletePriceListItemAsync

```csharp
Task DeletePriceListItemAsync(long priceListItemId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/price_list_items/:id`

The official FreeAgent docs show a singular `price_list_item` path for delete; the plural path matches list, get, create, and update.

---

## Models

### PriceListItem

| Property | Type | Wire field |
|----------|------|------------|
| `Code` | `string?` | `code` |
| `Quantity` | `decimal?` | `quantity` |
| `ItemType` | `InvoiceItemType?` | `item_type` |
| `Description` | `string?` | `description` |
| `Price` | `decimal?` | `price` |
| `VatStatus` | `PriceListItemVatStatus?` | `vat_status` |
| `SalesTaxRate` | `decimal?` | `sales_tax_rate` |
| `SecondSalesTaxRate` | `decimal?` | `second_sales_tax_rate` |
| `CategoryNominalCode` | `string?` | `category` (URI) |
| `Category` | `Category?` | nested `category` |
| `StockItemId` | `long?` | `stock_item` (URI) |
| `StockItemResource` | `StockItem?` | nested `stock_item` |

### CreatePriceListItemRequest / UpdatePriceListItemRequest

Typed write payloads exposing only documented attributes. Use `CategoryReference` and `StockItemReference` for linked resources.

### PriceListItemVatStatus

UK VAT status values: `out_of_scope`, `reduced`, `standard`, `zero`.
