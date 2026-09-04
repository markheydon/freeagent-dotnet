> This file defines coding and design conventions for the FreeAgent.NET SDK.

# Conventions


**Project:** FreeAgent.NET
**Last updated:** 4 September 2026

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
	├── PaginatedResponse.cs             # Public pagination result type
	├── Infrastructure/                  # Internal plumbing — not part of the public API surface
	│   ├── Authentication/              # OAuth token exchange/refresh client and models
	│   ├── Configuration/               # Environment enum and URL mapping
	│   ├── Http/                        # HTTP transport, rate limiting, API exceptions, pagination helpers
	│   └── Serialization/               # JSON converters and TFM compatibility
	├── Models/                          # Resource-grouped strongly typed models
	│   ├── Company/                     # Company resource models (Company, AnnualAccountingPeriod, SalesTaxRate, TaxTimelineItem, response wrappers)
	│   └── Contacts/                    # Contacts resource models (Contact, response wrappers)
	└── Services/                        # Resource-oriented service classes
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
- `FreeAgent.Client` — top-level consumer namespace (`FreeAgentClient`, `PaginatedResponse<T>`)
- `FreeAgent.Client.Infrastructure.Authentication` — OAuth types
- `FreeAgent.Client.Infrastructure.Configuration` — FreeAgentEnvironment, FreeAgentEnvironmentEndpoints
- `FreeAgent.Client.Infrastructure.Http` — HTTP client, exceptions, pagination helpers
- `FreeAgent.Client.Models.Company` — Company resource models
- `FreeAgent.Client.Models.Contacts` — Contacts resource models
- `FreeAgent.Client.Services.Company.CompanyService` — Company resource service
- `FreeAgent.Client.Services.Contacts.ContactService` — Contacts resource service

**Key naming rules (unchanged):**
- Service: [Resource]Service (CompanyService, ContactService)
- Response wrapper: [Resource]Response (CompanyResponse, ContactsResponse)
- Resource model: [Resource] (Company, Contact)
- Support model: descriptive noun (TaxTimelineItem, AnnualAccountingPeriod, SalesTaxRate)
- Exception: [Product][Context]Exception (FreeAgentApiException)
- Test class: [ClassName]Tests
- Test method: Method_State_Expected


**New conventions:**
- Models must be grouped by resource under Models/[Resource]/ with namespace FreeAgent.Client.Models.[Resource]
- Cross-resource shared primitives go in Models/Shared/ if needed in future
- All infrastructure plumbing goes under Infrastructure/ and uses internal/public as appropriate, never leaked into the top-level package namespace except for types that are part of the public SDK contract (for example `PaginatedResponse<T>`)
- Resource services take `IFreeAgentRequestClient` internally; do not introduce a shared public `ServiceBase` unless an ADR says otherwise
- Each resource service and any service-local helpers, options, or types must be placed in a resource-named subfolder under Services/ (e.g., Services/Invoices/ for invoice-related services and helpers). This keeps resource logic and extensions together and discoverable.
---

## Patterns in Use
- **Client + Services pattern** - expose a central `FreeAgentClient` with discoverable resource services.
- **Strongly typed contracts** - use explicit request/response/resource models for API payloads.
- **Exception hierarchy** - throw SDK-specific exception types for API and transport failures.
- **Async-first APIs** - all network-bound operations are async and cancellation-aware.
- **Composition over framework layering** - avoid app-style architecture layers not needed by an SDK package.
- **Documented operation variants** - when the API docs describe multiple create or update shapes for the same HTTP route, each variant gets its own public request type and service method. Public requests expose only attributes allowed for that variant; fixed wire values (for example `category_group`) are set inside the SDK. See [adr-0010-documented-operations-to-sdk-methods.md](adr/adr-0010-documented-operations-to-sdk-methods.md).

---

## Naming Quick Reference

| Thing | Convention | Example |
|---|---|---|
| Main client | `[Product]Client` | `FreeAgentClient` |
| Service | `[Resource]Service` | `CompanyService` |
| Request model (single shape) | `[Resource][Action]Request` | `ContactCreateRequest` |
| Request model (operation variant) | `[Action][Variant][Resource]Request` | `CreateIncomeCategoryRequest` |
| Service method (operation variant) | `[Action][Variant][Resource]Async` | `CreateIncomeCategoryAsync` |
| Discriminator factory | `For[Discriminator]` on request type | `CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(...)` |
| Discriminator enum | `[Discriminator][Variant][Field]` | `UkLimitedCompanyCostOfSalesTaxReportingName` |
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
| 4 September 2026 | Add operation-variant request/method naming and discriminator factories | ADR-0010 |
| 2 September 2026 | Align folder notes with current SDK (no ServiceBase; public PaginatedResponse) | Docs were describing a layout the code no longer uses |
| 1 May 2026 | Mirror tests folder structure to source layout      | Improve test discoverability as Infrastructure/Services grow |
| 1 May 2026 | Add resource-grouped Services structure and guidance | Service structure reorg, clarify placement of service-local helpers |
| 1 May 2026 | Update for new folder/namespace layout              | Project structure refactor     |
| 29 April 2026 | Initial draft                                    | Project kickoff               |
