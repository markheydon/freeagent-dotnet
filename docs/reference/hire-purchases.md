---
title: Hire purchases
parent: API coverage
nav_order: 19
---

# Hire purchases

Read-only access to hire purchases linked to bills (UK companies only).

| | |
|---|---|
| **SDK service** | `client.HirePurchases` (`HirePurchasesService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/hire_purchases](https://dev.freeagent.com/docs/hire_purchases) |
| **Sample app** | `/hire-purchases` (list), `/hire-purchases/detail` (read-only GET) |

Hire purchases are created or removed via the [Bills](bills.md) API (`is_paid_by_hire_purchase`).

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<HirePurchase>> ListAsync(
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/hire_purchases`

**Returns:** All hire purchases for the company (not paginated).

**Sample:**

```csharp
var hirePurchases = await client.HirePurchases.ListAsync();
```

---

### GetHirePurchaseAsync

```csharp
Task<HirePurchase> GetHirePurchaseAsync(long hirePurchaseId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `hirePurchaseId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/hire_purchases/:id`

**Sample:**

```csharp
var hirePurchase = await client.HirePurchases.GetHirePurchaseAsync(1);
```

---

## Models

### HirePurchase

| Property | Type | Wire field |
|----------|------|------------|
| `Url` | `string` | `url` |
| `ResourceId` | `long` | parsed from `url` |
| `Description` | `string?` | `description` |
| `Bill` | `Bill?` | nested `bill` |
| `BillId` | `long?` | `bill` (URI) |
| `LiabilitiesOverOneYearCategoryNominalCode` | `string?` | `liabilities_over_one_year_category` (URI) |
| `LiabilitiesOverOneYearCategory` | `Category?` | nested `liabilities_over_one_year_category` |
| `LiabilitiesUnderOneYearCategoryNominalCode` | `string?` | `liabilities_under_one_year_category` (URI) |
| `LiabilitiesUnderOneYearCategory` | `Category?` | nested `liabilities_under_one_year_category` |

### HirePurchaseReference

Environment-correct hire purchase URI builder via `client.Urls.HirePurchase(id)`.
