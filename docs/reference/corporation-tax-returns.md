---
title: Corporation tax returns
parent: API coverage
nav_order: 28
---

# Corporation tax returns

Read corporation tax return periods for a limited company, including amounts due, deadlines, and filing or payment status. Mark returns as filed, unfiled, paid, or unpaid when your OAuth access level allows.

| | |
|---|---|
| **SDK service** | `client.CorporationTaxReturns` (`CorporationTaxReturnsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/corporation_tax_returns](https://dev.freeagent.com/docs/corporation_tax_returns) |
| **Sample app** | `/corporation-tax-returns`, `/corporation-tax-returns/detail` |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<CorporationTaxReturn>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/corporation_tax_returns`

### GetCorporationTaxReturnAsync

```csharp
Task<CorporationTaxReturn> GetCorporationTaxReturnAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/corporation_tax_returns/:period_ends_on`

The path segment uses the period end date (`YYYY-MM-DD`).

### MarkAsFiledAsync

```csharp
Task<CorporationTaxReturn> MarkAsFiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/corporation_tax_returns/:period_ends_on/mark_as_filed`

Requires **Full Access** (non practice-managed) or **Account Manager** (practice-managed), per FreeAgent docs.

### MarkAsUnfiledAsync

```csharp
Task<CorporationTaxReturn> MarkAsUnfiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/corporation_tax_returns/:period_ends_on/mark_as_unfiled`

### MarkAsPaidAsync

```csharp
Task<CorporationTaxReturn> MarkAsPaidAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/corporation_tax_returns/:period_ends_on/mark_as_paid`

### MarkAsUnpaidAsync

```csharp
Task<CorporationTaxReturn> MarkAsUnpaidAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/corporation_tax_returns/:period_ends_on/mark_as_unpaid`

## Models

`CorporationTaxReturn` includes period dates, `amount_due`, `payment_due_on`, optional `payment_status`, filing metadata (`filing_due_on`, `filing_status`, `filed_at`, `filed_reference`), and the resource `url`.

Enumerations:

- `CorporationTaxReturnFilingStatus` — `draft`, `unfiled`, `pending`, `rejected`, `filed`, `marked_as_filed`
- `CorporationTaxReturnPaymentStatus` — `unpaid`, `marked_as_paid`, `paid` (reserved for future use)

## Sandbox note

Corporation tax returns apply to UK limited companies. Sole traders and other company types may return an empty list; sample apps document this and skip mutating examples when CT is unavailable.
