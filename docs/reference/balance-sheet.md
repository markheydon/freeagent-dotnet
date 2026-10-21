---
title: Balance sheet
parent: API coverage
nav_order: 24
---

# Balance sheet

Read balance sheet figures as at a date, or opening balances for the opening balances period.

| | |
|---|---|
| **SDK service** | `client.BalanceSheet` (`BalanceSheetService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/balance_sheet](https://dev.freeagent.com/docs/balance_sheet) |
| **Sample app** | `/balance-sheet` |

## Methods

### GetAsync

```csharp
Task<BalanceSheet> GetAsync(
    DateOnly? asAtDate = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/balance_sheet`

Optional query parameter: `as_at_date` (`YYYY-MM-DD`). When omitted, FreeAgent returns figures as at the current date from the start of the ongoing annual accounting period.

### GetOpeningBalancesAsync

```csharp
Task<BalanceSheet> GetOpeningBalancesAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/balance_sheet/opening_balances`

Opening balances responses omit `accounting_period_start_date` and `as_at_date`.

## Models

`BalanceSheet` includes period dates (when applicable), `currency`, capital and current asset sections, liabilities, equity, and rolled-up totals. Account lines use `name`, `nominal_code`, and `total_debit_value`.
