---
title: CIS bands
parent: API coverage
nav_order: 21
---

# CIS bands

List Construction Industry Scheme bands configured for the company. Only available to UK companies enrolled in CIS for Subcontractors.

| | |
|---|---|
| **SDK service** | `client.CisBands` (`CisBandsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/cis_bands](https://dev.freeagent.com/docs/cis_bands) |
| **Sample app** | `/cis-bands` (list) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<CisBand>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/cis_bands`

**Returns:** All CIS bands selected in CIS for Subcontractors settings (not paginated).

**Sample:**

```csharp
var bands = await client.CisBands.ListAsync();
foreach (var band in bands)
{
    Console.WriteLine($"{band.Name} deduction rate {band.DeductionRate}");
}
```

## Models

### CisBand

| Property | Wire name | Notes |
|----------|-----------|-------|
| `Name` | `name` | `CisDeductionRate` (`cis_gross`, `cis_standard`, `cis_higher`) |
| `DeductionRate` | `deduction_rate` | Decimal fraction (string on wire) |
| `IncomeDescription` | `income_description` | |
| `DeductionDescription` | `deduction_description` | |
| `NominalCode` | `nominal_code` | Category nominal code for the band |

Band names match `CisDeductionRate` on [contacts](contacts.md) and CIS fields on [invoices](invoices.md) and [bills](bills.md).
