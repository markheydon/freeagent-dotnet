# Agent Instructions

This file governs how AI agents (GitHub Copilot, etc.) should behave when operating autonomously in this repository.

---

## Allowed Actions
- Suggest and implement code changes within existing architectural patterns.
- Write and update tests.
- Update documentation files (README, CHANGELOG, inline comments).
- Raise GitHub Issues using the issue templates in `.github/ISSUE_TEMPLATE/`.
- Open draft PRs for human review.

## Not Allowed Without Explicit Instruction
- Adding or removing NuGet packages.
- Modifying database migration files.
- Changing CI/CD pipeline configuration.
- Altering authentication or authorisation logic.
- Changing `.env` / secrets configuration.

## Review Requirements
- All PRs opened by an agent require human review before merge.
- PRs touching Domain or Application layer require particular scrutiny.
- Any change that affects a goal in GOALS.md should be flagged in the PR description.

## Context Files
When working in this repo, refer to:
- `GOALS.md` - project intent and success criteria.
- `SCOPE.md` - what is and isn't in scope.
- `CONVENTIONS.md` - naming, patterns, and style.
- `.github/copilot-instructions.md` - coding standards.

## Issue Formatting
When raising GitHub Issues, use the templates in `.github/ISSUE_TEMPLATE/`:
- `feature_request.yml` for user-facing feature/enhancement work.
- `chore_request.yml` for maintenance/tooling/docs/governance tasks.
- `story_request.yml` for broader planning/admin stories.

For all issue types:
- Link issues to relevant goals from GOALS.md.
- Include explicit scope, acceptance criteria, and risks/trade-offs.
