---
title: Currency code
parent: API coverage
nav_order: 8
---

# Currency code

Typed ISO 4217 currency codes for wire values on implemented models.

| | |
|---|---|
| **SDK type** | `CurrencyCode` in `FreeAgent.Client.Models.Shared` |
| **FreeAgent docs** | [dev.freeagent.com/docs/currencies](https://dev.freeagent.com/docs/currencies) |

FreeAgent documents supported currency codes but does **not** expose a `/v2/currencies` REST resource. There is no `CurrenciesService`.

## Usage on models

| Model | Property |
|-------|----------|
| `Company` | `Currency` |
| `DirectDebitMandate` | `Currency` |
| `Project` | `Currency` |

Additional resources will use `CurrencyCode` for currency fields as they are implemented.

## Enum values

`CurrencyCode` maps each member to its ISO 4217 wire value (for example `CurrencyCode.Gbp` → `"GBP"`). Use IntelliSense to browse the full set of supported codes.

**Sample:**

```csharp
var company = await client.Company.GetCompanyAsync();
if (company.Currency == CurrencyCode.Gbp)
{
    // ...
}
```
