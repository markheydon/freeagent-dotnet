---
title: Cashflow
parent: API coverage
nav_order: 26
---

# Cashflow

Read historic cashflow for a date range.

| | |
|---|---|
| **SDK service** | `client.Cashflow` (`CashflowService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/cashflow](https://dev.freeagent.com/docs/cashflow) |
| **Sample app** | `/cashflow` |

## Methods

### GetAsync

```csharp
Task<Cashflow> GetAsync(
    DateOnly fromDate,
    DateOnly toDate,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/cashflow`

Required query parameters: `from_date`, `to_date` (`YYYY-MM-DD`).

## Models

`Cashflow` includes `from`, `to`, `incoming` and `outgoing` direction summaries (each with `total` and `months`), and `balance`.
