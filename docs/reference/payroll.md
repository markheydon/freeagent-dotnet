---
title: Payroll
parent: API coverage
nav_order: 42
---

# Payroll

Read RTI payroll periods and payslips, and mark HMRC liability payments as paid or unpaid.

| | |
|---|---|
| **SDK service** | `client.Payroll` (`PayrollService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/payroll](https://dev.freeagent.com/docs/payroll) |
| **Sample app** | `/payroll` (tax year), `/payroll/period` (payslips) |

Minimum access level: Tax and Limited Accounting. UK payroll companies only.

## Methods

### ListAsync

```csharp
Task<PayrollTaxYear> ListAsync(int taxYearEnd, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/payroll/:year`

Returns `periods` and `payments` for the tax year end (for example `2026` for April 2025–March 2026).

---

### ListPayslipsAsync

```csharp
Task<PayrollPeriod> ListPayslipsAsync(
    int taxYearEnd,
    int periodNumber,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/payroll/:year/:period`

`periodNumber` is the pay period index within the tax year (0–11).

---

### MarkPaymentAsPaidAsync

```csharp
Task<PayrollTaxYear> MarkPaymentAsPaidAsync(
    int taxYearEnd,
    DateOnly paymentDate,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/payroll/:year/payments/:payment_date/mark_as_paid`

---

### MarkPaymentAsUnpaidAsync

```csharp
Task<PayrollTaxYear> MarkPaymentAsUnpaidAsync(
    int taxYearEnd,
    DateOnly paymentDate,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/payroll/:year/payments/:payment_date/mark_as_unpaid`

---

## Models

| Type | Role |
|------|------|
| `PayrollTaxYear` | Tax year envelope with `Periods` and `Payments` |
| `PayrollPeriod` | Period metadata and optional `Payslips` |
| `PayrollPayslip` | Per-user payslip amounts |
| `PayrollPayment` | Liability payment due date, amount, and status |
| `PayrollPeriodFilingStatus` | Period filing status (`unfiled`, `pending`, `rejected`, `partially_filed`, `filed`) |
| `PayrollPaymentStatus` | Payment status (`unpaid`, `marked_as_paid`, `paid`) |
| `PayrollNiCalculationType` | NI calculation method (`Director`, `Employee`) |
| `PayrollStudentLoanDeductionsPlan` | Student loan plan (`Plan 1`, `Plan 2`) |

Payslip `user` links deserialize to `UserId` / nested `User` using the standard expandable user pattern.

## Sample

```csharp
var taxYearEnd = 2026;
var taxYear = await client.Payroll.ListAsync(taxYearEnd);

var period = await client.Payroll.ListPayslipsAsync(taxYearEnd, 0);
foreach (var payslip in period.Payslips ?? [])
{
    Console.WriteLine($"{payslip.UserId}: {payslip.BasicPay}");
}

// Liability payments are keyed by due date. Pick an unpaid payment that aligns with
// a completed pay period (see console smoke: PayrollLiabilityPaymentProbeSupport).
```

The console `--run-all` example loads payslips per period and only marks a **previous** period’s liability when at least two periods have payslips. See [Smoke and test limitations](../contributing/smoke-and-test-limitations.md).
