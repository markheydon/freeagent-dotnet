---
title: Smoke and test limitations
parent: Contributing
nav_order: 4
---

# Smoke and test limitations

This page records **what the console `--run-all` smoke harness and unit tests cannot cover today**, and why. Use it when planning follow-up work (Turpinverse seeders, new SDK resources, or manual sandbox setup).

For how CI smoke runs, see [CI sandbox smoke](ci-sandbox-smoke.md). For which examples are menu-only, see [Console sample README](../../samples/FreeAgent.Client.ConsoleSample/README.md).

## How smoke decides what runs

| Mechanism | Effect |
|-----------|--------|
| `MutatesData = true` | Write example. Interactive menu allows writes on **sandbox** only, unless `--allow-production-writes` is set. |
| `ExcludeFromRunAll = true` | Example is **not** included in `--run-all` (including CI). |
| `SandboxWriteGuard` | `--run-all`, `--seed-turpinverse`, and `--verify-linked-resources` **refuse non-sandbox** environments. There is no production escape hatch for smoke. |

`--allow-production-writes` applies to the **interactive menu only** and cannot be combined with `--run-all`.

## Cannot implement (API, SDK, or external dependency)

These gaps are not fixed by seeding alone; they need API capability, new SDK resources, or out-of-scope integration.

| Area | Reason |
|------|--------|
| `TakeDirectDebitPaymentAsync` | Requires an invoice and contact with a live direct-debit mandate; not creatable via the public API. |
| `CreateStockBankTransactionExplanationAsync` | Needs a stock item id; [stock items](../reference/stock-items.md) are read-only in the SDK. |
| `CreateCapitalAssetDisposalBankTransactionExplanationAsync` | Needs a disposed capital asset id (`client.CapitalAssets` with `view=disposed`, or dispose in the sandbox UI). Smoke does not create or dispose assets end-to-end. |
| `CreateLandlordPropertyBankTransactionExplanationAsync` | Needs a property URI and a `UkUnincorporatedLandlord` company; kept menu-only (not in `--run-all`). |
| Recurring invoice writes | API is read-only; see [recurring invoices](../reference/recurring-invoices.md). |
| Stock item writes | Read-only API; SDK exposes list and get only. |
| Bank feed connect or refresh | Bank-aggregator OAuth; out of SDK scope; see [bank feeds](../reference/bank-feeds.md). |
| `ConvertEstimateToInvoiceAsync` / `ConvertToCreditNoteAsync` in smoke | Creates converted documents that are not deleted in a simple probe lifecycle; kept menu-only. |
| `SendCreditNoteEmailAsync` on an existing credit note | Sends email for Turpinverse canon data; trial limits; no probe create/delete (unlike invoice send on a draft probe). |
| Payroll RTI periods and payslips | Read-only API; pay runs are created in the FreeAgent UI, not via the SDK. Turpinverse cannot seed them today. |

**Unit tests:** create variants for bank transaction explanations and `UpdateBankTransactionExplanationAsync` are covered in `BankTransactionExplanationsServiceTests`. Live smoke in `--run-all` exercises category, payment/refund, invoice receipt, bill payment, smart and categorised user payment, transfer, and update probes (with `SampleContext.Skip` when prerequisites are missing). Stock, capital-asset purchase and disposal, landlord property, and opening-balance explanation creates remain menu-only.

**Menu-only by design (not API-blocked):**

- **Stream all …** — full pagination; rate-limit risk on `--run-all`.
- **Linked resources** — use `--verify-linked-resources` instead.
- **Take direct debit payment** — needs a live mandate; see table above.
- **Convert estimate to invoice**, **convert invoice to credit note**, **convert estimate to credit note** — leaves non-deletable converted documents.
- **Send credit note email (template)** on the first existing credit note — no probe lifecycle; trial limits on canon data.
- **Update current user opening mileage** — mutates the CI OAuth user even with restore.
- Estimate and credit note **PDF** downloads — optional; invoice PDF is already in smoke.

## Turpinverse and sandbox data gaps

Turpinverse seeds contacts, projects, tasks, sales documents, bills, notes, and timeslips. It does **not** yet provide everything smoke examples need, and seeded projects are **not** a full FreeAgent “project workspace” (Summary / Expenses tab / rebill mix). Manual sandbox fixtures (for example Palmer Identity Vault used in `plan/FreeAgent Project Example/`) are manufactured for integration testing until canon catches up. Planned Turpinverse work (not this repository’s smoke PR):

| Data needed for smoke | Current state | Suggested action |
|----------------------|---------------|------------------|
| **Fully populated project** (FA UI tabs: invoices, estimates, time, notes, **outgoings** with rebill/unbillable mix) | Canon seeds projects, tasks, sales docs, bills, notes, and timeslips, but not expenses, bank explanations linked to a project, or rebill-to-project / markup / price scenarios across bills, expenses, and banking. Cannot validate `ProjectWorkspace` / outgoings aggregation or Palmer parity without manual data. | Turpinverse canon + seeders: one anchor project (Palmer-equivalent) with direct `project` and `rebill_to_project` links, unbillable costs, billed vs not-yet-rebilled rows, and at least one bank explanation with project rebill. Track with [Project workspace planning](../../plan/PROJECT_WORKSPACE.md). |
| Expenses (with optional attachment) | Not seeded | Expense seeder (user, admin category, small receipt base64). |
| Expenses with attachments | `GetFirstAttachmentIdAsync` scans up to 500 expenses | At least one canon expense with an attachment on create. |
| Bank transaction explanations | Not seeded | One category explanation or manual line on the company bank account. |
| Credit note reconciliations | Not seeded | Optional canon reconciliation, or rely on create probe in `--run-all`. |
| Price list items | Not seeded | Turpinverse canon PLI optional; console **get/update** create probe `SDK-CONSOLE-PLI` when the list is empty. |
| Hire purchases | Not seeded | Turpinverse HP bill optional; console **get detail** and hire-purchase bill probe fetch HP before bill delete. |
| Capital assets | Not seeded | Created via capital expenses or explanations; console **get detail with history** skips when the list is empty. |
| Recurring invoices | API cannot create | Manual sandbox UI or accept skip on get-by-id when list is empty. |
| Stock items | Read-only API | Manual catalogue in sandbox or accept skip on get-by-id. |
| Bank feeds | Connection out of scope | List-only smoke; get when empty prints a message and passes. |
| Payroll periods and payslips (RTI) | Not seeded; [payroll API](../reference/payroll.md) is read-only | Cannot upsert via Turpinverse or SDK. CI sandbox needs at least **two** pay periods with payslips (run payroll in the FreeAgent UI) before **Mark previous liability payment paid then unpaid** can run. Future Turpinverse work: documented sandbox playbook (users, profiles, two filed periods) or upstream fixture when create APIs exist. |

## Conditional skips (account configuration)

Some examples run in `--run-all` but may **skip** when the sandbox account lacks configuration:

| Example | Skip reason |
|---------|-------------|
| List sales tax periods | Company is not a US or universal company type (UK and other types receive `404` from the API) |
| List CIS bands | Company not enrolled in CIS for subcontractors (`cis_subcontractor` and `cis_enabled` are not true) |
| Get CIS settings | CIS not enabled for the company (`cis_enabled` is not true) |
| Send invoice email (template) | FreeAgent free-trial email limit |
| Send estimate email (template) | FreeAgent free-trial email limit |
| Mark invoice as scheduled | Missing invoice email template or account cannot schedule |
| Update credit note reconciliation | Probe gross value ≤ 1 (cannot decrement for update test) |
| Update/delete estimate item | Created item has no id in API response |
| Mark previous liability payment paid then unpaid | Fewer than two payroll periods with payslips, or no unpaid liability payment due between the previous and latest of those periods |
| List payslips for first payroll period | No payroll periods returned for the current UK tax year end |
| Bank transaction explanation create variants (payment/refund, invoice receipt, bill payment, user payments, transfer) | Missing project, invoice, bill, user, or second bank account |
| Update bank transaction explanation | Could not upload a probe bank transaction |
| Round-trip CIS prior deductions update | Subcontractor prior deductions not recorded on the company |
| Get or update sales tax period (probe) | Company is not a US or universal company type |

## Credit note reconciliation update

When probe gross value is 1 or less, the update example cleans up the created reconciliation and skips with `probe gross value cannot be decremented for update test`.
