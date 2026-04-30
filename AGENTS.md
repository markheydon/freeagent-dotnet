# Agent Instructions

This file defines repository-specific operating rules for AI coding and writing agents in this project.

## Purpose
- Keep agent behavior consistent with project goals, scope, and architecture.
- Reduce accidental drift in documentation quality, naming, and decision records.
- Ensure human review remains the final gate for all meaningful changes.

## Core Context
Agents should ground decisions in these files before making non-trivial changes:
- `GOALS.md` for project intent and success criteria.
- `SCOPE.md` for in-scope and out-of-scope boundaries.
- `CONVENTIONS.md` for coding and naming conventions.
- `.github/copilot-instructions.md` for repository-wide coding and documentation guidance.

## Allowed Actions
- Suggest and implement code changes within existing SDK architecture patterns.
- Add or update tests for changed behavior.
- Update repository documentation and implementation plans.
- Raise GitHub Issues using repository templates.
- Open draft pull requests for human review.

## Not Allowed Without Explicit Instruction
- Add or remove NuGet packages.
- Modify CI/CD pipeline behavior.
- Change authentication or authorization logic.
- Change secrets or environment configuration.
- Introduce architecture pattern changes without an ADR.

## Documentation Routing
- For general Markdown documentation (`**/*.md`), use the `tech-writer-mh` path by default.
- Follow `.github/instructions/markdown.instructions.md` for documentation consistency expectations.

## ADR Routing
- For ADR files under `adr/*.md`, use the `create-architectural-decision-record` skill as the primary path.
- Follow `.github/instructions/adr.instructions.md` for ADR-specific authoring rules.
- Store ADRs only in repository-root `adr/` using `adr-NNNN-[title-slug].md`.

## Issue Formatting
When creating or suggesting GitHub Issues, use:
- `.github/ISSUE_TEMPLATE/feature_request.yml` for user-facing features and enhancements.
- `.github/ISSUE_TEMPLATE/chore_request.yml` for maintenance, tooling, docs, and governance.
- `.github/ISSUE_TEMPLATE/story_request.yml` for planning and admin stories.

For all issue types:
- Link to relevant goals in `GOALS.md`.
- Include explicit scope, acceptance criteria, and risks/trade-offs.

## Review Requirements
- All agent-authored pull requests require human review before merge.
- Flag any change that impacts `GOALS.md` outcomes in the PR description.
- Highlight breaking API risk, migration impact, and test coverage impact in PR summaries.
