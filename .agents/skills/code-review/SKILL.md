---
name: code-review
description: >-
  Review FreeAgent.NET SDK changes as a principal architect would for pull
  request approval: bugs, security, public API contract, architecture,
  complexity, and maintainability only. Use when the user runs /code-review or
  loop-code-review, asks for a code review, or wants findings on a branch, PR,
  or local diff before merge. Read-only unless the user explicitly asks to
  implement fixes.
---

# Code review

Review as a principal architect and senior developer. Treat the change set like a pull request from another experienced developer.

Focus only on issues that would influence your decision to **approve or reject** the pull request. Findings must be the primary focus, ordered by severity. Do not make code changes unless the user explicitly asks for them.

This skill is the default in-place review for the FreeAgent.NET SDK. For automated Bugbot or security-focused subagent reviews, use `/review-bugbot` and `/review-security` unless the user wants this review only.

For drift against [GOALS.md](../../GOALS.md), [SCOPE.md](../../SCOPE.md), sample-sync rules, or the mechanical List API inventory, use the `goals-review` skill instead — do not duplicate that audit here unless a finding also blocks merge on correctness or maintainability grounds.

## Scope

Decide what to diff before reading code:

| User intent | Diff |
| --- | --- |
| Default (branch work, PR, “review my changes”) | Changes since merge-base with the repository default branch (usually `main`), including committed, staged, and unstaged edits |
| “Uncommitted”, “dirty”, “working tree” only | Staged + unstaged diff only |
| Named PR, branch, or GitHub link | Check out that head branch locally when needed, then use the branch diff row above |

Repository path: use the git root that contains the changes (may be a subfolder of the workspace).

If there is no diff, say so in one sentence and stop.

## Focus on finding

1. Bugs, defects, or logic errors (including HTTP/JSON mapping mistakes).
2. Security concerns (secrets in samples or tests, unsafe logging of tokens, auth handling).
3. Violations of SDK conventions — ground these in [CONVENTIONS.md](../../CONVENTIONS.md), [GOALS.md](../../GOALS.md), [SCOPE.md](../../SCOPE.md), and relevant [adr/](../../adr/) records.
4. Public API contract mistakes: wrong list/read naming (`ListAsync` / `ListAutoPagingAsync` vs `Get*` collections), missing operation variants, leaked `Infrastructure` types, invented validation beyond documented local constraints, or `ExpandableField<T>` on public surfaces.
5. Architectural issues that will become expensive to fix later (layer bleed, new patterns without an ADR when required).
6. Code that is unnecessarily complex.
7. Maintainability or testability — including missing or inadequate tests when **behaviour** changed, and missing **sample sync** when `src/FreeAgent.Client/Services/` changed (both Blazor probe pages and console example providers per [AGENTS.md](../../AGENTS.md)).

## Do not comment on

- Formatting issues (CI and `dotnet format` own formatting).
- Naming preferences unless they materially affect readability or public API discoverability.
- Personal coding style preferences.
- Minor optimisations without measurable value.
- Endpoints or resources not implemented in this change set.
- Planned future work.
- Stub implementations that are clearly placeholders.
- Missing functionality unless the current code creates a defect.
- Whole-repo List API mechanical inventories (use `goals-review`).

## How to review

1. Establish the diff (see **Scope**).
2. Read changed code and enough surrounding context to judge behaviour and integration.
3. Trace call paths when these areas change: `Infrastructure/Http`, OAuth token exchange/refresh, JSON serialization (`JsonPropertyName` on serialised properties), pagination (`per_page` ≤ 100), and resource services exposed on `FreeAgentClient`.
4. When SDK services change, spot-check that both sample apps still reflect implemented operations only ([docs/contributing/sample-probe-pages.md](../../docs/contributing/sample-probe-pages.md)).
5. When unsure whether something is a real issue, investigate (read callers, run targeted tests) before reporting.
6. Report only issues you would expect a senior engineer to raise during a real pull request review.

Meaningful changes in this repo should pass the Release build and test gate in [docs/contributing-setup.md](../../docs/contributing-setup.md) (also summarised in [CONTRIBUTING.md](../../CONTRIBUTING.md) and [.github/PULL_REQUEST_TEMPLATE.md](../../.github/PULL_REQUEST_TEMPLATE.md)). Use that when judging whether a finding blocks approval.

## Output

Lead with findings. Keep summary and praise minimal.

For each issue, provide:

- **Severity** — Critical, High, Medium, or Low.
- **Location** — `path/to/file.cs:line` (or code citation when reporting in chat).
- **Why it matters** — tie to approve/reject risk.
- **Suggested improvement** — concrete, actionable fix direction (do not implement unless asked).

Present issues in a compact markdown table sorted by severity (highest first):

| Severity | Location | Why it matters | Suggested improvement |
| --- | --- | --- | --- |
| … | … | … | … |

Use UK English in prose.

## Closing

End with a brief explanation of overall merge readiness (approve, approve with nits, or reject) based only on the issues above.

If no significant issues are found, say so explicitly. Mention residual risks or test gaps only when they are material to approval.

## After the review

- Do not implement fixes, commit, or push unless the user explicitly asks.
- If the user asks to fix findings, apply minimal correct diffs, match existing conventions, then run the contributing build/test commands before claiming the review is clear.
