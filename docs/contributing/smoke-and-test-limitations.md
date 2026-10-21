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
| `CreateCapitalAssetDisposalBankTransactionExplanationAsync` | Needs a disposed-asset id; no capital-asset resource in the SDK. |
| `CreateLandlordPropertyBankTransactionExplanationAsync` | Needs a property URI; no property resource in the SDK. |
| Recurring invoice writes | API is read-only; see [recurring invoices](../reference/recurring-invoices.md). |
| Stock item writes | Read-only API; SDK exposes list and get only. |
| Bank feed connect or refresh | Bank-aggregator OAuth; out of SDK scope; see [bank feeds](../reference/bank-feeds.md). |
| `ConvertEstimateToInvoiceAsync` / `ConvertToCreditNoteAsync` in smoke | Creates converted documents that are not deleted in a simple probe lifecycle; kept menu-only. |
| `SendCreditNoteEmailAsync` on an existing credit note | Sends email for Turpinverse canon data; trial limits; no probe create/delete (unlike invoice send on a draft probe). |

**Unit tests:** create variants for bank transaction explanations and `UpdateBankTransactionExplanationAsync` are covered in `BankTransactionExplanationsServiceTests`. Live smoke for most explanation **shapes** beyond category create-and-delete is still optional follow-up.

**Menu-only by design (not API-blocked):**

- **Stream all …** — full pagination; rate-limit risk on `--run-all`.
- **Linked resources** — use `--verify-linked-resources` instead.
- **Take direct debit payment** — needs a live mandate; see table above.
- **Convert estimate to invoice**, **convert invoice to credit note**, **convert estimate to credit note** — leaves non-deletable converted documents.
- **Send credit note email (template)** on the first existing credit note — no probe lifecycle; trial limits on canon data.
- **Update current user opening mileage** — mutates the CI OAuth user even with restore.
- Estimate and credit note **PDF** downloads — optional; invoice PDF is already in smoke.

## Turpinverse and sandbox data gaps

Turpinverse seeds contacts, projects, tasks, sales documents, notes, and timeslips. It does **not** yet provide everything smoke examples need. Planned Turpinverse work (not this repository’s smoke PR):

| Data needed for smoke | Current state | Suggested action |
|----------------------|---------------|------------------|
| Expenses (with optional attachment) | Not seeded | Expense seeder (user, admin category, small receipt base64). |
| Expenses with attachments | `GetFirstAttachmentIdAsync` scans up to 500 expenses | At least one canon expense with an attachment on create. |
| Bank transaction explanations | Not seeded | One category explanation or manual line on the company bank account. |
| Credit note reconciliations | Not seeded | Optional canon reconciliation, or rely on create probe in `--run-all`. |
| Price list items | Not seeded | Turpinverse canon PLI optional; console **get/update** create probe `SDK-CONSOLE-PLI` when the list is empty. |
| Hire purchases | Not seeded | Turpinverse HP bill optional; console **get detail** and hire-purchase bill probe fetch HP before bill delete. |
| Recurring invoices | API cannot create | Manual sandbox UI or accept skip on get-by-id when list is empty. |
| Stock items | Read-only API | Manual catalogue in sandbox or accept skip on get-by-id. |
| Bank feeds | Connection out of scope | List-only smoke; get when empty prints a message and passes. |

## Conditional skips (account configuration)

Some examples run in `--run-all` but may **skip** when the sandbox account lacks configuration:

| Example | Skip reason |
|---------|-------------|
| Send invoice email (template) | FreeAgent free-trial email limit |
| Send estimate email (template) | FreeAgent free-trial email limit |
| Mark invoice as scheduled | Missing invoice email template or account cannot schedule |
| Update credit note reconciliation | Probe gross value ≤ 1 (cannot decrement for update test) |
| Update/delete estimate item | Created item has no id in API response |

## Credit note reconciliation update

When probe gross value is 1 or less, the update example cleans up the created reconciliation and skips with `probe gross value cannot be decremented for update test`.
