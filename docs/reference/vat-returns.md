---
title: VAT returns
parent: API coverage
nav_order: 27
---

# VAT returns

Read company VAT return periods, filing status, payments, and breakdown details.

| | |
|---|---|
| **SDK service** | `client.VatReturns` (`VatReturnsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/vat_returns](https://dev.freeagent.com/docs/vat_returns) |
| **Sample app** | `/vat-returns` |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<VatReturn>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/vat_returns`

### GetAsync

```csharp
Task<VatReturn> GetAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/vat_returns/:period_ends_on`

The path segment uses the period end date (`YYYY-MM-DD`).

## Models

`VatReturn` includes period dates, `payments`, filing metadata, and optional `breakdown` rows. Write endpoints (mark as filed, payment status) are not implemented in this SDK release.
