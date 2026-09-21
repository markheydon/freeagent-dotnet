---
title: Company
parent: API coverage
nav_order: 1
---

# Company

Read company profile, business categories, and tax timeline for the authenticated account.

| | |
|---|---|
| **SDK service** | `client.Company` (`CompanyService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/company](https://dev.freeagent.com/docs/company) |
| **Sample app** | `/company`, `/company/business-categories`, `/company/tax-timeline` |

## Methods

### GetCompanyAsync

```csharp
Task<Company> GetCompanyAsync(CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/company`

**Returns:** `Company` - company profile for the authenticated account.

**Sample:**

```csharp
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
```

---

### ListBusinessCategoriesAsync

```csharp
Task<IReadOnlyList<string>> ListBusinessCategoriesAsync(CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/company/business_categories`

**Returns:** Business category names that can be assigned to a company.

**Sample:**

```csharp
var categories = await client.Company.ListBusinessCategoriesAsync();
```

---

### ListTaxTimelineAsync

```csharp
Task<IReadOnlyList<TaxTimelineItem>> ListTaxTimelineAsync(CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/company/tax_timeline`

**Returns:** Upcoming tax timeline events.

**Access:** Minimum FreeAgent access level - Tax, Accounting and Users.

**Sample:**

```csharp
var timeline = await client.Company.ListTaxTimelineAsync();
```

## Company model

Key read properties on `Company`:

| Property | Type | Wire name |
|----------|------|-----------|
| `Id` | `int` | `id` |
| `Name` | `string` | `name` |
| `Subdomain` | `string` | `subdomain` |
| `Type` | `CompanyType?` | `type` |
| `Currency` | `CurrencyCode?` | `currency` |
| `MileageUnits` | `MileageUnit?` | `mileage_units` |
| `CompanyRegistrationNumber` | `string?` | `company_registration_number` |
| `SalesTaxRegistrationStatus` | `SalesTaxRegistrationStatus?` | `sales_tax_registration_status` |
| `InitialVatBasis` | `InitialVatBasis?` | `initial_vat_basis` |
| `ShortDateFormat` | `ShortDateFormat?` | `short_date_format` |
| `Url` | `string` | `url` |

See IntelliSense or the source model for the full property list.

## Related types

| Type | Purpose |
|------|---------|
| `TaxTimelineItem` | Single tax timeline entry |
| `CompanyType` | Company type wire enum |
| `CurrencyCode` | ISO 4217 currency codes - see [Currency code](currency-code.md) |

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md) when the response envelope is missing expected data, or when the API returns an error status.
