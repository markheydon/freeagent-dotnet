---
title: Stock items
parent: API coverage
nav_order: 17
---

# Stock items

Read-only access to stock catalogue items defined in the FreeAgent account.

| | |
|---|---|
| **SDK service** | `client.StockItems` (`StockItemsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/stock_items](https://dev.freeagent.com/docs/stock_items) |
| **Sample app** | `/stock-items` (list), `/stock-items/detail` (read-only GET) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<StockItem>> ListAsync(
    string? sort = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `sort` | `string` | No | `null` | `sort` (`created_at`, `description`, `updated_at`; prefix `-` for descending) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/stock_items`

**Returns:** All stock items for the company (not paginated).

**Sample:**

```csharp
using FreeAgent.Client.Models.StockItems;

var stockItems = await client.StockItems.ListAsync(sort: StockItemSortOptions.Description);
```

---

### GetStockItemAsync

```csharp
Task<StockItem> GetStockItemAsync(long stockItemId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `stockItemId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/stock_items/:id`

**Sample:**

```csharp
var stockItem = await client.StockItems.GetStockItemAsync(3);
```

---

## Models

### StockItem

| Property | Type | Wire field |
|----------|------|------------|
| `Url` | `string` | `url` |
| `ResourceId` | `long` | parsed from `url` |
| `Description` | `string?` | `description` |
| `OpeningQuantity` | `decimal?` | `opening_quantity` |
| `OpeningBalance` | `decimal?` | `opening_balance` |
| `CostOfSaleCategoryNominalCode` | `string?` | `cost_of_sale_category` (URI) |
| `CostOfSaleCategory` | `Category?` | nested `cost_of_sale_category` |
| `StockOnHand` | `decimal?` | `stock_on_hand` |
| `CreatedAt` | `DateTimeOffset?` | `created_at` |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` |

### StockItemReference

Typed URI reference for `stock_item` links on invoice lines, price list items, and other resources. Build with `client.Urls.StockItem(id)` or `StockItemReference.Parse(uri)`.
