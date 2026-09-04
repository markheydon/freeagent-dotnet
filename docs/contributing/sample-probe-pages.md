# Sample probe pages

How to build and maintain Blazor sample pages that validate SDK wire-to-model mapping against the live FreeAgent API.

Audience: contributors implementing or retrofitting SDK endpoints.

## Purpose

The sample app is a **developer workbench**, not a product UI. Each probe page should answer:

1. Does the SDK call succeed against a real sandbox account?
2. Does every model property map correctly to the wire JSON FreeAgent returns?
3. Can a contributor spot mismatches, missing fields, and read-only attributes quickly?

**Reference implementations:**

| Pattern | Sample routes | SDK surface |
|---------|---------------|-------------|
| Single-resource GET | `/company` | `CompanyService.GetCompanyAsync()` |
| List + per-row mapping | `/contacts` | `ContactService.GetContactsPageAsync(...)` |
| CRUD + seed data | `/contacts/detail` | `ContactService` create/get/update/delete |

Read those pages before adding a new resource.

## Shared building blocks

Reuse these sample-only components and services. Do not duplicate probe logic on new pages.

| Asset | Location | Role |
|-------|----------|------|
| `EndpointProbeHeader` | `Components/Shared/EndpointProbeHeader.razor` | Page title, call under test, API docs link, environment, endpoint path |
| `ModelProbeResults` | `Components/Shared/ModelProbeResults.razor` | Mapping summary chips, mapping table, all-fields table, raw JSON |
| `ModelWireDiagnostics` | `Services/ModelWireDiagnostics.cs` | Builds `ModelProbeSnapshot` with SDK-aware equivalence checks |
| `ApiDiagnosticsService` | `Services/ApiDiagnosticsService.cs` | Fetches raw wire JSON for a path after an SDK call |
| `ApiErrorDiagnostics` | `Components/Shared/ApiErrorDiagnostics.razor` | Shows raw error payload on API failures |

### Wire-to-model comparison rules

`ModelWireDiagnostics` compares wire JSON to the deserialized SDK model by:

1. Deserialising each wire field through the **same property type** the model uses.
2. Using `JsonNumberHandling.AllowReadingFromString` so decimal fields returned as strings (for example `account_balance`) do not false-positive.
3. Classifying each property as **Match**, **Mismatch**, **Missing in model**, **Model-only**, or **Not returned**.

Only add model properties that exist on the [FreeAgent API docs](https://dev.freeagent.com/docs/index). Do not invent wire fields (for example `contact_name` is not part of the contacts API).

## Page patterns

### Single GET (one resource)

Follow `Components/Pages/Company.razor`:

1. Render `EndpointProbeHeader` with the SDK method, `/v2/...` path, and `DocsUrl` pointing at the official FreeAgent docs page for the resource.
2. On load (or button click), call the SDK service.
3. Fetch the matching raw payload via `ApiDiagnosticsService`.
4. Build `ModelProbeResults` with `ModelWireDiagnostics.Build(model, rawPayload, envelopeProperty)`.
5. Show `ApiErrorDiagnostics` when the call fails.

### List GET (paginated collection)

Follow `Components/Pages/Contacts.razor`:

1. Load one page through the SDK (`Get*PageAsync`).
2. Fetch the list wire payload for the same query.
3. For each row, offer **Inspect mapping** using `ModelWireDiagnostics.TryGetArrayItem` to pass the array item element into `ModelProbeResults`.
4. Expose filters the SDK supports (view, sort, `updated_since`, and so on).
5. Link to a detail/CRUD page with `?id=` when the resource has an identifier.

### CRUD (create, read, update, delete)

Follow `Components/Pages/ContactDetail.razor`:

1. Support `?id=` deep links for get/update/delete.
2. After **create** or **update**, fetch `GET /resource/:id` wire JSON so diagnostics reflect persisted state.
3. Show `MudProgressLinear` while `_loading` is true (bulk seed operations can take several seconds).
4. Disable action buttons during in-flight requests.

Optional seed helpers (when useful for testing):

- **Narrative demo data** — bundled canon (see Turpinverse under `Data/` and `Services/Turpinverse/`). FreeAgent contact address fields are billing-oriented (they appear on invoices), so Turpinverse maps the primary organisation's `registeredOffice` — not persona home addresses.
- **Full-field probe fixture** — one contact with every writable attribute populated (`SampleContactFixtures`, `SampleContactSeeder`).
- **Upsert by stable key** — match existing records by email (contacts) or an equivalent natural key; update when canon changes (`ContactSeederSupport`).

## Navigation and honesty

- Register new routes in `Components/Layout/MainLayout.razor`.
- Add the page to the table in [`samples/README.md`](../../samples/README.md).
- Do not add sample UI for SDK endpoints that do not exist yet (see ADR-0006).

## Checklist for a new resource

- [ ] SDK models, service, and tests land in the same PR as the sample pages.
- [ ] List probe page (if the API supports listing) with per-row mapping inspection.
- [ ] Detail or CRUD probe page (if the API supports get/create/update/delete).
- [ ] `EndpointProbeHeader` on each page with accurate `CallUnderTest`, `EndpointPath`, and `DocsUrl`.
- [ ] `ModelProbeResults` after successful SDK calls; `ApiErrorDiagnostics` on failures.
- [ ] Raw JSON panel present for full payload inspection.
- [ ] Navigation group added in `MainLayout.razor`.
- [ ] [`docs/reference/api-coverage.md`](../reference/api-coverage.md) and [`samples/README.md`](../../samples/README.md) updated.

## Related documents

- [`plan/IMPLEMENTING_ENDPOINTS.md`](../../plan/IMPLEMENTING_ENDPOINTS.md) — full endpoint checklist
- [`.agents/skills/implement-endpoint/SKILL.md`](../../.agents/skills/implement-endpoint/SKILL.md) — agent workflow
- [`AGENTS.md`](../../AGENTS.md) — sample sync policy
- [`adr/adr-0006-sample-app-living-reference.md`](../../adr/adr-0006-sample-app-living-reference.md)
