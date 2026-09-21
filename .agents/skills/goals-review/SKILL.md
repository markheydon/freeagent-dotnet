---
name: goals-review
description: Audit the repository (or a PR diff) against GOALS.md, SCOPE.md, CONVENTIONS.md, and AGENTS.md sample-sync rules. Use when the user asks for a goals review, drift check, project health audit, or whether work still aligns with G1–G6.
---

# Goals Review

Run a structured audit to detect drift from project intent. This is a **read-only review** unless the user explicitly asks you to fix findings.

## Required reading (always)

Read these before assessing:

1. [GOALS.md](../../GOALS.md) - G1–G6 and kill criteria
2. [SCOPE.md](../../SCOPE.md) - in/out of scope for v1.0
3. [CONVENTIONS.md](../../CONVENTIONS.md) - coding and SDK patterns
4. [AGENTS.md](../../AGENTS.md) - sample sync, skills routing, allowed actions

## Scope of review

Determine scope from user context:

| Trigger | Scope |
|---------|--------|
| No extra context | Whole repository health check (lightweight sampling for most areas; **exhaustive** for all `*Service.cs` list API naming) |
| PR number or branch | `git diff main...HEAD` plus sample/docs parity for changed resources |
| `@`-mentioned paths | Those paths against relevant goals |

Run `dotnet build` and `dotnet test` when reviewing code changes. Report pass/fail.

## Checklist - map findings to goal IDs

### G1 - Clean, strongly typed SDK

- [ ] New/changed API surfaces use typed models and service methods, not raw `HttpClient` in consumer-facing code
- [ ] Every serialised property has `JsonPropertyName`
- [ ] Documented operation variants have separate request types/methods (not overloaded shapes)

### G2 - Stripe.NET-quality DX

- [ ] Consumers can perform documented writes from IntelliSense without reconstructing payloads from FreeAgent docs
- [ ] Linked resources use flat read properties (`Contact?`, `ContactId`, denormalised names) - not public `ExpandableField<T>` or `.Value`
- [ ] Optional `*GetOptions` hydration on single GET only; list endpoints do not N+1 hydrate
- [ ] Fail-fast only on **official local** contract constraints; no invented business-rule validation
- [ ] README and docs examples show the primary happy path (not wire-format archaeology)

### G2/G3 - List API shape (Stripe.NET parity)

Reference: Stripe.NET `List` / `ListAutoPagingAsync`; this repo uses `ListAsync` / `ListAutoPagingAsync` on resource services (see [docs/how-to/pagination.md](../../docs/how-to/pagination.md), [adr-0012-list-api-naming.md](../../adr/adr-0012-list-api-naming.md), [CONVENTIONS.md](../../CONVENTIONS.md)).

#### Mechanical audit (mandatory)

For **every** `*Service.cs` under `src/FreeAgent.Client/Services/`, build a per-service inventory and flag violations. Do not sample - check all services even in a whole-repo health check.

**Step 1 - enumerate services**

```bash
find src/FreeAgent.Client/Services -name '*Service.cs'
```

**Step 2 - detect forbidden collection-read names**

Search `src/`, `tests/`, `samples/`, and `docs/` for legacy or drift patterns:

```bash
rg 'Get\w+PageAsync|GetAll\w+Async|GetUsersAsync|GetCategoriesAsync|GetContactsAsync|GetProjectsAsync|GetEmailAddressesAsync|GetBusinessCategoriesAsync|GetTaxTimelineAsync' \
  src tests samples docs
```

Flag any hit outside [docs/how-to/upgrading.md](../../docs/how-to/upgrading.md) migration tables and [adr-0012-list-api-naming.md](../../adr/adr-0012-list-api-naming.md) historical context.

Also flag collection reads whose public method name starts with `Get` but is not a single-resource retrieve (for example `GetInvoicesAsync`, `Get*PageAsync`).

**Step 3 - classify each service and verify expected methods**

| API pagination | Required public list methods | Single-resource reads |
|---|---|---|
| Paginated (`PaginatedResponse<T>`) | `ListAsync` **and** `ListAutoPagingAsync` | `Get[Resource]Async` |
| Non-paginated collection | `ListAsync` only (no `ListAutoPagingAsync`) | `Get[Resource]Async` |
| Multiple collections on one service | `List[Descriptor]Async` per endpoint (for example `ListTaxTimelineAsync`) | `Get[Resource]Async` for singletons |

**Step 4 - verify implementation wiring**

For each paginated service:

- [ ] `ListAutoPagingAsync` calls `ListAsync` in its loop (no duplicated HTTP/query logic)
- [ ] `per_page` maximum 100 enforced in `ListAsync`

**Step 5 - verify docs, samples, and tests match SDK names**

Cross-check [docs/reference/api-coverage.md](../../docs/reference/api-coverage.md), [samples/README.md](../../samples/README.md), probe pages, how-to docs, and service tests against the inventory from Step 3. Stale method names are **medium** severity drift.

#### Checklist (apply per service from inventory)

- [ ] Paginated resources expose both `ListAsync` (single page) and `ListAutoPagingAsync`
- [ ] Non-paginated collection endpoints use `ListAsync` (or `List[Descriptor]Async` when one service has multiple lists)
- [ ] Single-resource reads remain `Get*Async`; collection reads are not named `Get*`
- [ ] `ListAutoPagingAsync` delegates to `ListAsync` internally (no duplicate HTTP logic)
- [ ] Cross-resource list naming is consistent (all paginated services use the same pair)
- [ ] Happy-path docs and examples prefer `ListAutoPagingAsync`; `ListAsync` appears for UI paging and explicit batch control
- [ ] `per_page` maximum 100 enforced on paginated list paths

#### Required report section

Include a **List API inventory** table in every goals review:

| Service | Pagination | List method(s) | Auto-paging | Single GET | Status |
|---|---|---|---|---|---|
| `ContactService` | Paginated | `ListAsync` | `ListAutoPagingAsync` | `GetContactAsync` | OK / drift |

### G4 - Safe, stable, boring

- [ ] Error handling uses typed exceptions; no swallowed HTTP failures
- [ ] Breaking changes are documented in [docs/how-to/upgrading.md](../../docs/how-to/upgrading.md) when public API changes

### G5 - Contribution-friendly

- [ ] ADRs exist for architectural decisions; conventions documented in CONVENTIONS.md and skills
- [ ] Public API changes are intentional and discoverable

### G6 - Protocol-level OAuth only

- [ ] No UI/browser OAuth orchestration, token persistence, or callback hosting in the SDK

### Scope boundaries

- [ ] No UI/CLI in the SDK package (`src/FreeAgent.Client`)
- [ ] No business-rule abstraction (VAT logic, accounting opinions)
- [ ] No undocumented/experimental endpoints
- [ ] Blazor sample reflects **implemented** SDK endpoints only (sample sync per AGENTS.md)

### Sample app sync (when SDK services changed)

- [ ] Every changed `src/FreeAgent.Client/Services/**` endpoint has a matching sample probe page
- [ ] Probe pages follow [docs/contributing/sample-probe-pages.md](../../docs/contributing/sample-probe-pages.md)

### Documentation

- [ ] UK English in docs and comments
- [ ] New patterns explained in Diátaxis-appropriate locations (`docs/explanation/`, `docs/how-to/`, etc.)

## Output format

Produce a concise report:

```markdown
# Goals review - <scope>

**Verdict:** Aligned | Minor drift | Significant drift

## Summary
<2–4 sentences>

## Findings

| ID | Severity | Goal | Finding | Evidence |
|----|----------|------|---------|----------|
| F1 | high/medium/low | G2 | ... | file:line or diff |

## Passing checks
- Bullet list of goals/areas with no issues found

## Recommended actions
1. Ordered list - fixes first, then preventive measures
```

Severity guide:

- **high** - violates scope, goal, or public API contract; should block merge
- **medium** - inconsistency or doc/sample drift; fix soon
- **low** - style, minor doc gap, or improvement opportunity

Do not file GitHub issues unless the user asks. Do not edit code unless the user asks you to remediate findings.
