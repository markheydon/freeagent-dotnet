---
title: Bank transactions
parent: API coverage
nav_order: 8.45
---

# Bank transactions

List, get, and delete bank transactions for a bank account, and upload statements as JSON lines or a file.

| | |
|---|---|
| **SDK service** | `client.BankTransactions` (`BankTransactionsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/bank_transactions](https://dev.freeagent.com/docs/bank_transactions) |
| **Sample app** | `/bank-transactions` (list), `/bank-transactions/detail` (get, delete, uploads) |

Pagination patterns: see [Pagination](../how-to/pagination.md). Bank account links: see [Linked resources](../explanation/linked-resources.md). Cluster context: see [Banking entity map](../explanation/api-entity-map/banking.md).

Nested `bank_transaction_explanations` on GET use the fields documented on the bank transactions page. Create, update, and delete for explanations are a separate resource and are not part of this service.

A successful statement upload means FreeAgent accepted the payload. It does not confirm that every line was imported. List the account, or pass `lastUploaded: true`, to see the result.

## Methods

### ListAsync / ListAutoPagingAsync

**HTTP:** `GET /v2/bank_transactions`

Requires `bank_account` (`bankAccount` or `bankAccountId`). Also supports `page`, `per_page` (max `100`), `view` (`all`, `unexplained`, `explained`, `manual`, `imported`, `marked_for_review`), `from_date`, `to_date`, `updated_since`, and `last_uploaded`.

**Sample:**

```csharp
using FreeAgent.Client.Models.BankTransactions;

var page = await client.BankTransactions.ListAsync(
    bankAccountId: bankAccountId,
    view: BankTransactionView.Unexplained);

await foreach (var transaction in client.BankTransactions.ListAutoPagingAsync(bankAccountId: bankAccountId))
{
    Console.WriteLine($"{transaction.DatedOn} {transaction.Amount} {transaction.Description}");
}
```

### GetBankTransactionAsync

**HTTP:** `GET /v2/bank_transactions/:id`

Returns the transaction and any nested explanations.

### DeleteBankTransactionAsync

**HTTP:** `DELETE /v2/bank_transactions/:id`

FreeAgent deletes a transaction only when it is fully unexplained or has no explanations.

The official docs page currently titles this operation "Delete a bank transaction explanation" and shows `DELETE /v2/bank_transaction/:id` (singular). The SDK calls the plural path used by the rest of the resource.

### UploadBankStatementAsync

**HTTP:** `POST /v2/bank_transactions/statement?bank_account=`

Sends `{ "statement": [ ... ] }`. Each line requires `dated_on`. `description`, `amount`, `fitid`, and `transaction_type` are optional. The SDK omits unset optional fields. FreeAgent defaults an omitted type to `OTHER`.

**Sample:**

```csharp
await client.BankTransactions.UploadBankStatementAsync(
    new UploadBankStatementRequest(
    [
        new BankStatementTransaction
        {
            DatedOn = new DateOnly(2019, 7, 1),
            Amount = -100m,
            Description = "Local Council",
            FitId = "049b807d-83ea-4d98-854c-e84b18775d31"
        }
    ]),
    bankAccountId: bankAccountId);
```

### UploadBankStatementFileAsync

**HTTP:** `POST /v2/bank_transactions/statement?bank_account=` as `multipart/form-data`

Sends the file as the `statement` part. FreeAgent recommends OFX and also accepts QIF and some CSV formats.

## Models

| Model | Role |
|-------|------|
| `BankTransaction` | Transaction returned by list and get |
| `BankTransactionExplanation` | Nested explanation on a transaction |
| `BankTransactionView` | List `view` filter |
| `BankStatementTransaction` | One JSON statement line |
| `BankStatementTransactionType` | Statement `transaction_type` wire values |
| `UploadBankStatementRequest` | JSON statement upload |
| `UploadBankStatementFileRequest` | File statement upload |
