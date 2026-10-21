---
title: Depreciation profiles
parent: API coverage
nav_order: 21
---

# Depreciation profiles

Typed depreciation profile shapes nested under `capital_asset` when creating or updating capital purchases on expenses, bill items, and bank transaction explanations.

| | |
|---|---|
| **SDK types** | `DepreciationProfile`, `CapitalAssetPurchaseWrite`, `DepreciationMethod`, `DepreciationFrequency` in `FreeAgent.Client` |
| **FreeAgent docs** | [dev.freeagent.com/docs/depreciation_profiles](https://dev.freeagent.com/docs/depreciation_profiles) |
| **Sample app** | `/depreciation-profiles` (wire mapping), bank transaction explanation CRUD (capital asset purchase with profile) |

FreeAgent documents depreciation profiles but does **not** expose a `/v2/depreciation_profiles` REST resource. There is no `DepreciationProfilesService`.

## Documented methods

FreeAgent supports three depreciation methods, each with its own required wire fields:

| Method wire value | SDK factory | Required fields |
|-------------------|-------------|-----------------|
| `straight_line` | `CapitalAssetPurchaseWrite.StraightLine` | `asset_life_years` (2–25), optional `frequency` |
| `reducing_balance` | `CapitalAssetPurchaseWrite.ReducingBalance` | `annual_depreciation_percentage` (1–99), optional `frequency` |
| `no_depreciation` | `CapitalAssetPurchaseWrite.NoDepreciation` | `method` only |

Valid `frequency` values are `monthly` and `annually`. When omitted on write, FreeAgent defaults to monthly posting.

## Usage on parent resources

Set `CapitalAssetPurchase` on the parent model before create or update:

| Parent | Property |
|--------|----------|
| `Expense` | `CapitalAssetPurchase` |
| `BillItem` | `CapitalAssetPurchase` |
| `CreateCapitalAssetPurchaseBankTransactionExplanationRequest` | `CreateWithDepreciationProfile(...)` |
| `UpdateBankTransactionExplanationRequest` | `CapitalAssetPurchase` |

**Sample (expense):**

```csharp
var expense = new Expense
{
    UserId = userId,
    CategoryNominalCode = "602",
    DatedOn = new DateOnly(2024, 2, 1),
    GrossValue = -20_000m,
    Description = "Office equipment",
    CapitalAssetPurchase = CapitalAssetPurchaseWrite.StraightLine(10, DepreciationFrequency.Annually)
};

await client.Expenses.CreateExpenseAsync(expense);
```

**Sample (bank transaction explanation):**

```csharp
await client.BankTransactionExplanations.CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
    CreateCapitalAssetPurchaseBankTransactionExplanationRequest.CreateWithDepreciationProfile(
        bankTransactionId,
        categoryNominalCode: "720",
        datedOn: new DateOnly(2024, 2, 1),
        grossValue: -500m,
        capitalAssetPurchase: CapitalAssetPurchaseWrite.ReducingBalance(25)));
```

## Read model

`DepreciationProfile` deserialises the nested object returned on capital asset responses and nested purchase payloads.
