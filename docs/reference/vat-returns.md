---
title: VAT returns
parent: API coverage
nav_order: 27
---

# VAT returns

Read company VAT return periods, filing status, payments, and breakdown details. Mark-as transitions update live return state.

| | |
|---|---|
| **SDK service** | `client.VatReturns` (`VatReturnsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/vat_returns](https://dev.freeagent.com/docs/vat_returns) |
| **Sample app** | `/vat-returns` (list/detail GET), `/vat-returns/detail` (transitions) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<VatReturn>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/vat_returns`

### GetVatReturnAsync

```csharp
Task<VatReturn> GetVatReturnAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/vat_returns/:period_ends_on`

The path segment uses the period end date (`YYYY-MM-DD`).

### MarkAsFiledAsync

```csharp
Task<VatReturn> MarkAsFiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/vat_returns/:period_ends_on/mark_as_filed`

Minimum access level: **Full Access**.

### MarkAsUnfiledAsync

```csharp
Task<VatReturn> MarkAsUnfiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/vat_returns/:period_ends_on/mark_as_unfiled`

### MarkPaymentAsPaidAsync

```csharp
Task<VatReturn> MarkPaymentAsPaidAsync(
    DateOnly periodEndsOn,
    DateOnly paymentDueOn,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/vat_returns/:period_ends_on/payments/:due_on/mark_as_paid`

### MarkPaymentAsUnpaidAsync

```csharp
Task<VatReturn> MarkPaymentAsUnpaidAsync(
    DateOnly periodEndsOn,
    DateOnly paymentDueOn,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/vat_returns/:period_ends_on/payments/:due_on/mark_as_unpaid`

## Models

`VatReturn` includes period dates, `payments`, filing metadata, and optional `breakdown` rows. Payment rows use `VatReturnPayment` with `DueOn` and `VatReturnPaymentStatus`.
