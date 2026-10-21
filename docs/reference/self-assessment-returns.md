---
title: Self assessment returns
parent: API coverage
nav_order: 30
---

# Self assessment returns

Read income tax return periods for a user, including scheduled payments, deadlines, and filing status. Mark returns or individual payments as filed, unfiled, paid, or unpaid when your OAuth access level allows.

FreeAgent exposes these resources on the `self_assessment_returns` wire path under each user.

| | |
|---|---|
| **SDK service** | `client.SelfAssessmentReturns` (`SelfAssessmentReturnsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/self_assessment_returns](https://dev.freeagent.com/docs/self_assessment_returns) |
| **Sample app** | `/self-assessment-returns`, `/self-assessment-returns/detail` |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<SelfAssessmentReturn>> ListAsync(long userId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/users/:user_id/self_assessment_returns`

### GetSelfAssessmentReturnAsync

```csharp
Task<SelfAssessmentReturn> GetSelfAssessmentReturnAsync(long userId, DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/users/:user_id/self_assessment_returns/:period_ends_on`

The path segment uses the period end date (`YYYY-MM-DD`).

### MarkAsFiledAsync

```csharp
Task<SelfAssessmentReturn> MarkAsFiledAsync(long userId, DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/users/:user_id/self_assessment_returns/:period_ends_on/mark_as_filed`

Requires **Full Access**, per FreeAgent docs.

### MarkAsUnfiledAsync

```csharp
Task<SelfAssessmentReturn> MarkAsUnfiledAsync(long userId, DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/users/:user_id/self_assessment_returns/:period_ends_on/mark_as_unfiled`

### MarkPaymentAsPaidAsync

```csharp
Task<SelfAssessmentReturn> MarkPaymentAsPaidAsync(
    long userId,
    DateOnly periodEndsOn,
    DateOnly paymentDate,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/users/:user_id/self_assessment_returns/:period_ends_on/payments/:payment_date/mark_as_paid`

The payment path segment uses the payment due date (`YYYY-MM-DD`).

### MarkPaymentAsUnpaidAsync

```csharp
Task<SelfAssessmentReturn> MarkPaymentAsUnpaidAsync(
    long userId,
    DateOnly periodEndsOn,
    DateOnly paymentDate,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/users/:user_id/self_assessment_returns/:period_ends_on/payments/:payment_date/mark_as_unpaid`

## Models

`SelfAssessmentReturn` includes period dates, a `payments` array (`SelfAssessmentReturnPayment`), filing metadata (`filing_due_on`, `filing_status`, `filed_at`, `filed_reference`), and the resource `url`.

Enumerations:

- `SelfAssessmentReturnFilingStatus` — `unfiled`, `pending`, `rejected`, `provisionally_filed`, `filed`, `marked_as_filed`
- `SelfAssessmentReturnPaymentStatus` — `unpaid`, `marked_as_paid`, `paid` (reserved for future use)

## Sandbox note

Self assessment returns are user-scoped. Sole traders and partnerships are the typical audience; limited-company sandboxes may return an empty list. Sample apps resolve the authenticated user via `users/me` and skip mutating `--run-all` examples when no returns are available.
