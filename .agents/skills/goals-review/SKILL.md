---
name: goals-review
description: Audit the repository (or a PR diff) against GOALS.md, SCOPE.md, CONVENTIONS.md, and AGENTS.md sample-sync rules. Use when the user asks for a goals review, drift check, project health audit, or whether work still aligns with G1–G6.
---

# Goals Review

Run a structured audit to detect drift from project intent. This is a **read-only review** unless the user explicitly asks you to fix findings.

## Required reading (always)

Read these before assessing:

1. [GOALS.md](../../GOALS.md) — G1–G6 and kill criteria
2. [SCOPE.md](../../SCOPE.md) — in/out of scope for v1.0
3. [CONVENTIONS.md](../../CONVENTIONS.md) — coding and SDK patterns
4. [AGENTS.md](../../AGENTS.md) — sample sync, skills routing, allowed actions

## Scope of review

Determine scope from user context:

| Trigger | Scope |
|---------|--------|
| No extra context | Whole repository health check (lightweight sampling, not every file) |
| PR number or branch | `git diff main...HEAD` plus sample/docs parity for changed resources |
| `@`-mentioned paths | Those paths against relevant goals |

Run `dotnet build` and `dotnet test` when reviewing code changes. Report pass/fail.

## Checklist — map findings to goal IDs

### G1 — Clean, strongly typed SDK

- [ ] New/changed API surfaces use typed models and service methods, not raw `HttpClient` in consumer-facing code
- [ ] Every serialised property has `JsonPropertyName`
- [ ] Documented operation variants have separate request types/methods (not overloaded shapes)

### G2 — Stripe.NET-quality DX

- [ ] Consumers can perform documented writes from IntelliSense without reconstructing payloads from FreeAgent docs
- [ ] Linked resources use flat read properties (`Contact?`, `ContactId`, denormalised names) — not public `ExpandableField<T>` or `.Value`
- [ ] Optional `*GetOptions` hydration on single GET only; list endpoints do not N+1 hydrate
- [ ] Fail-fast only on **official local** contract constraints; no invented business-rule validation
- [ ] README and docs examples show the primary happy path (not wire-format archaeology)

### G3 — Automatic pagination

- [ ] List methods expose page iterators / `GetAll*` helpers; consumers are not forced to manage `page` manually

### G4 — Safe, stable, boring

- [ ] Error handling uses typed exceptions; no swallowed HTTP failures
- [ ] Breaking changes are documented in [docs/how-to/upgrading.md](../../docs/how-to/upgrading.md) when public API changes

### G5 — Contribution-friendly

- [ ] ADRs exist for architectural decisions; conventions documented in CONVENTIONS.md and skills
- [ ] Public API changes are intentional and discoverable

### G6 — Protocol-level OAuth only

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
# Goals review — <scope>

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
1. Ordered list — fixes first, then preventive measures
```

Severity guide:

- **high** — violates scope, goal, or public API contract; should block merge
- **medium** — inconsistency or doc/sample drift; fix soon
- **low** — style, minor doc gap, or improvement opportunity

Do not file GitHub issues unless the user asks. Do not edit code unless the user asks you to remediate findings.
