> This file defines coding and design conventions for the FreeAgent.NET SDK.

# Conventions


**Project:** FreeAgent.NET
**Last updated:** 1 May 2026

This document records decisions about how code is written in this project.
It exists so that both humans and AI produce consistent output.
When in doubt, follow what's here. To change a convention, update this file and
create an ADR if it's a significant architectural change.

---

## Project Structure

```
src/
└── FreeAgent.Client/
	├── FreeAgentClient.cs               # Main consumer entry point
	├── Infrastructure/                  # Internal plumbing — not part of the public API surface
	│   ├── Authentication/              # OAuth token exchange/refresh client and models
	│   ├── Configuration/               # Environment enum and URL mapping
	│   └── Http/                        # HTTP transport, rate limiting, API exceptions, pagination helpers
	├── Models/                          # Resource-grouped strongly typed models
	│   ├── Company/                     # Company resource models (Company, AnnualAccountingPeriod, SalesTaxRate, TaxTimelineItem, response wrappers)
	│   └── Contacts/                    # Contacts resource models (ContactSummary, response wrappers)
	└── Services/                        # Resource-oriented service classes
		├── ServiceBase.cs               # Shared abstract base for all services
		├── Company/
		│   └── CompanyService.cs
		└── Contacts/
		    └── ContactService.cs

tests/
└── FreeAgent.Client.Tests/
	├── FreeAgentClientTests.cs
	├── Infrastructure/
	│   ├── Authentication/
	│   ├── Configuration/
	│   └── Http/
	├── Services/
	│   ├── Company/
	│   └── Contacts/
	└── TestSupport/
```

**Namespace layout:**
- `FreeAgent.Client` — top-level consumer namespace (FreeAgentClient)
- `FreeAgent.Client.Infrastructure.Authentication` — OAuth types
- `FreeAgent.Client.Infrastructure.Configuration` — FreeAgentEnvironment, FreeAgentEnvironmentEndpoints
- `FreeAgent.Client.Infrastructure.Http` — HTTP client, exceptions, pagination
- `FreeAgent.Client.Models.Company` — Company resource models
- `FreeAgent.Client.Models.Contacts` — Contacts resource models
- `FreeAgent.Client.Services.ServiceBase` — shared service base class
- `FreeAgent.Client.Services.Company.CompanyService` — Company resource service
- `FreeAgent.Client.Services.Contacts.ContactService` — Contacts resource service

**Key naming rules (unchanged):**
- Service: [Resource]Service (CompanyService, ContactService)
- Response wrapper: [Resource]Response (CompanyResponse, ContactsResponse)
- Resource model: [Resource] (Company, ContactSummary)
- Support model: descriptive noun (TaxTimelineItem, AnnualAccountingPeriod, SalesTaxRate)
- Exception: [Product][Context]Exception (FreeAgentApiException)
- Test class: [ClassName]Tests
- Test method: Method_State_Expected


**New conventions:**
- Models must be grouped by resource under Models/[Resource]/ with namespace FreeAgent.Client.Models.[Resource]
- Cross-resource shared primitives go in Models/Shared/ if needed in future
- All infrastructure plumbing goes under Infrastructure/ and uses internal/public as appropriate, never leaked into the top-level package namespace
- ServiceBase is the public abstract base class for all services; new services must inherit it
- Each resource service and any service-local helpers, options, or types must be placed in a resource-named subfolder under Services/ (e.g., Services/Invoices/ for invoice-related services and helpers). This keeps resource logic and extensions together and discoverable.
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
| Date       | Change                                              | Reason                        |
|------------|-----------------------------------------------------|-------------------------------|
| 1 May 2026 | Mirror tests folder structure to source layout      | Improve test discoverability as Infrastructure/Services grow |
| 1 May 2026 | Add resource-grouped Services structure and guidance | Service structure reorg, clarify placement of service-local helpers |
| 1 May 2026 | Update for new folder/namespace layout              | Project structure refactor     |
| 29 April 2026 | Initial draft                                    | Project kickoff               |
