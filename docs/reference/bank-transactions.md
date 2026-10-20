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

Nested `bank_transaction_explanations` on GET use the fields documented on the bank transactions page. Create, update, and delete for explanations are handled by [`client.BankTransactionExplanations`](bank-transaction-explanations.md), not this service.

A successful statement upload means FreeAgent accepted the payload. It does not confirm that every line was imported. List the account, or pass `lastUploaded: true`, to see the result.

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<BankTransaction>> ListAsync(
    int page = 1,
    int perPage = 25,
    BankAccountReference? bankAccount = null,
    long? bankAccountId = null,
    BankTransactionView? view = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    bool? lastUploaded = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `bankAccount` | `BankAccountReference?` | Yes* | `null` | `bank_account` (URI) |
| `bankAccountId` | `long?` | Yes* | `null` | `bank_account` (URI built from id) |
| `view` | `BankTransactionView?` | No | `null` | `view` |
| `fromDate` | `DateOnly?` | No | `null` | `from_date` |
| `toDate` | `DateOnly?` | No | `null` | `to_date` |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` (ISO 8601) |
| `lastUploaded` | `bool?` | No | `null` | `last_uploaded` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

\* Supply `bankAccount` or `bankAccountId`, not both.

**HTTP:** `GET /v2/bank_transactions`

**Returns:** One page of bank transactions for the bank account.

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; a bank account is required.

**Sample:**

```csharp
using FreeAgent.Client.Models.BankTransactions;

var page = await client.BankTransactions.ListAsync(
    bankAccountId: bankAccountId,
    view: BankTransactionView.Unexplained);
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<BankTransaction> ListAutoPagingAsync(
    int perPage = 25,
    BankAccountReference? bankAccount = null,
    long? bankAccountId = null,
    BankTransactionView? view = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    bool? lastUploaded = null,
    CancellationToken cancellationToken = default)
```

Accepts the same filters as `ListAsync` (except `page`). Loops until all pages are yielded.

**Sample:**

```csharp
await foreach (var transaction in client.BankTransactions.ListAutoPagingAsync(bankAccountId: bankAccountId))
{
    Console.WriteLine($"{transaction.DatedOn} {transaction.Amount} {transaction.Description}");
}
```

---

### GetBankTransactionAsync

```csharp
Task<BankTransaction> GetBankTransactionAsync(
    long bankTransactionId,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankTransactionId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bank_transactions/:id`

Returns the transaction and any nested `bank_transaction_explanations`.

---

### DeleteBankTransactionAsync

```csharp
Task DeleteBankTransactionAsync(
    long bankTransactionId,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankTransactionId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/bank_transactions/:id`

FreeAgent deletes a transaction only when it is fully unexplained or has no explanations.

The official docs page currently titles this operation "Delete a bank transaction explanation" and shows `DELETE /v2/bank_transaction/:id` (singular). The SDK calls the plural path used by the rest of the resource.

---

### UploadBankStatementAsync

```csharp
Task UploadBankStatementAsync(
    UploadBankStatementRequest request,
    BankAccountReference? bankAccount = null,
    long? bankAccountId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `request` | `UploadBankStatementRequest` | Yes | - | `statement` array |
| `bankAccount` | `BankAccountReference?` | Yes* | `null` | query `bank_account` |
| `bankAccountId` | `long?` | Yes* | `null` | query `bank_account` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

\* Supply `bankAccount` or `bankAccountId`, not both.

**HTTP:** `POST /v2/bank_transactions/statement?bank_account=`

Each line requires `dated_on`. `description`, `amount`, `fitid`, and `transaction_type` are optional on the wire. The SDK omits unset optional fields. FreeAgent defaults an omitted type to `OTHER`.

**Client validation:** `request` must contain at least one statement line.

A successful response means the upload was accepted, not that every line was imported.

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

---

### UploadBankStatementFileAsync

```csharp
Task UploadBankStatementFileAsync(
    UploadBankStatementFileRequest request,
    BankAccountReference? bankAccount = null,
    long? bankAccountId = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `request` | `UploadBankStatementFileRequest` | Yes | - | multipart `statement` file part |
| `bankAccount` | `BankAccountReference?` | Yes* | `null` | query `bank_account` |
| `bankAccountId` | `long?` | Yes* | `null` | query `bank_account` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

\* Supply `bankAccount` or `bankAccountId`, not both.

**HTTP:** `POST /v2/bank_transactions/statement?bank_account=` as `multipart/form-data`

FreeAgent recommends OFX and also accepts QIF and some CSV formats.

**Client validation:** the statement stream must contain data.

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
