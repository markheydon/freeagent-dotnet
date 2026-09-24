# Agent Instructions

This file defines repository-specific operating rules for AI coding and writing agents in this project.

## Purpose

- Keep agent behaviour consistent with project goals, scope, and architecture.
- Reduce accidental drift in documentation quality, naming, and decision records.
- Ensure human review remains the final gate for all meaningful changes.

## Core Context

Agents should ground decisions in these files before making non-trivial changes:

- `GOALS.md` for project intent and success criteria.
- `SCOPE.md` for in-scope and out-of-scope boundaries.
- `CONVENTIONS.md` for coding and naming conventions.

## Language and Spelling

All documentation, comments, and user-facing text in this repository **must use UK English spelling and vocabulary**. Do not use US English.

## Tech Stack

- .NET 8.0 (LTS) and .NET 10.0 (LTS), with .NET 10 as the primary focus.
- xUnit for testing.
- No database (API client only).

## Architecture

- SDK-oriented architecture: main client entry point, resource services, typed models, and HTTP/auth support components.
- No UI, CLI, or end-user tooling in the SDK package.
- No business-rule abstraction.
- Protocol-level OAuth helpers only (authorisation URL, code exchange, token refresh). No UI/browser orchestration or app-level flow management.

## Sample Sync

Both sample apps must reflect the **current, implemented** state of the SDK - not planned or aspirational endpoints.

| Sample | Path | Role |
|--------|------|------|
| Blazor workbench | `samples/FreeAgent.Client.BlazorSample` | Wire-to-model probe pages for every SDK operation |
| Console reference | `samples/FreeAgent.Client.ConsoleSample` | Real-world integration patterns (DI, interactive menu, `--run-all` smoke test) |

Framework targeting note:

- The SDK package targets .NET 8.0 and .NET 10.0.
- Both sample apps are intentionally .NET 10.0-only.
- "Sample sync" means endpoint capability parity, not target framework parity.

Shared rules:

- Every API endpoint implemented in `src/FreeAgent.Client/Services/` must be exercised in **both** sample apps, using the pattern appropriate to each app.
- When a new service or endpoint is added to the SDK, update both samples in the same PR.
- When an endpoint is removed or renamed, update both samples in the same PR.
- Do not add sample coverage for endpoints that do not yet exist in the SDK.
- The samples are the living reference for "what this SDK can do today". Keep them honest.

### Blazor sample (probe pages)

- Add or update page(s) under `samples/FreeAgent.Client.BlazorSample/Components/Pages/`.
- Update navigation in `samples/FreeAgent.Client.BlazorSample/Components/Layout/MainLayout.razor`.
- Follow [`docs/contributing/sample-probe-pages.md`](docs/contributing/sample-probe-pages.md).
- Use **Company** (`/company`), **Contacts** (`/contacts`, `/contacts/detail`), and **Categories** (`/categories`, `/categories/detail` for multi-variant writes) as reference implementations.
- Reuse `EndpointProbeHeader`, `ModelWireDiagnostics`, `ModelProbeResults`, and `ApiErrorDiagnostics` - do not build one-off mapping UIs.
- List pages must support per-row wire-to-model inspection; CRUD pages must support deep links, post-mutation wire fetch, and a visible loading progress bar.

### Console sample (example providers)

- Add or update a matching `*Samples.cs` provider class under `samples/FreeAgent.Client.ConsoleSample/Samples/` for each SDK resource area (discovered via `ConsoleSamplesAttribute`).
- Register read-only examples that exercise the main SDK calls for that resource (list, filtered list, and get detail as appropriate).
- Update the example catalogue in [`samples/FreeAgent.Client.ConsoleSample/README.md`](samples/FreeAgent.Client.ConsoleSample/README.md).
- Use existing providers such as `TaskSamples.cs` and `ProjectSamples.cs` as reference implementations.

## Skills

Project skills live in `.agents/skills/`. Read the matching `SKILL.md` when the task matches its description.

| Skill | Use when |
|---|---|
| `implement-endpoint` | Adding or retrofitting a FreeAgent API resource end-to-end. Map **every documented operation heading** on the API docs page to a typed SDK method - not just each HTTP route. When multiple create/update shapes share a route, use a separate request type and method per variant. |
| `create-architectural-decision-record` | Creating or major-updating an ADR |
| `documentation-writer` | Diátaxis-aligned documentation structure |
| `project-documentation` | Project-aware docs placement and terminology |
| `mudblazor` | Sample app Blazor UI with MudBlazor |
| `pr-address-review` | Addressing open PR review comment threads |
| `goals-review` | Auditing the repo or a PR diff against GOALS.md, SCOPE.md, CONVENTIONS.md, and sample-sync rules |

## Task Routing

Route work by task type, not by custom agent runtime:

- **SDK work** (`src/`, `tests/`): follow `CONVENTIONS.md`. Use `implement-endpoint` when adding or retrofitting API resources.
- **Documentation** (`**/*.md` except `adr/`): use `documentation-writer` and `project-documentation`.
- **ADRs** (`adr/*.md`): use `create-architectural-decision-record` only.

## Allowed Actions

- Suggest and implement code changes within existing SDK architecture patterns.
- Add or update tests for changed behaviour.
- Update repository documentation and implementation plans.
- Raise GitHub Issues using repository templates.
- Open draft pull requests for human review.

## Not Allowed Without Explicit Instruction

- Add or remove NuGet packages.
- Modify CI/CD pipeline behaviour.
- Change authentication or authorization logic.
- Change secrets or environment configuration.
- Introduce architecture pattern changes without an ADR.

## ADR Routing

- Store ADRs only in repository-root `adr/` using `adr-NNNN-[title-slug].md`.
- Do not place ADRs under `docs/` (`docs/` is reserved for public-facing documentation).
- Use the `create-architectural-decision-record` skill for ADR creation and major updates.

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
