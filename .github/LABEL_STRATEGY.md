# Label Strategy

> The **single source of truth** for all label definitions across `markheydon` repositories is:
> [`markheydon/github-workflows — plan/LABEL_STRATEGY.md`](https://github.com/markheydon/github-workflows/blob/main/plan/LABEL_STRATEGY.md)
>
> This file is a local reference summary only. When the cross-repo strategy changes,
> update this file to keep it in sync.

---

## Core Work Item Labels

Every issue should have exactly one of these labels.

| Label | Board | Description |
|-------|-------|-------------|
| `story` | ✅ | A user-facing feature, improvement, documentation change, or technical task. This is the **primary unit of work** on the project board. |
| `bug` | ✅ | Something isn't working as expected. |
| `epic` | ❌ | A large body of work made up of multiple stories. Epics group stories and are never tracked directly on the board. |

### Labels deliberately NOT used

The following labels have been **excluded** from this strategy. Do not create or use them:

| Label | Reason |
|-------|--------|
| `feature` | Superseded by `story`. |
| `chore` | Not a label in this strategy — maintenance and governance work is categorised as `story`. |
| `improvement` | Superseded by `story`. |
| `technical` | Superseded by `story`. |
| `dependency` | Replaced by `blocked`. |

---

## Modifier Labels

These can be applied alongside a core label. An issue may have multiple modifiers.

| Label | Description |
|-------|-------------|
| `priority-high` | High priority — address before other items. |
| `blocked` | Blocked by another issue or external dependency. |
| `not-started` | Work has not yet started (explicit backlog filtering). |
| `out-of-scope` | Intentionally deferred — may be revisited. |
| `feedback-required` | Waiting for feedback before work can proceed. |
| `waiting-for-details` | Further details required before work can start. |

---

## Issue Template ↔ Label Mapping

| Template | Label applied | Rationale |
|----------|--------------|-----------|
| `feature_request.yml` | `story` | Feature requests are stories — they drive board inclusion. |
| `chore_request.yml` | *(none — applied at triage)* | Chore work is labelled `story` at triage if it warrants board tracking. Not all chores need to be on the board. |
| `story_request.yml` | `story` | Broad planning/admin work. |
| Bug report | `bug` | Standard core label. |

---

## Decision Guide

1. Is this a large body of work containing multiple sub-tasks? → `epic`
2. Is this a new feature, improvement, docs change, or technical task? → `story`
3. Is something broken or not working as expected? → `bug`
4. Is it blocked? → add `blocked`
5. Is it urgent? → add `priority-high`
6. Is it on the backlog but not yet started? → add `not-started`
7. Has it been deferred? → add `out-of-scope`
8. Waiting on feedback? → add `feedback-required` or `waiting-for-details`
