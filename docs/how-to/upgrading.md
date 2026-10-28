---
title: Upgrading
parent: How-to
nav_order: 4
---

# Upgrading

FreeAgent.NET follows [Semantic Versioning](https://semver.org/). Until stable `1.0.0`, published packages are **prerelease** (currently **beta**, e.g. `0.1.0-beta.3`).

## Prerelease expectations

- During **beta**, breaking changes are avoided where possible and documented in release notes.
- **Alpha** tags (`0.x.y-alpha.n`) are historical; treat upgrades from alpha to beta as a major integration step.
- Pin an exact version in your project file:

```xml
<PackageReference Include="FreeAgent.Client" Version="0.1.0-beta.3" />
```

## Upgrade checklist

1. Read the [GitHub Release notes](https://github.com/markheydon/freeagent-dotnet/releases) for breaking changes.
2. Update the package reference and restore.
3. Run your test suite - model mapping changes (for example `string` to enum, `DateTime` to `DateOnly`) are the most common breaks when moving between prerelease versions.
4. Check [API coverage](../reference/api-coverage.md) for newly implemented endpoints.

## Recent breaking changes (alpha through beta.1)

### Property and capital asset linked resources (pre-V1 G2)

| Before | After |
|--------|--------|
| `Invoice.PropertyUri`, `Bill.PropertyUri`, `Expense.PropertyUri`, `CreditNote.PropertyUri`, `RecurringInvoice.PropertyUri` | `PropertyId` (and optional read `Property`) |
| `BankTransactionExplanation.PropertyUri` | `PropertyId` |
| `UpdateBankTransactionExplanationRequest.PropertyUri` | `PropertyId` |
| `Expense.CapitalAssetUri`, `BillItem.CapitalAssetUri`, `BankTransactionExplanation.CapitalAssetUri` / `DisposedAssetUri` | `CapitalAssetId` / `DisposedAssetId` (read-only, parsed from the wire) |
| `CreateLandlordPropertyBankTransactionExplanationRequest.Create(..., string propertyUri, ...)` | `Create(..., long propertyId, ...)` only |
| `CreateLandlordPropertyBankTransactionExplanationRequest.Create(..., long propertyId, FreeAgentEnvironment, ...)` | Removed — the service supplies the environment |

`AccountingTransaction` adds `SourceItemKind` alongside `SourceItemId` for report rows.

Create and update calls on invoices, estimates, credit notes, bills, expenses, and journal sets now throw `ArgumentException` before HTTP when documented required fields are missing (contact, dates, line descriptions, journal balance on create, and related rules). Company-type constraints still surface as API validation errors.

### Model namespace flattening

All public model types now live in `FreeAgent.Client` alongside `FreeAgentClient` and `*GetOptions`. Remove every `using FreeAgent.Client.Models.*;` directive.

- `DefaultAdditionalTextResponse` / `DefaultAdditionalTextRequest` for invoices and estimates are a single shared pair (identical wire shape).
- The unused duplicate `CreditNoteResponse` type under the invoice models folder was removed; use `CreditNoteResponse` from the credit notes API surface.

Historical note: an earlier alpha release moved `BankTransactionExplanation` from a bank-transactions model namespace into its own resource folder; that type is now simply `FreeAgent.Client.BankTransactionExplanation`.

### Typed wire enums and unknown-tolerant deserialisation

- Remaining string wire fields are now typed: `SecondSalesTaxStatus`, `CisDeductionRate?` for `cis_rate`, `BankTransactionExplanationType?` for explanation `type`, `BankFeedState?` for feed `state`, and `BillAttachmentContentType?` on read-model attachments.
- Wire enums deserialise unrecognised API strings to an `Unknown` member instead of throwing. Check for `Unknown` after GET calls; do not send `Unknown` on create or update (serialisation throws).
- `SecondSalesTaxStatus` exposes `Kind` and `WireValue` so account-specific second sales tax codes are not lost when `Kind` is `Unknown`.

### List `view` and `sort` query parameters

- `*Views` string constants and raw `sort` strings on list methods are replaced with wire enums (for example `InvoiceView`, `ContactView`) and `ApiSort<TField>` (for example `ApiSort<InvoiceSortField>`).
- `CreditNoteView` is a struct supporting `CreditNoteView.LastMonths(n)` for `last_{n}_months` views.
- `BalanceSheet.Currency` is `CurrencyCode?`; `ExpenseAttachment.ContentType` is `BillAttachmentContentType?` on read.
- Replace `InvoiceViews.Open` with `InvoiceView.Open`, and `sort: "-updated_at"` with `new ApiSort<InvoiceSortField>(InvoiceSortField.UpdatedAt, descending: true)`.

### List API naming (Stripe.NET parity)

Collection reads now use Stripe.NET-aligned names on resource services:

| Before | After |
|---|---|
| `GetContactsPageAsync` | `client.Contacts.ListAsync` |
| `GetAllContactsAsync` | `client.Contacts.ListAutoPagingAsync` |
| `GetProjectsPageAsync` | `client.Projects.ListAsync` |
| `GetAllProjectsAsync` | `client.Projects.ListAutoPagingAsync` |
| `GetUsersAsync` | `client.Users.ListAsync` |
| `GetCategoriesAsync` | `client.Categories.ListAsync` |
| `GetEmailAddressesAsync` | `client.EmailAddresses.ListAsync` |
| `GetBusinessCategoriesAsync` | `client.Company.ListBusinessCategoriesAsync` |
| `GetTaxTimelineAsync` | `client.Company.ListTaxTimelineAsync` |

Single-resource reads (`GetContactAsync`, `GetProjectAsync`, etc.) are unchanged.

### Linked resource fields on Projects

`Project.Contact` is now `Contact?` (read) for nested or hydrated billing contact data. Set `ContactId` when creating or updating a project.

Use `GetProjectAsync(id, new ProjectGetOptions { IncludeContact = true })` when you need full contact fields such as `OrganisationName`. Use `ContactName` when a display label from a single HTTP call is enough.

On update, the SDK round-trips `ContactId` from the read model. Linked resource writes across the SDK use `*Id` on models (and `CategoryNominalCode` for categories), not `*Reference` properties or `client.Urls` at the call site.

Project list methods accept `ContactReference? contact` and `long? contactId` instead of `string? contact`. Supply only one contact filter parameter - specifying both throws `ArgumentException`.

Use `TryGetResourceId()` or `GetResourceId()` when a parsed identifier is required. `ResourceId` returns `0` when parsing fails.

Top-level resources implement `IFreeAgentResource` with a computed `ResourceId` property. Use `project.ResourceId` for display, or the extension methods when calling services.

`ExpandableField<T>` is no longer part of the public API.

### Project task naming consistency

Task-related public API members now use the `ProjectTask` prefix to align with the `ProjectTask` model and avoid confusion with `System.Threading.Tasks.Task`:

| Before | After |
|---|---|
| `client.Tasks` | `client.ProjectTasks` |
| `TaskService` | `ProjectTaskService` |
| `GetTaskAsync` / `CreateTaskAsync` / `UpdateTaskAsync` / `DeleteTaskAsync` | `GetProjectTaskAsync` / `CreateProjectTaskAsync` / `UpdateProjectTaskAsync` / `DeleteProjectTaskAsync` |
| `TaskGetOptions` | `ProjectTaskGetOptions` |
| `TaskReference` | `ProjectTaskReference` |
| `client.Urls.Task(id)` | `client.Urls.ProjectTask(id)` |
| `TaskStatus`, `TaskViews`, `TaskSortOptions`, `TaskBillingPeriod` | `ProjectTaskStatus`, `ProjectTaskViews`, `ProjectTaskSortOptions`, `ProjectTaskBillingPeriod` |
| `Timeslip.Task` / `Timeslip.TaskId` | `Timeslip.ProjectTask` / `Timeslip.ProjectTaskId` |
| `TimeslipGetOptions.IncludeTask` | `TimeslipGetOptions.IncludeProjectTask` |

Wire JSON property names (`task`, `tasks`) are unchanged.

### Line item nested `StockItem` read property

On read models, the hydrated stock item on invoice, credit note, bill, and price list line items is now `StockItem?` named `StockItem` (replacing `StockItemResource`). `StockItemId` is unchanged for writes and for parsing link URIs from GET responses.

### Linked-resource hydration on GET

Several resources gained optional `*GetOptions` flags for second-hop hydration (for example `BillGetOptions.IncludeRebilledOnInvoice`, `TimeslipGetOptions.IncludeBilledOnInvoice`, `BankTransactionExplanationGetOptions`). List endpoints are unchanged; hydration applies only to single-resource `Get*Async` overloads that accept options.

### Contacts list returns full `Contact` models

Contact list methods now return `Contact` instead of the removed `ContactSummary` type. Update any code that depended on the slimmer list shape.

When creating or updating contacts, read-only API fields (`url`, balances, mandate state, timestamps) are ignored during serialisation. You can still round-trip a `Contact` retrieved from the API without sending those fields back.

### Sales tax and EC VAT MOSS (#102)

| Before | After |
|---|---|
| `place_of_supply` as `string?` on invoices, estimates, credit notes, recurring invoices, bank explanations | `EcMossPlaceOfSupply?` |
| Expense / bill item `SalesTaxRate` as `decimal?` | `PurchaseSalesTaxRate?` (`PurchaseSalesTaxRate.Auto` or `FromPercentage`) |
| (none) | `client.EcMoss.ListSalesTaxRatesAsync` |

MOSS creates/updates throw `ArgumentException` when `ec_status` is `EC VAT MOSS` but required fields are missing (see [EC VAT MOSS sales tax rates](../reference/ec-moss-sales-tax-rates.md)).

## Additive API changes

- **Attachments:** `client.Attachments` (`GetAttachmentAsync`, `DeleteAttachmentAsync`) — see [Attachments reference](../reference/attachments.md).
- **EC VAT MOSS:** `client.EcMoss.ListSalesTaxRatesAsync` — see [EC VAT MOSS sales tax rates](../reference/ec-moss-sales-tax-rates.md).

### Goals review (list and read naming)

Collection reads and generic report `GetAsync` names were aligned with [ADR-0012](../../adr/adr-0012-list-api-naming.md):

| Before | After |
|---|---|
| `client.EcMoss.GetSalesTaxRatesAsync` | `client.EcMoss.ListSalesTaxRatesAsync` |
| `client.TrialBalance.GetSummaryAsync` | `client.TrialBalance.ListSummaryAsync` |
| `client.TrialBalance.GetOpeningBalancesAsync` | `client.TrialBalance.ListOpeningBalancesAsync` |
| `client.BalanceSheet.GetAsync` | `client.BalanceSheet.GetBalanceSheetAsync` |
| `client.Cashflow.GetAsync` | `client.Cashflow.GetCashflowAsync` |
| `client.VatReturns.GetAsync` | `client.VatReturns.GetVatReturnAsync` |
| `client.CorporationTaxReturns.GetAsync` | `client.CorporationTaxReturns.GetCorporationTaxReturnAsync` |
| `client.FinalAccountsReports.GetAsync` | `client.FinalAccountsReports.GetFinalAccountsReportAsync` |
| `client.IncomeTaxReturns.GetAsync` | `client.IncomeTaxReturns.GetIncomeTaxReturnAsync` |
| `client.SelfAssessmentReturns.GetAsync` | `client.SelfAssessmentReturns.GetSelfAssessmentReturnAsync` |

`ProfitAndLossService.GetSummaryAsync`, `BalanceSheetService.GetOpeningBalancesAsync`, and `JournalSetsService.GetOpeningBalancesAsync` are unchanged (single-resource reads, not collections).

## Stable releases

Stable `1.0.0` will follow the criteria in [VERSIONING.md](../../VERSIONING.md). Until then, pin an exact prerelease version and read release notes before upgrading.
