---
title: EC VAT MOSS sales tax rates
parent: API coverage
nav_order: 32.5
---

# EC VAT MOSS sales tax rates

Look up MOSS sales tax rate bands for an EU place-of-supply country and transaction date. See also [Sales tax and VAT](https://dev.freeagent.com/docs/sales_tax) for field behaviour on invoices, estimates, and bank transaction explanations.

| | |
|---|---|
| **SDK service** | `client.EcMoss` (`EcMossService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/sales_tax](https://dev.freeagent.com/docs/sales_tax) (EC VAT MOSS section) |
| **Sample app** | `/ec-moss/sales-tax-rates` |

## Methods

### GetSalesTaxRatesAsync

```csharp
Task<IReadOnlyList<EcMossSalesTaxRate>> GetSalesTaxRatesAsync(
    EcMossPlaceOfSupply country,
    DateOnly date,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/ec_moss/sales_tax_rates?country={name}&date={YYYY-MM-DD}`

- `country` — EU member country name (use `EcMossPlaceOfSupply`; same values as `place_of_supply` on MOSS documents).
- `date` — Transaction date.

## Models

| Property | Type | Wire field |
|----------|------|------------|
| `Percentage` | `decimal?` | `percentage` |
| `Band` | `string?` | `band` |

## MOSS writes on other resources

When `ec_status` is `EC VAT MOSS`:

- **Invoices, estimates, credit notes** — `place_of_supply` (`EcMossPlaceOfSupply`) is required on create and update (validated by the SDK before HTTP).
- **Bank transaction explanations** — `place_of_supply` and `sales_tax_rate` are required on create and update when MOSS applies.

Use `GetSalesTaxRatesAsync` with the same country and transaction date to choose a `sales_tax_rate` for bank explanations (and to inform invoice line rates).
