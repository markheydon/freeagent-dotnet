> This file defines coding and design conventions for the FreeAgent.NET SDK.

# Conventions

**Project:** FreeAgent.NET
**Last updated:** 29 April 2026

This document records decisions about how code is written in this project.
It exists so that both humans and AI produce consistent output.
When in doubt, follow what's here. To change a convention, update this file and
create an ADR if it's a significant architectural change.

---

## Project Structure

````
src/
└── FreeAgent.Client/
	├── FreeAgentClient.cs            # Main consumer entry point
	├── Authentication/               # OAuth token exchange/refresh client and models
	├── Http/                         # HTTP transport, rate limiting, API exceptions, pagination helpers
	├── Models/                       # Strongly typed FreeAgent resource/request/response models
	└── Services/                     # Resource-oriented service classes (Company, Contacts, etc.)

tests/
└── FreeAgent.Client.Tests/
	├── Authentication/
	├── Http/
	└── ...
````

---

## Patterns in Use
- **Client + Services pattern** - expose a central `FreeAgentClient` with discoverable resource services.
- **Strongly typed contracts** - use explicit request/response/resource models for API payloads.
- **Exception hierarchy** - throw SDK-specific exception types for API and transport failures.
- **Async-first APIs** - all network-bound operations are async and cancellation-aware.
- **Composition over framework layering** - avoid app-style architecture layers not needed by an SDK package.

---

## Naming Quick Reference

| Thing | Convention | Example |
|---|---|---|
| Main client | `[Product]Client` | `FreeAgentClient` |
| Service | `[Resource]Service` | `CompanyService` |
| Request model | `[Resource][Action]Request` | `ContactCreateRequest` |
| Response wrapper | `[Resource]Response` | `CompanyResponse` |
| Resource model | `[Resource]` | `Company` |
| Exception | `[Product][Context]Exception` | `FreeAgentApiException` |
| Test class | `[ClassName]Tests` | `FreeAgentOAuthClientTests` |
| Test method | `Method_State_Expected` | `GetAuthorizationUrl_WithState_IncludesStateParameter` |

---

## Things We Don't Do Here
- No app-style controller or database architecture in this SDK package
- No business-rule abstraction (VAT, accounting policy, reporting opinions)
- No `.Result` or `.Wait()` on async code
- No commented-out code committed to main
- No `TODO` without a linked GitHub Issue number

---

## Revision History
| Date | Change | Reason |
|---|---|---|
| 29 April 2026 | Initial draft | Project kickoff |
