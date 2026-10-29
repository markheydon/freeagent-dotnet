# Project workspace and linked data

**Status:** Pre-issue planning (not committed until owner approves)  
**Primary fixture:** Sandbox *Palmer Identity Vault* — screenshots and notes under [`plan/FreeAgent Project Example/`](FreeAgent%20Project%20Example/) (project id `50651` in URLs at time of capture)  
**Turpinverse:** Does not yet seed an equivalent fully populated project; see [Turpinverse and sandbox data gaps](../docs/contributing/smoke-and-test-limitations.md#turpinverse-and-sandbox-data-gaps).

## Problem statement

FreeAgent’s **project UI** presents cohesive tabs (Summary, Invoices, Estimates, Time, **Expenses**, Profit & Loss, …). The **REST API** exposes the same business data as separate resources with different link shapes, list filters, and amount fields. SDK consumers must discover that mapping themselves and orchestrate multiple paginated calls (plus banking fan-out) to approximate one UI tab.

This is not a wire-modelling bug: types match the API. It is an **integration ergonomics** gap (G2) while respecting G1, G4, and SCOPE (no accounting policy, no alternate domain model).

## Non-goals (v1)

- Replacing or extending the `Project` resource model with Summary/P&amp;L totals (no FA GET for those aggregates).
- Certifying statutory or management accounts; any FA UI parity notes are **documented observations**, not SDK guarantees.
- Attachment file download orchestration inside workspace load.
- Hire purchases as a separate slice (derive from bills when needed).
- Partial workspace on failure (see **Fail fast** below).
- Production of rebill-at **calculated** amounts in code unless explicitly ADR’d; prefer documenting inputs (`rebill_type`, `rebill_factor`, cost fields) and optional sample code.

## Palmer checklist (downstream feedback)

| Item | Meaning | SDK today |
|------|---------|-----------|
| **Rebill-to-project inclusion** | Outgoings rebilled *to* a project use `rebill_to_project` as well as `project`. UI Expenses tab mixes bills, expenses, bank explanations. | Models + hydration exist. List APIs document `?project=` only; bank explanations list by bank account only. |
| **Normalised cost amounts** | UI **Cost** vs **Rebill at** (e.g. bill net £2,920 vs rebill £3,212 with markup). | Raw decimals per type; no cross-resource guide in docs yet (include in this story). |
| **Alignment with FA summary totals** | Summary “Minus Expenses” uses **costs**; Expenses tab “Not Billed Yet” uses **rebill-at** sums. | No helper; document Palmer arithmetic as reference. |
| **FA UI tab → API sources** | Matrix mapping tabs to endpoints and link fields. | Partial entity-map coverage only. |

## Rebill — not a missing model

Rebill fields and GET hydration exist on bills, expenses, and payment/refund bank explanations. The gap is **discovery, multi-source aggregation, banking fan-out, and documentation** — not a separate “implement rebill” story.

## Resource inventory

“List by project?” = documented `project` query on the list endpoint.

| Cluster | Resource | Link fields | List `?project=` | Notes |
|---------|----------|-------------|------------------|-------|
| Projects & time | Tasks | `project` | Yes | |
| | Timeslips | `project`, `task` | Yes | `billed_on_invoice` when billed |
| | Notes | `parent_url` → project | Yes | |
| Sales | Invoices | `project`, line `project` | Yes | |
| | Estimates | `project` | Yes | |
| | Recurring invoices | `project` | Yes | Read-only create API |
| | Credit notes | `project`, line `project` | Yes | |
| Purchases | Bills | `project`, `rebill_to_project`, line `project` | Yes | `rebilled_on_invoice_item` |
| | Expenses | `project`, `rebill_to_project` | Yes | `rebilled_on_invoice` |
| Banking | Bank transaction explanations | `project`, `rebill_*` | **No** | List by `bank_account` |
| | Bank transactions | nested explanations | No | Scan or use explanations list |
| Other | Hire purchases | via bill | N/A | Read-only |

## Sandbox spike (block implementation details)

Run against Palmer (`50651`) or successor Turpinverse anchor project. Record results in the table below before locking filter logic.

| Check | Expected question | Result (TBD) |
|-------|-------------------|----------------|
| Bills `?project=` | Includes rows where only `rebill_to_project` matches? | |
| Bills `?project=` | Includes header vs line-level `project` only? | |
| Expenses `?project=` | Same for `rebill_to_project`? | |
| Bank explanations | Count per account when filtered by `ProjectId` | |
| Duplicates | Same economic event in more than one collection? | |

## Public API shape (agreed direction)

### Slice types

Partial loads; typed **collections per resource** (no primary `switch` on source kind).

| Type | Contents | FA UI tab |
|------|----------|-----------|
| `ProjectOutgoings` | `Bills`, `Expenses`, `BankExplanations` | Expenses |
| `ProjectIncome` | `Invoices`, `CreditNotes` | Invoices |
| `ProjectPipeline` | `Estimates`, `RecurringInvoices` | Estimates |
| `ProjectTime` | `Tasks`, `Timeslips` | Tasks, Time |
| `ProjectNotes` | `Notes` | Notes |

Optional later: thin wrappers per item (`ProjectLinkedBill`, …) for documented cost/rebill field mapping without mutating `Bill`.

**Link scope** (outgoings): `ProjectLinkScope` — `DirectProject | RebillToProject | All` (default `All` after spike confirms semantics).

**Banking:** `ProjectOutgoingsLoadOptions` must accept explicit `bankAccountIds` or a documented strategy (e.g. all accounts from `BankAccounts.ListAutoPagingAsync`).

### Top-level bundle

- **Type:** `ProjectWorkspace`
- **Load:** `ProjectService.LoadWorkspaceAsync(projectId, ProjectWorkspaceLoadOptions, ct)`
- **Slices:** `GetOutgoingsAsync`, `GetIncomeAsync`, … on `ProjectService` — each slice loader is reused by `LoadWorkspaceAsync` (no duplicated HTTP logic).

```csharp
public sealed class ProjectWorkspace
{
    public Project Project { get; init; }
    public ProjectOutgoings? Outgoings { get; init; }
    public ProjectIncome? Income { get; init; }
    public ProjectPipeline? Pipeline { get; init; }
    public ProjectTime? Time { get; init; }
    public ProjectNotes? Notes { get; init; }
}

[Flags]
public enum ProjectWorkspaceSections { None = 0, Outgoings = 1, Income = 2, Pipeline = 4, Time = 8, Notes = 16, All = … }
```

**Fail fast:** If any **requested** section fails, the whole `LoadWorkspaceAsync` throws; no half-filled workspace.

**Pagination:** Slice loaders use existing `ListAutoPagingAsync` (G3). Document that `Sections = All` may be heavy (rate limits); callers should trim flags.

**Placement:** Composite helpers on **`ProjectService`** (`client.Projects`), not a parallel entry point.

**New public types:** `ProjectWorkspace`, slice types, load options, and `ProjectLinkScope` require an **ADR** before merge (view layer over existing resources).

## Deliverables by tier

| Tier | Deliverable |
|------|-------------|
| **A — Docs** | Explanation: project UI tabs → API sources (matrix + Palmer). Explanation subsection or page: monetary values on read models (pass-through, signs, which field is “cost” per resource). Extend projects entity map outbound links. |
| **B — Code** | Slice loaders + `LoadWorkspaceAsync`; shared internal link-matching helper; unit tests with wire fixtures; optional integration tests when Turpinverse anchor exists. |
| **C — Defer** | Unified timeline list, billing buckets (`NotBilledYet` / …), SDK-computed rebill totals — only if A+B insufficient. |

## Sample sync (when implementing)

Per `AGENTS.md`: exercise new `ProjectService` methods from **both** Blazor workbench (probe page under Projects) and console `ProjectSamples` (or dedicated provider). Do not add sample coverage beyond what the SDK exposes.

## Open questions

1. Spike outcomes for `?project=` vs `rebill_to_project` (see table above).
2. Default bank account discovery: all accounts vs require explicit ids in options.
3. Compute `rebill_at` in SDK vs docs-only (default: docs-only for v1).
4. Invoice/credit note line-level `project` without header project — include in income slice?

## GitHub issue (draft)

**Title:** `[Story] Project workspace loaders, outgoings aggregation, and FA UI→API docs`

**Labels:** `type/story`; link parent feature cluster if one exists for Projects.

**Body summary:**

- Implement `ProjectWorkspace` + slice types and `ProjectService` loaders (fail-fast).
- Document tab → API mapping and monetary field semantics.
- Complete Palmer (or Turpinverse) spike; record filter semantics.
- ADR for new public types.
- Tests: wire fixtures; live sandbox when Turpinverse gap closed.
- Sample sync both apps.

**Acceptance criteria:**

- [ ] Spike table in this plan updated with results.
- [ ] ADR merged for workspace public API.
- [ ] Explanation docs published under `docs/explanation/`.
- [ ] `ProjectService` slice + workspace methods with tests.
- [ ] Blazor + console sample coverage.
- [ ] Turpinverse “fully populated project” tracked separately (may block live parity tests only).

## Related governance

- **G2, G3** — discoverability and pagination.
- **SCOPE** — compose API resources; do not encode VAT/profit policy.
- **GOALS** — prefer existing `Bill`, `Expense`, `BankTransactionExplanation` inside slice collections.
