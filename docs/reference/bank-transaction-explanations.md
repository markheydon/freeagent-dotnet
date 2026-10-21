---
title: Bank transaction explanations
parent: API coverage
nav_order: 8.42
---

# Bank transaction explanations

List, get, create, update, and delete explanations that categorise bank movements and link to invoices, bills, projects, and other resources.

| | |
|---|---|
| **SDK service** | `client.BankTransactionExplanations` (`BankTransactionExplanationsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/bank_transaction_explanations](https://dev.freeagent.com/docs/bank_transaction_explanations) |
| **Console sample** | `FreeAgent.Client.ConsoleSample` — category **Bank transaction explanations** |

Bank account and transaction links: see [Linked resources](../explanation/linked-resources.md). Cluster context: see [Banking entity map](../explanation/api-entity-map/banking.md). Nested explanations on bank transaction GET: see [Bank transactions](bank-transactions.md).

FreeAgent exposes many create shapes on one `POST` route. The SDK maps each documented operation to a dedicated `Create*Async` method and request type.

From 1 December 2026, explanations may return an `attachments` array instead of a singular `attachment`. Use `X-Api-Version: 2026-09-01` to adopt the new behaviour early.

## EC VAT MOSS

When `ec_status` is `EC VAT MOSS`, the SDK requires `place_of_supply` (`EcMossPlaceOfSupply`) and `sales_tax_rate` on create and update before calling the API. Use [EC VAT MOSS sales tax rates](ec-moss-sales-tax-rates.md) to look up rates for the same country and `dated_on`.

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<BankTransactionExplanation>> ListAsync(
    BankAccountReference? bankAccount = null,
    long? bankAccountId = null,
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankAccount` | `BankAccountReference?` | Yes* | `null` | `bank_account` (URI) |
| `bankAccountId` | `long?` | Yes* | `null` | `bank_account` (URI built from id) |
| `fromDate` | `DateOnly?` | No | `null` | `from_date` |
| `toDate` | `DateOnly?` | No | `null` | `to_date` |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` (ISO 8601) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

\* Supply `bankAccount` or `bankAccountId`, not both.

**HTTP:** `GET /v2/bank_transaction_explanations`

This endpoint returns the full collection for the bank account in one response (no pagination parameters).

**Client validation:** a bank account is required.

**Sample:**

```csharp
var explanations = await client.BankTransactionExplanations.ListAsync(
    bankAccountId: bankAccountId,
    fromDate: new DateOnly(2024, 1, 1));
```

---

### GetBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> GetBankTransactionExplanationAsync(
    long bankTransactionExplanationId,
    CancellationToken cancellationToken = default)

Task<BankTransactionExplanation> GetBankTransactionExplanationAsync(
    long bankTransactionExplanationId,
    BankTransactionExplanationGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankTransactionExplanationId` | `long` | Yes | - | path `:id` |
| `options` | `BankTransactionExplanationGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bank_transaction_explanations/:id`

`BankTransactionExplanationGetOptions` exposes `Include*` flags for category, project, bank account, paid invoice, paid credit note, paid bill, paid user, direct contact, transfer bank account, and stock item links.

**Sample:**

```csharp
var explanation = await client.BankTransactionExplanations.GetBankTransactionExplanationAsync(123);
```

---

### CreateCategoryBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateCategoryBankTransactionExplanationAsync(
    CreateCategoryBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Use `CreateCategoryBankTransactionExplanationRequest.ForExistingTransaction(...)` against an unexplained bank transaction, or `WithNewTransaction(...)` to create the bank transaction and explanation together.

**Sample:**

```csharp
var created = await client.BankTransactionExplanations.CreateCategoryBankTransactionExplanationAsync(
    CreateCategoryBankTransactionExplanationRequest.WithNewTransaction(
        bankAccountId: bankAccountId,
        categoryNominalCode: "285",
        datedOn: new DateOnly(2024, 3, 18),
        grossValue: -10m,
        description: "Office supplies"));
```

---

### CreatePaymentOrRefundBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreatePaymentOrRefundBankTransactionExplanationAsync(
    CreatePaymentOrRefundBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Payment or refund with project rebilling. Factory: `CreatePaymentOrRefundBankTransactionExplanationRequest.Create(...)`.

---

### CreateInvoiceReceiptBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateInvoiceReceiptBankTransactionExplanationAsync(
    CreateInvoiceReceiptBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Invoice receipt or credit note refund. Factory: `CreateInvoiceReceiptBankTransactionExplanationRequest.Create(...)`.

---

### CreateBillPaymentBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateBillPaymentBankTransactionExplanationAsync(
    CreateBillPaymentBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Bill payment or bill refund. Factory: `CreateBillPaymentBankTransactionExplanationRequest.Create(...)`.

---

### CreateSmartUserPaymentBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateSmartUserPaymentBankTransactionExplanationAsync(
    CreateSmartUserPaymentBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Money paid to or received from a user without a category. Factory: `CreateSmartUserPaymentBankTransactionExplanationRequest.Create(...)`.

---

### CreateCategorisedUserPaymentBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateCategorisedUserPaymentBankTransactionExplanationAsync(
    CreateCategorisedUserPaymentBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

User payment with a category. Factory: `CreateCategorisedUserPaymentBankTransactionExplanationRequest.Create(...)`.

---

### CreateTransferBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateTransferBankTransactionExplanationAsync(
    CreateTransferBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Transfer between bank accounts. Factory: `CreateTransferBankTransactionExplanationRequest.Create(...)`.

---

### CreateStockBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateStockBankTransactionExplanationAsync(
    CreateStockBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Stock purchase or sale. Factory: `CreateStockBankTransactionExplanationRequest.Create(...)`.

---

### CreateCapitalAssetPurchaseBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
    CreateCapitalAssetPurchaseBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Capital asset purchase. Factory: `CreateCapitalAssetPurchaseBankTransactionExplanationRequest.Create(...)`.

---

### CreateCapitalAssetDisposalBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateCapitalAssetDisposalBankTransactionExplanationAsync(
    CreateCapitalAssetDisposalBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Capital asset disposal. Factory: `CreateCapitalAssetDisposalBankTransactionExplanationRequest.Create(...)`.

---

### CreateLandlordPropertyBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateLandlordPropertyBankTransactionExplanationAsync(
    CreateLandlordPropertyBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Landlord property profit and loss categories. Factory: `CreateLandlordPropertyBankTransactionExplanationRequest.Create(...)`.

Resolve property URIs with [`client.Properties`](properties.md) (`ListAsync`, `GetPropertyAsync`) or `client.Urls.Property(propertyId)` — do not hand-build URIs at the call site when you already have an id.

**Sample:**

```csharp
var propertyId = (await client.Properties.ListAsync()).First().ResourceId;

var explanation = await client.BankTransactionExplanations.CreateLandlordPropertyBankTransactionExplanationAsync(
    CreateLandlordPropertyBankTransactionExplanationRequest.Create(
        bankTransactionId,
        categoryNominalCode,
        propertyId,
        client.Environment,
        datedOn,
        grossValue));
```

---

### CreateOpeningBalanceBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> CreateOpeningBalanceBankTransactionExplanationAsync(
    CreateOpeningBalanceBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/bank_transaction_explanations`

Opening balance debtor or creditor (categories 681 or 796). Factory: `CreateOpeningBalanceBankTransactionExplanationRequest.Create(...)`.

---

### UpdateBankTransactionExplanationAsync

```csharp
Task<BankTransactionExplanation> UpdateBankTransactionExplanationAsync(
    long bankTransactionExplanationId,
    UpdateBankTransactionExplanationRequest request,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankTransactionExplanationId` | `long` | Yes | - | path `:id` |
| `request` | `UpdateBankTransactionExplanationRequest` | Yes | - | `bank_transaction_explanation` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/bank_transaction_explanations/:id`

Set only the properties you want to change; omitted properties are not serialised.

**Sample:**

```csharp
var updated = await client.BankTransactionExplanations.UpdateBankTransactionExplanationAsync(
    123,
    new UpdateBankTransactionExplanationRequest
    {
        Description = "Updated description",
        GrossValue = -12m
    });
```

---

### DeleteBankTransactionExplanationAsync

```csharp
Task DeleteBankTransactionExplanationAsync(
    long bankTransactionExplanationId,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankTransactionExplanationId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/bank_transaction_explanations/:id`

## BankTransactionExplanation model

Key properties:

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `Type` | `BankTransactionExplanationType?` | `type` | Read-only explanation type |
| `DatedOn` | `DateOnly?` | `dated_on` | |
| `Description` | `string?` | `description` | |
| `GrossValue` | `decimal?` | `gross_value` | Bank account currency |
| `EcStatus` | `BankTransactionExplanationEcStatus?` | `ec_status` | |
| `CategoryNominalCode` | `string?` | - | Parsed from `category` link |
| `BankTransactionId` | `long?` | - | Parsed from `bank_transaction` link |
| `BankAccountId` | `long?` | - | Parsed from `bank_account` link |
| `PaidInvoiceId` | `long?` | - | Parsed from `paid_invoice` link |
| `PaidBillId` | `long?` | - | Parsed from `paid_bill` link |
| `ProjectId` | `long?` | - | Parsed from `project` link |
| `Attachment` | `BillAttachment?` | `attachment` | Legacy shape |
| `Attachments` | `List<BillAttachment>?` | `attachments` | API version 2026-09-01+ |
| `CreatedAt` | `DateTimeOffset?` | `created_at` | Read-only |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` | Read-only |
| `Url` | `string` | `url` | |

See the model type for rebilling, transfer, stock, capital asset, property, and opening balance link fields.

## Write requests

All create request types inherit shared fields from `BankTransactionExplanationCreateRequestBase` (`DatedOn`, `GrossValue`, `Description`, VAT fields, and optional `Attachment`).

Category create requires either `bankAccountId` (new transaction) or `bankTransactionId` (existing transaction), plus `categoryNominalCode`.

Other create factories require `bankTransactionId` and the link fields documented on the FreeAgent page for that explanation type.

`UpdateBankTransactionExplanationRequest` accepts optional fields for the explanation attributes FreeAgent allows on update.

## BankTransactionExplanationReference

Use `BankTransactionExplanationReference.ForEnvironment(environment, id)` or `BankTransactionExplanationReference.Parse(uri)` for environment-correct explanation URIs.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
