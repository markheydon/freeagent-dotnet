---
title: Capital assets
parent: API coverage
nav_order: 18
---

# Capital assets

Read-only access to capital assets and optional lifecycle history.

| | |
|---|---|
| **SDK service** | `client.CapitalAssets` (`CapitalAssetsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/capital_assets](https://dev.freeagent.com/docs/capital_assets) |
| **Sample app** | `/capital-assets` (list), `/capital-assets/detail` (read-only GET) |

Capital assets are created indirectly via [Expenses](expenses.md) or [Bank transaction explanations](bank-transaction-explanations.md) with capital categories — there is no standalone create on the capital assets endpoint.

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<CapitalAsset>> ListAsync(
    CapitalAssetView? view = null,
    bool includeHistory = false,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `view` | `CapitalAssetView?` | No | `null` (API default) | `view` query |
| `includeHistory` | `bool` | No | `false` | `include_history=true` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/capital_assets`

**Returns:** All capital assets for the company (not paginated).

**Sample:**

```csharp
var disposed = await client.CapitalAssets.ListAsync(
    CapitalAssetView.Disposed,
    includeHistory: true);
```

---

### GetCapitalAssetAsync

```csharp
Task<CapitalAsset> GetCapitalAssetAsync(long capitalAssetId, CancellationToken cancellationToken = default)

Task<CapitalAsset> GetCapitalAssetAsync(
    long capitalAssetId,
    CapitalAssetGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `capitalAssetId` | `long` | Yes | - | path `:id` |
| `options` | `CapitalAssetGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/capital_assets/:id`

Set `CapitalAssetGetOptions.IncludeHistory` to request lifecycle events on the response.

**Sample:**

```csharp
var asset = await client.CapitalAssets.GetCapitalAssetAsync(
    1,
    new CapitalAssetGetOptions { IncludeHistory = true });
```

---

## Models

### CapitalAsset

| Property | Type | Wire field |
|----------|------|------------|
| `Url` | `string` | `url` |
| `ResourceId` | `long` | parsed from `url` |
| `Description` | `string?` | `description` |
| `AssetLifeYears` | `int?` | `asset_life_years` (deprecated) |
| `DepreciationProfile` | `DepreciationProfile?` | `depreciation_profile` |
| `AssetType` | `string?` | `asset_type` |
| `PurchasedOn` | `DateOnly?` | `purchased_on` |
| `DisposedOn` | `DateOnly?` | `disposed_on` |
| `CreatedAt` | `DateTimeOffset?` | `created_at` |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` |
| `CapitalAssetHistory` | `List<CapitalAssetHistoryEntry>?` | `capital_asset_history` |

### CapitalAssetHistoryEntry

| Property | Type | Wire field |
|----------|------|------------|
| `Type` | `CapitalAssetHistoryEventType?` | `type` |
| `Description` | `string?` | `description` |
| `Date` | `DateOnly?` | `date` |
| `Value` | `decimal?` | `value` |
| `TaxValue` | `decimal?` | `tax_value` |
| `ExpenseId` | `long?` | `link` (expense URI) |
| `BillId` | `long?` | `link` (bill URI) |
| `BankTransactionExplanationId` | `long?` | `link` (explanation URI) |

### CapitalAssetView

Documented `view` query values: `all`, `disposed`, `disposable`.

### CapitalAssetReference

Environment-correct capital asset URI builder via `client.Urls.CapitalAsset(id)`.
