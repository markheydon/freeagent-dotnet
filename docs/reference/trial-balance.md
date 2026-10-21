---
title: Trial balance
parent: API coverage
nav_order: 25
---

# Trial balance

Read trial balance summary lines for a date range or opening balances.

| | |
|---|---|
| **SDK service** | `client.TrialBalance` (`TrialBalanceService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/trial_balance](https://dev.freeagent.com/docs/trial_balance) |
| **Sample app** | `/trial-balance` |

## Methods

### GetSummaryAsync

```csharp
Task<IReadOnlyList<TrialBalanceLine>> GetSummaryAsync(
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/trial_balance/summary`

Optional query parameters: `from_date`, `to_date` (`YYYY-MM-DD`). When omitted, FreeAgent returns the summary for the current date.

### GetOpeningBalancesAsync

```csharp
Task<IReadOnlyList<TrialBalanceLine>> GetOpeningBalancesAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/trial_balance/summary/opening_balances`

## Models

`TrialBalanceLine` includes category links (`Category`, `CategoryNominalCode`), `nominal_code`, `display_nominal_code`, `name`, `total`, and optional `bank_account` / `user` links.
