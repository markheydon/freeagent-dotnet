---
title: Cashflow
parent: API coverage
nav_order: 26
---

# Cashflow

Read **historic** cashflow for a date range. FreeAgent’s in-product Cashflow chart includes **projected** cashflow; that projection is **not** available on this API. Dates in the future return zero totals (see [FreeAgent cashflow docs](https://dev.freeagent.com/docs/cashflow)).

| | |
|---|---|
| **SDK service** | `client.Cashflow` (`CashflowService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/cashflow](https://dev.freeagent.com/docs/cashflow) |
| **Sample app** | `/cashflow` |

## Methods

### GetCashflowAsync

```csharp
Task<Cashflow> GetCashflowAsync(
    DateOnly fromDate,
    DateOnly toDate,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/cashflow`

Required query parameters: `from_date`, `to_date` (`YYYY-MM-DD`).

## Models

`Cashflow` includes `from`, `to`, `incoming` and `outgoing` direction summaries (each with `total` and `months`), and `balance`.
