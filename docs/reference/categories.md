---
title: Categories
parent: API coverage
nav_order: 3
---

# Categories

List, create, update, and delete chart-of-accounts categories. Create and update operations use **typed request factories** per documented variant - each variant exposes only the attributes allowed for that category group.

| | |
|---|---|
| **SDK service** | `client.Categories` (`CategoryService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/categories](https://dev.freeagent.com/docs/categories) |
| **Sample app** | `/categories` (list), `/categories/detail` (CRUD) |

**Notes:**

- Typed write factories accept three-digit nominal codes in documented ranges (for example `047` for income). Sub-account codes (for example `602-1`) can be read and deleted but are not supported by write factories.
- On update, the path `nominalCode` identifies the existing category and may differ from `nominal_code` in the body when renaming.

## Read methods

### ListAsync

```csharp
Task<CategorySets> ListAsync(
    bool includeSubAccounts = false,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `includeSubAccounts` | `bool` | No | `false` | `sub_accounts=true` when `true` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/categories`

**Returns:** `CategorySets` - all category groups for the company (not paginated).

**Sample:**

```csharp
var categories = await client.Categories.ListAsync(includeSubAccounts: true);
```

---

### GetCategoryAsync

```csharp
Task<Category> GetCategoryAsync(string nominalCode, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `nominalCode` | `string` | Yes | - | path `:nominal_code` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/categories/:nominal_code`

Use `category.NominalCode` for API calls - not `ResourceId` when the code is non-numeric.

---

### DeleteCategoryAsync

```csharp
Task DeleteCategoryAsync(string nominalCode, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `nominalCode` | `string` | Yes | - | path `:nominal_code` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/categories/:nominal_code`

## Create methods

Each create method maps to `POST /v2/categories` with a variant-specific payload. The SDK sets `category_group` - callers do not supply it.

| Method | Request type | Nominal code range |
|--------|--------------|-------------------|
| `CreateIncomeCategoryAsync` | `CreateIncomeCategoryRequest` | 001–049 |
| `CreateCostOfSalesCategoryAsync` | `CreateCostOfSalesCategoryRequest` | 096–199 |
| `CreateAdminExpensesCategoryAsync` | `CreateAdminExpensesCategoryRequest` | 200–399 |
| `CreateCurrentAssetCategoryAsync` | `CreateCurrentAssetCategoryRequest` | 671–720 |
| `CreateLiabilitiesCategoryAsync` | `CreateLiabilitiesCategoryRequest` | 731–780 |
| `CreateEquityCategoryAsync` | `CreateEquityCategoryRequest` | 921–960 |

### Income and equity

Use `Create(description, nominalCode)`:

```csharp
var request = CreateIncomeCategoryRequest.Create("Consulting", "047");
var category = await client.Categories.CreateIncomeCategoryAsync(request);

var equity = CreateEquityCategoryRequest.Create("Retained earnings", "950");
await client.Categories.CreateEquityCategoryAsync(equity);
```

### Cost of sales, admin expenses, liabilities

Use company-type-specific factories. Each factory requires `taxReportingName` (typed enum), `allowableForTax`, and optionally `autoSalesTaxRate`:

```csharp
using FreeAgent.Client.Models.Categories;

var request = CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
    description: "Materials",
    nominalCode: "100",
    taxReportingName: UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
    allowableForTax: true);

var category = await client.Categories.CreateCostOfSalesCategoryAsync(request);
```

| Request type | Factory methods |
|--------------|-----------------|
| `CreateCostOfSalesCategoryRequest` | `ForUkLimitedCompany`, `ForUkSoleTrader`, `ForUkPartnership`, `ForUniversalAndUsCompany` |
| `CreateAdminExpensesCategoryRequest` | `ForUkLimitedCompany`, `ForUkSoleTrader`, `ForUkPartnership`, `ForUniversalAndUsCompany` |
| `CreateLiabilitiesCategoryRequest` | `ForUkLimitedCompany`, `ForOtherCompanyTypes` |
| `CreateCurrentAssetCategoryRequest` | `Create(description, nominalCode, CurrentAssetTaxReportingName)` |

### Tax reporting name enums

Pick the enum that matches your company type. Members map to exact wire values via `EnumWireValue` - browse IntelliSense for the full list.

| Company context | Cost of sales enum | Admin expenses enum | Liabilities enum |
|-----------------|-------------------|---------------------|------------------|
| UK limited company | `UkLimitedCompanyCostOfSalesTaxReportingName` | `UkLimitedCompanyAdminExpensesTaxReportingName` | `UkLimitedCompanyLiabilitiesTaxReportingName` |
| UK sole trader | `UkSoleTraderCostOfSalesTaxReportingName` | `UkSoleTraderAdminExpensesTaxReportingName` | - |
| UK partnership | `UkPartnershipCostOfSalesTaxReportingName` | `UkPartnershipAdminExpensesTaxReportingName` | - |
| Universal / US | `UniversalAndUsCostOfSalesTaxReportingName` | `UniversalAndUsAdminExpensesTaxReportingName` | - |
| Other company types | - | - | `OtherCompanyLiabilitiesTaxReportingName` |
| All (current asset) | - | - | `CurrentAssetTaxReportingName` |

## Update methods

Each update method maps to `PUT /v2/categories/:nominal_code`. The path `nominalCode` is the **existing** category identifier.

| Method | Request type |
|--------|--------------|
| `UpdateIncomeCategoryAsync` | `UpdateIncomeCategoryRequest` |
| `UpdateCostOfSalesCategoryAsync` | `UpdateCostOfSalesCategoryRequest` |
| `UpdateAdminExpensesCategoryAsync` | `UpdateAdminExpensesCategoryRequest` |
| `UpdateCurrentAssetCategoryAsync` | `UpdateCurrentAssetCategoryRequest` |
| `UpdateLiabilitiesCategoryAsync` | `UpdateLiabilitiesCategoryRequest` |
| `UpdateEquityCategoryAsync` | `UpdateEquityCategoryRequest` |

Factory methods mirror the create request types. Pass the existing nominal code as the first argument:

```csharp
var request = UpdateIncomeCategoryRequest.Create("Updated name", "048");
var updated = await client.Categories.UpdateIncomeCategoryAsync("047", request);
```

## Category model

Key read properties:

| Property | Type | Wire name |
|----------|------|-----------|
| `Description` | `string?` | `description` |
| `NominalCode` | `string?` | `nominal_code` |
| `GroupDescription` | `string?` | `group_description` |
| `AllowableForTax` | `bool?` | `allowable_for_tax` |
| `TaxReportingName` | `string?` | `tax_reporting_name` |
| `AutoSalesTaxRate` | `CategoryAutoSalesTaxRate?` | `auto_sales_tax_rate` |
| `Url` | `string` | `url` |

## CategoryAutoSalesTaxRate

| Member | Wire value |
|--------|------------|
| `OutsideOfTheScopeOfVat` | `Outside of the scope of VAT` |
| `ZeroRate` | `Zero rate` |
| `ReducedRate` | `Reduced rate` |
| `StandardRate` | `Standard rate` |
| `Exempt` | `Exempt` |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md). Nominal code range violations are validated client-side before the HTTP request for typed write factories.
