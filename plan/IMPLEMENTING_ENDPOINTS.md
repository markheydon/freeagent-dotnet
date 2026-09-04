# Implementing or Retrofitting FreeAgent Endpoints

Use this checklist when adding a new endpoint or retrofitting an existing entity so SDK code, tests, sample app, and docs stay aligned.

## 1. Validate the Entity First

- Validate entity name against https://dev.freeagent.com/docs/index.
- Stop immediately if no matching endpoint page exists.
- If a docs URL is explicitly provided, allow it as an override after validating it is reachable and relevant.
- **Fetch the docs page and inventory every operation heading** (`##` / `###`) before planning. The unit of implementation is the documented operation heading, not the HTTP route alone (see [adr-0010-documented-operations-to-sdk-methods.md](../adr/adr-0010-documented-operations-to-sdk-methods.md)).

## 2. Plan First, Then Implement

- Consult the [API entity map](../docs/explanation/api-entity-map.md) before choosing the next resource — it shows documented URI dependencies and suggested implementation layers.
- Start with a concise implementation plan before editing code.
- Include an explicit contract check against `plan/API_TYPE_MAPPING_POLICY.md`.
- Include a drift check against `plan/API_TO_SDK_ALIGNMENT.md`.
- Include scope for:
  - **Operation heading inventory** and a table mapping **heading → HTTP route → SDK method → request type → allowed fields**.
  - New endpoint implementation tasks.
  - Retrofit tasks for existing same-entity files.
  - **Documented use-case variants** — when one HTTP route has multiple create/update shapes in the API docs, plan a typed request and service method per variant rather than a single generic payload.
  - Breaking API-surface changes (allowed pre-GA, but must be called out clearly).
  - Test updates.
  - Sample app updates.
  - README updates.

## 3. Model the API Contract (Strict Rules)

- Add request/response models under `src/FreeAgent.Client/Models/`.
- Use `System.Text.Json`.
- Add `JsonPropertyName` to every serialised/deserialised property, including envelope wrappers.
- Use `DateOnly` for date-only API fields.
- Use `DateTime`/`DateTimeOffset` only for timestamp fields, following existing repo convention.
- Model constrained string fields as enums or strong value mappings with exact wire-value behaviour.
- Use `JsonStringEnumMemberName` on each API-facing enum member to make wire values explicit.
- Prefer strongly typed fields; use `JsonExtensionData` only when API shape is intentionally open-ended.
- Add XML docs to all public types and members.
- When allowed wire keys on write operations differ by operation variant, use a distinct enum on that request. When they also differ by a documented discriminator (for example company type), use discriminator-specific enums and factory methods — see [adr-0010-documented-operations-to-sdk-methods.md](../adr/adr-0010-documented-operations-to-sdk-methods.md).

## 4. Add or Retrofit Service Methods

- Add/extend service classes under `src/FreeAgent.Client/Services/`.
- Keep methods async and accept `CancellationToken`.
- Use clear naming that distinguishes single-page access from auto-pagination where relevant.
- When the API docs describe multiple create or update variants for the same route, add a separate public request type and service method per variant. Each request type exposes only the attributes documented for that variant; fixed wire values are set inside the SDK.
- Do not expose a generic create/update that unions all variant attributes.
- Use explicit response wrappers and throw `FreeAgentApiException` when required payload nodes are missing.

## 5. Handle Pagination Explicitly

- For list endpoints, provide both:
  - Single-page method for deterministic page control.
  - Auto-pagination method for consumer convenience.
- Follow FreeAgent pagination limits (`per_page` max 100).
- Preserve cancellation support during pagination loops.

## 6. Cover Errors and Retries

- Ensure behaviour works with shared HTTP retry/transport handling.
- Add tests for non-success responses and missing payload branches.
- Prefer typed exception assertions (`FreeAgentRateLimitException`, transport exceptions, and base `FreeAgentApiException` where appropriate).

## 7. Add or Update Tests

- Add/update service tests in `tests/FreeAgent.Client.Tests/Services/`.
- Add/update HTTP behaviour tests in `tests/FreeAgent.Client.Tests/Infrastructure/Http/` if infrastructure behaviour changes.
- Cover success paths, failures, and edge cases including:
  - Envelope/wrapper deserialisation.
  - URL construction.
  - Enum wire-value mapping.
  - `DateOnly` handling.
  - Pagination cancellation behaviour.
  - At least one test per documented write variant (payload fields included/excluded).

## 8. Keep the Sample App Honest (Mandatory)

Follow [`docs/contributing/sample-probe-pages.md`](../docs/contributing/sample-probe-pages.md). **Company** and **Contacts** are the reference probe implementations.

- Add/update sample pages under `samples/FreeAgent.Client.Sample/Components/Pages/` for every implemented endpoint.
- Update sample navigation in `samples/FreeAgent.Client.Sample/Components/Layout/MainLayout.razor` in the same change.
- Do not add sample UI for endpoints not implemented in the SDK.
- Reuse shared probe components:
  - `EndpointProbeHeader` — page context and call under test
  - `ModelWireDiagnostics` + `ModelProbeResults` — wire-to-model mapping with filter chips and raw JSON
  - `ApiErrorDiagnostics` — failed API responses
- **List endpoints:** load a page via the SDK, fetch matching wire JSON, and offer per-row mapping inspection (`Contacts.razor`).
- **CRUD endpoints:** support `?id=` deep links, fetch wire JSON after create/update, and show a progress bar during long operations (`ContactDetail.razor`).
- **Multiple write variants:** when the SDK exposes more than one create or update method for a resource, the sample must be able to invoke each (variant selector on one page is sufficient).
- **Seed data (when useful):** provide narrative demo data and/or a full-field probe fixture; upsert by a stable key so re-running updates existing sandbox records.
- Model only fields documented in the official FreeAgent API — do not invent wire properties.
- Update [`samples/README.md`](../samples/README.md) and [`docs/reference/api-coverage.md`](../docs/reference/api-coverage.md).
- If the official docs expose URI links not yet shown on the relevant [entity map cluster](../docs/explanation/api-entity-map.md), update that cluster page in the same change.

## 9. Update README

- Update API coverage list.
- Add or update usage examples for new or retrofitted methods.
- Keep retry/error handling docs aligned with actual behaviour.

## 10. Agent Routing

- Follow `AGENTS.md` task routing for SDK work vs documentation.
- Use the `implement-endpoint` skill in `.agents/skills/implement-endpoint/` for endpoint implementation and retrofit.

## Validation

Run from repository root:

```bash
dotnet build
dotnet test
```

References:
- `GOALS.md`
- `SCOPE.md`
- `CONVENTIONS.md`
- `AGENTS.md`
- `adr/adr-0010-documented-operations-to-sdk-methods.md`
- `.agents/skills/implement-endpoint/SKILL.md`
- `plan/API_TYPE_MAPPING_POLICY.md`
- `plan/API_TO_SDK_ALIGNMENT.md`
