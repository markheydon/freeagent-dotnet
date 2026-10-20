---
title: Bank accounts
parent: API coverage
nav_order: 8.4
---

# Bank accounts

Typed access to company bank accounts for listing, fixture creation, and remittance link hydration.

| | |
|---|---|
| **SDK service** | `client.BankAccounts` (`BankAccountsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/bank_accounts](https://dev.freeagent.com/docs/bank_accounts) |
| **Sample app** | `/bank-accounts` (list), `/bank-accounts/detail` (CRUD) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<BankAccount>> ListAsync(
    string? view = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `view` | `string` | No | `null` | `view` (`standard_bank_accounts`, `credit_card_accounts`, `paypal_accounts`) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bank_accounts`

**Returns:** All bank accounts for the company (not paginated).

**Sample:**

```csharp
using FreeAgent.Client.Models.BankAccounts;

var bankAccounts = await client.BankAccounts.ListAsync(BankAccountViews.StandardBankAccounts);
```

---

### GetBankAccountAsync

```csharp
Task<BankAccount> GetBankAccountAsync(long bankAccountId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankAccountId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bank_accounts/:id`

**Sample:**

```csharp
var bankAccount = await client.BankAccounts.GetBankAccountAsync(1);
```

---

### CreateStandardBankAccountAsync

```csharp
Task<BankAccount> CreateStandardBankAccountAsync(
    CreateStandardBankAccountRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_accounts` with `type` set to `StandardBankAccount`.

---

### CreateCreditCardBankAccountAsync

```csharp
Task<BankAccount> CreateCreditCardBankAccountAsync(
    CreateCreditCardBankAccountRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_accounts` with `type` set to `CreditCardAccount` and `account_number` (last four digits).

---

### CreateEcommerceBankAccountAsync

```csharp
Task<BankAccount> CreateEcommerceBankAccountAsync(
    CreateEcommerceBankAccountRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_accounts` with `type` set to `EcommerceAccount`.

---

### UpdateBankAccountAsync

```csharp
Task<BankAccount> UpdateBankAccountAsync(
    long bankAccountId,
    UpdateBankAccountRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/bank_accounts/:id`

---

### DeleteBankAccountAsync

```csharp
Task DeleteBankAccountAsync(long bankAccountId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/bank_accounts/:id`

---

## Models

### BankAccount

| Property | Type | Wire field |
|----------|------|------------|
| `Url` | `string` | `url` |
| `ResourceId` | `long` | parsed from `url` |
| `Type` | `BankAccountType?` | `type` |
| `Name` | `string?` | `name` |
| `Currency` | `CurrencyCode?` | `currency` |
| `IsPersonal` | `bool?` | `is_personal` |
| `IsPrimary` | `bool?` | `is_primary` |
| `Status` | `BankAccountStatus?` | `status` |
| `BankName` | `string?` | `bank_name` |
| `OpeningBalance` | `decimal?` | `opening_balance` |
| `BankCode` | `string?` | `bank_code` |
| `CurrentBalance` | `decimal?` | `current_balance` |
| `LatestActivityDate` | `DateOnly?` | `latest_activity_date` |
| `CreatedAt` | `DateTimeOffset?` | `created_at` |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` |
| `BankGuessEnabled` | `bool?` | `bank_guess_enabled` |
| `AccountNumber` | `string?` | `account_number` |
| `SortCode` | `string?` | `sort_code` |
| `SecondarySortCode` | `string?` | `secondary_sort_code` |
| `Iban` | `string?` | `iban` |
| `Bic` | `string?` | `bic` |
| `Email` | `string?` | `email` |

### Write requests

Use `CreateStandardBankAccountRequest.Create`, `CreateCreditCardBankAccountRequest.Create`, `CreateEcommerceBankAccountRequest.Create`, and `UpdateBankAccountRequest.Create` factory methods. The SDK sets creatable `type` values; callers must not supply them.
