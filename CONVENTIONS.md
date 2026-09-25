> This file defines coding and design conventions for the FreeAgent.NET SDK.

# Conventions


**Project:** FreeAgent.NET
**Last updated:** 24 September 2026

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
	├── FreeAgentResourceUrls.cs         # Environment-correct resource URI builders
	├── PaginatedResponse.cs             # Public pagination result type
	├── *GetOptions.cs                   # Single-GET hydration options (Projects, Invoices, Estimates, RecurringInvoices, Tasks, Timeslips, Notes)
	├── Infrastructure/                  # Internal plumbing - not part of the public API surface
	│   ├── Authentication/              # OAuth token exchange/refresh client and models
	│   ├── Configuration/               # Environment enum and URL mapping
	│   ├── Http/                        # HTTP transport, rate limiting, API exceptions, pagination helpers
	│   └── Serialization/               # JSON converters and TFM compatibility
	├── Models/                          # Resource-grouped strongly typed models
	│   ├── BankAccounts/                # Bank account stub models for invoice links
	│   ├── Categories/                  # Category models, operation-variant requests, nominal-code validation
	│   ├── Company/                     # Company, AnnualAccountingPeriod, SalesTaxRate, TaxTimelineItem, response wrappers
	│   ├── Contacts/                    # Contact models and response wrappers
	│   ├── CreditNotes/                 # Credit note stub models for invoice transitions
	│   ├── EmailAddresses/              # Email addresses response wrapper
	│   ├── Invoices/                    # Invoice models, line items, transitions, response wrappers
	│   ├── Notes/                       # Note models and response wrappers
	│   ├── Projects/                    # Project models, views, sort options, response wrappers
	│   ├── RecurringInvoices/           # Recurring invoice models and response wrappers
	│   ├── Shared/                      # Cross-resource primitives (ContactReference, ProjectReference, CurrencyCode)
	│   ├── Tasks/                       # Task models and response wrappers
	│   ├── Timeslips/                   # Timeslip models and response wrappers
	│   └── Users/                       # User models and response wrappers
	└── Services/                        # Resource-oriented service classes
		├── Categories/                  # CategoryService and category write/response mappers
		├── Company/
		│   └── CompanyService.cs
		├── Contacts/
		│   └── ContactService.cs
		├── EmailAddresses/
		│   └── EmailAddressesService.cs
		├── Invoices/
		│   └── InvoiceService.cs
		├── RecurringInvoices/
		│   └── RecurringInvoiceService.cs
		├── Notes/
		│   └── NoteService.cs
		├── Projects/
		│   └── ProjectService.cs
		├── Tasks/
		│   └── TaskService.cs
		├── Timeslips/
		│   └── TimeslipService.cs
		└── Users/
		    └── UserService.cs

tests/
└── FreeAgent.Client.Tests/
	├── FreeAgentClientTests.cs
	├── Infrastructure/
	│   ├── Authentication/
	│   ├── Configuration/
	│   ├── Http/
	│   └── Serialization/
	├── Models/
	│   ├── Categories/
	│   ├── Contacts/
	│   ├── Invoices/
	│   ├── RecurringInvoices/
	│   ├── Notes/
	│   ├── Projects/
	│   ├── Shared/
	│   ├── Tasks/
	│   ├── Timeslips/
	│   └── Users/
	├── Sample/                          # Sample-app helper tests (Turpinverse, wire diagnostics)
	├── Samples/                         # Console sample helper tests
	├── Services/
	│   ├── Categories/
	│   ├── Company/
	│   ├── Contacts/
	│   ├── EmailAddresses/
	│   ├── Invoices/
	│   ├── RecurringInvoices/
	│   ├── Notes/
	│   ├── Projects/
	│   ├── Tasks/
	│   ├── Timeslips/
	│   └── Users/
	└── TestSupport/
```

**Namespace layout:**
- `FreeAgent.Client` - top-level consumer namespace (`FreeAgentClient`, `PaginatedResponse<T>`, `*GetOptions`)
- `FreeAgent.Client.Infrastructure.Authentication` - OAuth types
- `FreeAgent.Client.Infrastructure.Configuration` - FreeAgentEnvironment, FreeAgentEnvironmentEndpoints
- `FreeAgent.Client.Infrastructure.Http` - HTTP client, exceptions, pagination helpers
- `FreeAgent.Client.Models.[Resource]` - resource models (BankAccounts, Categories, Company, Contacts, CreditNotes, EmailAddresses, Invoices, Notes, Projects, RecurringInvoices, Shared, Tasks, Timeslips, Users)
- `FreeAgent.Client.Services.[Resource]` - resource services (Categories, Company, Contacts, EmailAddresses, Invoices, Notes, Projects, Tasks, Timeslips, Users)

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
- **Documented operation-contract validation** - when the official FreeAgent docs state a local, account-independent constraint, validate it client-side (for example `per_page` ≤ 100). Categories nominal-code ranges are one documented case, not a template to invent similar checks on resources that do not document them. Account-state checks (for example uniqueness) and accounting policy remain out of scope per [SCOPE.md](SCOPE.md).
- **Linked resource identity** - documented URI links use flat read properties (`Contact?`, `ContactId`, denormalised display names), `*Reference` types on write payloads and list filters, and optional `*GetOptions` hydration on single-resource GET. `ExpandableField<T>` is internal wire plumbing only. Build URIs with `client.Urls`. See [adr-0011-linked-resource-identity-and-expandable-references.md](adr/adr-0011-linked-resource-identity-and-expandable-references.md) and [docs/explanation/linked-resources.md](docs/explanation/linked-resources.md).

---

## Naming Quick Reference

| Thing | Convention | Example |
|---|---|---|
| Main client | `[Product]Client` | `FreeAgentClient` |
| Service | `[Resource]Service` | `CompanyService` |
| Request model (single shape) | `[Resource][Action]Request` | `ContactCreateRequest` |
| Request model (operation variant) | `[Action][Variant][Resource]Request` | `CreateIncomeCategoryRequest` |
| Service method (operation variant) | `[Action][Variant][Resource]Async` | `CreateIncomeCategoryAsync` |
| Collection read (single page or full response) | `ListAsync` on the resource service | `ContactService.ListAsync` |
| Collection read (auto-pagination) | `ListAutoPagingAsync` on the resource service | `ContactService.ListAutoPagingAsync` |
| Collection read (secondary endpoint on same service) | `List[Descriptor]Async` | `CompanyService.ListTaxTimelineAsync` |
| Single resource read | `Get[Resource]Async` | `GetContactAsync` |
| Discriminator factory | `For[Discriminator]` on request type | `CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(...)` |
| Discriminator enum | `[Discriminator][Variant][Field]` | `UkLimitedCompanyCostOfSalesTaxReportingName` |
| Response wrapper | `[Resource]Response` | `CompanyResponse` |
| Resource model | `[Resource]` | `Company` |
| Resource identity interface | `IFreeAgentResource` | `Contact`, `Project` |
| Linked resource (read) | `[Resource]?`, `long? [Resource]Id`, denormalised display field | `Project.Contact`, `Project.ContactId`, `Project.ContactName` |
| Linked resource (write/filter) | `[Resource]Reference` | `ContactReference`, `ProjectReference` |
| Single GET hydration | `[Resource]GetOptions` with `Include*` flags | `ProjectGetOptions.IncludeBillingContact` |
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
| 24 September 2026 | Refresh project-structure tree for Invoices, Tasks, Timeslips, Notes, and stub link models | Goals review documentation drift |
| 21 September 2026 | Refresh project-structure tree for all implemented resources and tests layout | Goals review sample-sync and docs drift |
| 21 September 2026 | Adopt Stripe.NET-aligned `ListAsync` / `ListAutoPagingAsync` naming for collection reads | ADR-0012 |
| 21 September 2026 | Add linked resource identity conventions (`ExpandableField<T>`, `*Reference`, `client.Urls`) | ADR-0011 |
| 4 September 2026 | Limit contract validation to official local docs constraints | G2: fail-fast without invented checks |
| 4 September 2026 | Add operation-variant request/method naming and discriminator factories | ADR-0010 |
| 2 September 2026 | Align folder notes with current SDK (no ServiceBase; public PaginatedResponse) | Docs were describing a layout the code no longer uses |
| 1 May 2026 | Mirror tests folder structure to source layout      | Improve test discoverability as Infrastructure/Services grow |
| 1 May 2026 | Add resource-grouped Services structure and guidance | Service structure reorg, clarify placement of service-local helpers |
| 1 May 2026 | Update for new folder/namespace layout              | Project structure refactor     |
| 29 April 2026 | Initial draft                                    | Project kickoff               |
