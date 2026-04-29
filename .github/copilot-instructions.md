# Copilot Instructions for FreeAgent.NET

> ⚠️ Review and customise these defaults for your project before relying on them.

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

## What Not to Do

- Do not put business logic in controllers.
- Do not generate migrations automatically - flag when a migration is needed.
- Do not add NuGet packages without flagging it first.
- Do not change the architecture pattern without an ADR.

## GitHub Issues

- When suggesting work to be done, format it as a GitHub Issue using the story issue template.
- Link issues to GOALS.md goals where relevant.
