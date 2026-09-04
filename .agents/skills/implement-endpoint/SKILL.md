---
name: implement-endpoint
description: Implement or retrofit one FreeAgent endpoint end-to-end with strict model guardrails, sample app sync, and plan-first workflow. Use when adding or updating SDK resource services, models, tests, or sample pages for a FreeAgent API entity.
---

# Implement Endpoint

Implement one FreeAgent endpoint page end-to-end for this repository, including SDK models and wrappers, service methods, tests, sample app page and navigation sync, and README/API coverage updates.

If the entity already exists, retrofit it to current guardrails in the same run.

## Required Inputs

- `EntityName` (required)
- `DocsUrlOverride` (optional FreeAgent docs URL)

## Step 1 — Validate Entity and Inventory Operations

1. Resolve the docs index: https://dev.freeagent.com/docs/index
2. Validate that `EntityName` maps to a real docs page.
3. If no matching endpoint page exists, stop immediately and report the invalid entity.
4. If `DocsUrlOverride` is provided, validate and use that page.
5. **Fetch the docs page and list every operation heading** (`##` / `###`) before planning — for example "List all categories", "Get a single category", "Create an income category", "Create a cost of sales category", "Update an admin expenses category". The unit of implementation is the **documented operation heading**, not the HTTP route alone.

## Step 2 — Plan First (New or Retrofit)

1. Produce a concise implementation plan before editing code.
2. If the entity already exists, audit existing models and services against these guardrails:
   - Every serialised property has `JsonPropertyName`
   - Date-only API fields use `DateOnly` (not `DateTime`)
   - Timestamp fields use `DateTimeOffset` (not `DateTime`)
   - Constrained string fields are enums or strong value mappings with exact wire values
   - Response payloads use explicit wrapper/envelope models
   - Services validate wrappers and throw `FreeAgentApiException` on missing payload
3. Flag any guardrail violations as retrofit tasks in the plan.
4. Include in the plan:
   - **Operation heading inventory** from Step 1
   - A table mapping **heading → HTTP route → SDK method → request type → allowed fields** (new) or existing service gaps (retrofit)
   - **Documented use-case variants** — when the API docs describe multiple create/update shapes for the same route (for example income vs cost-of-sales categories), list each variant and the typed SDK method/request type it will map to
   - New files vs retrofit files (with specific violations listed)
   - Breaking API-surface changes expected
   - Test, sample app, and documentation changes

Proceed to implementation only after the plan is complete.

## Step 3 — Models and Guardrails

Apply to all new and retrofitted models:

1. Use `System.Text.Json`.
2. Every serialised/deserialised property must have `JsonPropertyName`.
3. Use `DateOnly` for date-only API fields.
4. Use `DateTimeOffset` for timestamp fields (not `DateTime`).
5. Constrained string fields must use enums or strong value mappings with exact wire-value behaviour.
6. API-facing enums must use explicit `JsonStringEnumMemberName` wire values for each enum member.
7. When docs are ambiguous for constrained values, do not silently guess; mark unresolved mapping and add a follow-up issue.
8. Response payloads must use explicit wrapper/envelope models.
9. Missing required payload branches must throw `FreeAgentApiException`.
10. **Per-variant allowed values:** when allowed wire keys differ by operation variant, use a distinct enum on that request only. When they also differ by a documented discriminator (for example company type), use discriminator-specific enums and factory methods — do not flatten into one enum, do not leave `string` plus "see the API docs", and do not add a runtime validator that fetches Company or account settings. See [adr-0010-documented-operations-to-sdk-methods.md](../../adr/adr-0010-documented-operations-to-sdk-methods.md).

Common retrofit violations:

- `DateTime` instead of `DateOnly` for date-only fields
- `DateTime` instead of `DateTimeOffset` for timestamps
- String fields that should be enums per API docs
- Missing `JsonPropertyName`
- Response wrappers not validated in service methods
- Generic create/update payload that unions all variant attributes

## Step 4 — Services and Pagination

1. Follow existing service structure under `src/FreeAgent.Client/Services/`.
2. Keep methods async and accept `CancellationToken`.
3. When the API paginates list results, provide both single-page and auto-pagination methods. Do not invent pagination for endpoints that return a complete collection (for example Categories).
4. Where the API accepts `per_page`, respect the FreeAgent maximum of 100 and fail fast if the caller exceeds it.
5. **Documented use-case variants:** when the API docs describe multiple create or update shapes for the same HTTP route, expose a separate public request type and service method per variant (for example `CreateIncomeCategoryAsync`, `CreateCostOfSalesCategoryAsync`). Each request type must include only the attributes allowed for that variant. Fixed wire values such as `category_group` are set by the SDK — callers must not supply them. Do not expose a single generic create/update that forces consumers to read external docs to learn which fields apply.
6. **Documented local contract checks:** fail fast only on constraints the official docs state that do not require account state. Do not invent extra validation, uniqueness checks, or fetches of Company/settings. Categories nominal-code ranges are one documented example, not a pattern to copy onto undocumented fields.

## Step 5 — Tests

Add or update tests to cover:

- URL construction
- Envelope/wrapper deserialisation
- Date handling (`DateOnly`)
- Enum/string wire mapping exactness
- Missing payload branch exceptions
- Pagination behaviour and cancellation **when the API paginates**
- **At least one test per documented write variant** — assert URL, envelope, and which fields are included or excluded in the serialised payload

## Step 6 — Sample App Sync

Follow the probe-page standard documented in [`docs/contributing/sample-probe-pages.md`](../../docs/contributing/sample-probe-pages.md). Use **Company** (single GET), **Contacts** (paginated list + CRUD), and **Categories** (non-paginated list + multi-variant writes) as reference implementations.

1. Add or update page(s) under `samples/FreeAgent.Client.BlazorSample/Components/Pages/`.
2. Update navigation in `samples/FreeAgent.Client.BlazorSample/Components/Layout/MainLayout.razor`.
3. Do not add sample UI for endpoints not implemented in SDK.
4. On each probe page, include:
   - `EndpointProbeHeader` (call under test, `DocsUrl`, environment, endpoint path)
   - `ModelProbeResults` built via `ModelWireDiagnostics.Build(...)` after successful SDK calls
   - `ApiErrorDiagnostics` on failures
   - A readable raw JSON section (provided by `ModelProbeResults`)
5. For **list** endpoints: per-row mapping inspection from the wire array item (see `Contacts.razor`).
6. For **CRUD** endpoints: detail page with `?id=` deep links; fetch wire JSON after create/update; show `MudProgressLinear` while operations run (see `ContactDetail.razor`).
7. When the SDK exposes **multiple write variants** for the same resource, the sample must be able to invoke each public write method — a variant selector on one CRUD page is sufficient; exercising only one variant (for example income-only) is not.
8. Add seed fixtures when demo data helps field coverage (narrative canon and/or a full-detail probe contact); upsert by a stable natural key when re-running should refresh existing records.
9. Only model wire fields that appear in the official FreeAgent API docs.

Update [`samples/README.md`](../../samples/README.md) and [`docs/reference/api-coverage.md`](../../docs/reference/api-coverage.md) in the same change.

## Step 7 — Documentation

Update the root `README.md`, [`src/FreeAgent.Client/README.md`](../../src/FreeAgent.Client/README.md) (API coverage, usage examples), [`docs/reference/api-coverage.md`](../../docs/reference/api-coverage.md), and any affected plan or entity-map sequencing docs.

## Step 8 — Validation

Run from repository root:

```bash
dotnet build
dotnet test
```

Highlight any breaking changes applied during retrofit (DateTime → DateOnly, string → enum).

## References

- `adr/adr-0010-documented-operations-to-sdk-methods.md`
- `docs/contributing/sample-probe-pages.md`
- `plan/IMPLEMENTING_ENDPOINTS.md`
- `plan/API_TYPE_MAPPING_POLICY.md`
- `plan/API_TO_SDK_ALIGNMENT.md`
- `CONVENTIONS.md`
- `AGENTS.md`
