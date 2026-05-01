# Implementing or Retrofitting FreeAgent Endpoints

Use this checklist when adding a new endpoint or retrofitting an existing entity so SDK code, tests, sample app, and docs stay aligned.

## 1. Validate the Entity First

- Validate entity name against https://dev.freeagent.com/docs/index.
- Stop immediately if no matching endpoint page exists.
- If a docs URL is explicitly provided, allow it as an override after validating it is reachable and relevant.

## 2. Plan First, Then Implement

- Start with a concise implementation plan before editing code.
- Include an explicit contract check against `plan/API_TYPE_MAPPING_POLICY.md`.
- Include a drift check against `plan/API_TO_SDK_ALIGNMENT.md`.
- Include scope for:
  - New endpoint implementation tasks.
  - Retrofit tasks for existing same-entity files.
  - Breaking API-surface changes (allowed pre-GA, but must be called out clearly).
  - Test updates.
  - Sample app updates.
  - README updates.

## 3. Model the API Contract (Strict Rules)

- Add request/response models under `src/FreeAgent.Client/Models/`.
- Use `System.Text.Json`.
- Add `JsonPropertyName` to every serialized/deserialized property, including envelope wrappers.
- Use `DateOnly` for date-only API fields.
- Use `DateTime`/`DateTimeOffset` only for timestamp fields, following existing repo convention.
- Model constrained string fields as enums or strong value mappings with exact wire-value behavior.
- Use `JsonStringEnumMemberName` on each API-facing enum member to make wire values explicit.
- Prefer strongly typed fields; use `JsonExtensionData` only when API shape is intentionally open-ended.
- Add XML docs to all public types and members.

## 4. Add or Retrofit Service Methods

- Add/extend service classes under `src/FreeAgent.Client/Services/`.
- Keep methods async and accept `CancellationToken`.
- Use clear naming that distinguishes single-page access from auto-pagination where relevant.
- Use explicit response wrappers and throw `FreeAgentApiException` when required payload nodes are missing.

## 5. Handle Pagination Explicitly

- For list endpoints, provide both:
  - Single-page method for deterministic page control.
  - Auto-pagination method for consumer convenience.
- Follow FreeAgent pagination limits (`per_page` max 100).
- Preserve cancellation support during pagination loops.

## 6. Cover Errors and Retries

- Ensure behavior works with shared HTTP retry/transport handling.
- Add tests for non-success responses and missing payload branches.
- Prefer typed exception assertions (`FreeAgentRateLimitException`, transport exceptions, and base `FreeAgentApiException` where appropriate).

## 7. Add or Update Tests

- Add/update service tests in `tests/FreeAgent.Client.Tests/Services/`.
- Add/update HTTP behavior tests in `tests/FreeAgent.Client.Tests/Infrastructure/Http/` if infrastructure behavior changes.
- Cover success paths, failures, and edge cases including:
  - Envelope/wrapper deserialization.
  - URL construction.
  - Enum wire-value mapping.
  - `DateOnly` handling.
  - Pagination cancellation behavior.

## 8. Keep the Sample App Honest (Mandatory)

- Add/update sample pages under `samples/FreeAgent.Client.Sample/Components/Pages/` for every implemented endpoint.
- Update sample navigation in `samples/FreeAgent.Client.Sample/Components/Layout/MainLayout.razor` in the same change.
- Do not add sample UI for endpoints not implemented in the SDK.
- For developer validation pages, include complete model output, not summary-only cards.
- Include a readable raw JSON section to inspect full payload shape from the SDK model.

## 9. Update README

- Update API coverage list.
- Add or update usage examples for new or retrofitted methods.
- Keep retry/error handling docs aligned with actual behavior.

## 10. Agent Routing

- Use `C# Expert` for C# implementation and test changes.
- Use `Tech Writer` for Markdown documentation updates.

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
- `.github/copilot-instructions.md`
- `.github/agents/CSharpExpert.agent.md`
- `.github/agents/tech-writer-mh.agent.md`
- `plan/API_TYPE_MAPPING_POLICY.md`
- `plan/API_TO_SDK_ALIGNMENT.md`
