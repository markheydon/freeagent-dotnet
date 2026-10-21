---
title: Profit and loss
parent: API coverage
nav_order: 23
---

# Profit and loss

Read profit and loss summary figures for a date range or accounting period.

| | |
|---|---|
| **SDK service** | `client.ProfitAndLoss` (`ProfitAndLossService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/profit_and_loss](https://dev.freeagent.com/docs/profit_and_loss) |
| **Sample app** | `/profit-and-loss` |

## Methods

### GetSummaryAsync

```csharp
Task<ProfitAndLossSummary> GetSummaryAsync(
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    string? accountingPeriod = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/profit_and_loss/summary`

Optional query parameters: `from_date`, `to_date` (`YYYY-MM-DD`), or `accounting_period` (for example `2022/23`). When omitted, FreeAgent returns the current accounting year to date.

## Models

`ProfitAndLossSummary` includes period dates, income and expense totals, `operating_profit`, optional `less` line items (`title`, `total`), and retained profit figures.
