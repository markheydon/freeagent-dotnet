# Copilot Instructions for FreeAgent.NET

> ⚠️ Review and customise these defaults for your project before relying on them.

## Language and Spelling

All documentation, comments, and user-facing text in this repository **must use UK English spelling and vocabulary**. Do not use US English. When in doubt, prefer the Oxford English Dictionary standard.

## Project Context

FreeAgent.NET is an open-source, modern .NET SDK for the FreeAgent REST API. It aims to provide a first-class developer experience comparable to Stripe.NET, with strongly typed models, clean service abstractions, automatic pagination, and predictable error handling. The package is published to NuGet for use by the author's own applications and the wider .NET community.

## Tech Stack

- .NET 10.0 (LTS).
- xUnit for testing.
- Microsoft.Extensions.Http (NuGet).
- No database (API client only).

## Architecture

- SDK-oriented architecture: main client entry point, resource services, typed models, and HTTP/auth support components.
- No UI, CLI, or end-user tooling.
- No business-rule abstraction.
- No handling of OAuth flows beyond programmatic token exchange/refresh.
- Targeting .NET 10 LTS.

## Coding Conventions

> ⚠️ Review and customise these defaults for your project before relying on them.

- Use async/await for all I/O.
- Use strongly typed models for all API requests/responses.
- Service abstractions for each API resource.
- Exception hierarchy for API and transport errors.
- No business logic in SDK.

## Naming

> ⚠️ Review and customise these defaults for your project before relying on them.

- Classes: PascalCase.
- Methods: PascalCase.
- Properties: PascalCase.
- Interfaces: I prefix.
- Test classes: [ClassName]Tests.

## Sample and Test App Sync

The Blazor sample (`samples/FreeAgent.Client.Sample`) must reflect the **current, implemented** state of the SDK — not planned or aspirational endpoints.

- Every API endpoint implemented in `src/FreeAgent.Client/Services/` must have a corresponding page or component in the sample app that exercises it.
- When a new service or endpoint is added to the SDK, a matching UI page in the sample app must be added in the same PR.
- When an endpoint is removed or renamed, the sample app must be updated in the same PR.
- Do not add sample UI for endpoints that do not yet exist in the SDK.
- The sample app is the living reference for "what this SDK can do today". Keep it honest.

## What Not to Do

- Do not put business logic in controllers.
- Do not generate migrations automatically - flag when a migration is needed.
- Do not add NuGet packages without flagging it first.
- Do not change the architecture pattern without an ADR.
- Do not add sample app pages for SDK endpoints that are not yet implemented.

## GitHub Issues

- When suggesting user-facing feature work, use `.github/ISSUE_TEMPLATE/feature_request.yml`.
- When suggesting maintenance/tooling/docs/governance work, use `.github/ISSUE_TEMPLATE/chore_request.yml`.
- When suggesting broad planning/admin work, use `.github/ISSUE_TEMPLATE/story_request.yml`.
- Include explicit scope, acceptance criteria, risks/trade-offs, and GOALS.md links where relevant.

## ADR Authoring Override

- Store all ADRs in the repository-root `adr/` directory.
- Do not place ADRs under `docs/` because `docs/` is reserved for public-facing documentation and GitHub Pages content.
- ADR filename convention: `adr-NNNN-[title-slug].md` (4-digit sequence, lowercase filename).
- When existing ADRs are in legacy locations (such as repository root or `docs/adr/`), move them into `adr/` as part of ADR housekeeping changes.
