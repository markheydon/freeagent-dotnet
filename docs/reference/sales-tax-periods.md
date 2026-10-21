---
title: Sales tax periods
parent: API coverage
nav_order: 32
---

# Sales tax periods

List, create, update, and delete sales tax periods for US and universal companies.

| | |
|---|---|
| **SDK service** | `client.SalesTaxPeriods` (`SalesTaxPeriodsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/sales_tax_periods](https://dev.freeagent.com/docs/sales_tax_periods) |
| **Sample app** | `/sales-tax-periods` (list), `/sales-tax-periods/detail` (CRUD) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<SalesTaxPeriod>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/sales_tax_periods`

**Returns:** All sales tax periods for the company (not paginated).

---

### GetSalesTaxPeriodAsync

```csharp
Task<SalesTaxPeriod> GetSalesTaxPeriodAsync(long salesTaxPeriodId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/sales_tax_periods/:id`

---

### CreateSalesTaxPeriodAsync

```csharp
Task<SalesTaxPeriod> CreateSalesTaxPeriodAsync(
    CreateSalesTaxPeriodRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/sales_tax_periods`

Required request fields: `sales_tax_name`, `sales_tax_registration_status`, `sales_tax_rate_1`, `sales_tax_is_value_added`, `effective_date`.

**Sample:**

```csharp
var period = await client.SalesTaxPeriods.CreateSalesTaxPeriodAsync(new CreateSalesTaxPeriodRequest
{
    SalesTaxName = "Sales Tax",
    SalesTaxRegistrationStatus = SalesTaxPeriodRegistrationStatus.Registered,
    SalesTaxRate1 = 8.25m,
    SalesTaxIsValueAdded = false,
    EffectiveDate = new DateOnly(2026, 1, 1),
    SalesTaxRegistrationNumber = "12345678"
});
```

---

### UpdateSalesTaxPeriodAsync

```csharp
Task<SalesTaxPeriod> UpdateSalesTaxPeriodAsync(
    long salesTaxPeriodId,
    UpdateSalesTaxPeriodRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/sales_tax_periods/:id`

Include only the attributes you wish to change.

---

### DeleteSalesTaxPeriodAsync

```csharp
Task DeleteSalesTaxPeriodAsync(long salesTaxPeriodId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/sales_tax_periods/:id`

Locked periods cannot be deleted.

## Models

### SalesTaxPeriod

| Property | Type | Wire name |
|----------|------|-----------|
| `Url` | `string` | `url` |
| `ResourceId` | `long` | (parsed from `url`) |
| `SalesTaxName` | `string?` | `sales_tax_name` |
| `SalesTaxRegistrationStatus` | `SalesTaxPeriodRegistrationStatus?` | `sales_tax_registration_status` |
| `SalesTaxRate1` | `decimal?` | `sales_tax_rate_1` |
| `SalesTaxRate2` | `decimal?` | `sales_tax_rate_2` |
| `SalesTaxRate3` | `decimal?` | `sales_tax_rate_3` |
| `SalesTaxIsValueAdded` | `bool?` | `sales_tax_is_value_added` |
| `SalesTaxRegistrationNumber` | `string?` | `sales_tax_registration_number` |
| `EffectiveDate` | `DateOnly?` | `effective_date` |
| `IsLocked` | `bool?` | `is_locked` |
| `LockedReason` | `string?` | `locked_reason` |
| `SecondSalesTaxName` | `string?` | `second_sales_tax_name` |
| `SecondSalesTaxRate1` | `decimal?` | `second_sales_tax_rate_1` |
| `SecondSalesTaxRate2` | `decimal?` | `second_sales_tax_rate_2` |
| `SecondSalesTaxRate3` | `decimal?` | `second_sales_tax_rate_3` |
| `SecondSalesTaxIsCompound` | `bool?` | `second_sales_tax_is_compound` |

### SalesTaxPeriodRegistrationStatus

Documented wire values on sales tax periods (distinct from company `SalesTaxRegistrationStatus`):

| Member | Wire value |
|--------|------------|
| `NotRegistered` | `Not Registered` |
| `Registered` | `Registered` |
