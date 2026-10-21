---
title: Transactions
parent: API coverage
nav_order: 27
---

# Transactions

Read ledger transactions posted to account categories for a period. Bank statement lines are covered by [Bank transactions](bank-transactions.md).

| | |
|---|---|
| **SDK service** | `client.Transactions` (`TransactionsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/transactions](https://dev.freeagent.com/docs/transactions) |
| **Sample app** | `/transactions`, `/transactions/detail` |

Minimum access level: Tax, Accounting & Users.

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<AccountingTransaction>> ListAsync(
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    string? nominalCode = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/transactions`

Optional query parameters: `from_date`, `to_date` (`YYYY-MM-DD`), and `nominal_code`. When dates are omitted, FreeAgent returns transactions from the start of the current accounting period to the current date. Requested periods must be 12 months or shorter, or fall inside a single accounting year. The endpoint returns the full collection for the period, so there is no auto-paging method.

### GetAsync

```csharp
Task<AccountingTransaction> GetAsync(long transactionId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/accounting/transactions/:id`

## Models

`AccountingTransaction` includes `dated_on` (`DateOnly`), `created_at` and `updated_at` (`DateTimeOffset`), `description`, category link fields (`Category`, `CategoryNominalCode`, `CategoryName`), `nominal_code`, `debit_value` (credits are negative), `source_item_url` with `SourceItemId` when the final path segment is numeric, and `foreign_currency_data` (`AccountingTransactionForeignCurrency` with `CurrencyCode` and a foreign `debit_value`).

```csharp
var monthAgo = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1);
var transactions = await client.Transactions.ListAsync(fromDate: monthAgo, nominalCode: "359");
var detail = await client.Transactions.GetAsync(transactions[0].ResourceId);
```
