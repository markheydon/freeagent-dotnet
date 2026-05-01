# Implementing New FreeAgent Endpoints

Use this checklist when adding endpoint coverage to keep SDK, tests, sample app, and docs in sync.

## 1. Model the API Contract

- Add request/response models under `src/FreeAgent.Client/Models/`.
- Map JSON names exactly with `JsonPropertyName`.
- Prefer strongly typed fields; use `JsonExtensionData` only where the API shape is intentionally open-ended.
- Add XML docs to all public types and members.

## 2. Add Service Methods

- Add/extend a service under `src/FreeAgent.Client/Services/`.
- Keep methods async and accept `CancellationToken`.
- Add clear method naming that distinguishes single-page access from auto-pagination where relevant.
- Validate response wrappers and throw `FreeAgentApiException` when required payload nodes are missing.

## 3. Handle Pagination Explicitly

- For list endpoints, provide both:
  - Single-page method (for deterministic page control).
  - Auto-pagination method (for consumer convenience).
- Follow FreeAgent pagination limits (`per_page` max 100).
- Preserve cancellation support during pagination loops.

## 4. Cover Errors and Retries

- Ensure method behavior works with shared HTTP retry/transport handling.
- Add tests for non-success responses and missing payload branches.
- Prefer typed exception assertions (`FreeAgentRateLimitException`, transport exceptions, and base `FreeAgentApiException` as appropriate).

## 5. Add/Update Tests

- Add service tests in `tests/FreeAgent.Client.Tests/Services/`.
- Add HTTP behavior tests in `tests/FreeAgent.Client.Tests/Infrastructure/Http/` if infrastructure behavior changes.
- Cover success paths, failures, and edge cases (including cancellation for auto-pagination).

## 6. Keep the Sample App Honest

- Add/update sample pages under `samples/FreeAgent.Client.Sample/Components/Pages/` for every implemented endpoint.
- Update sample navigation in `samples/FreeAgent.Client.Sample/Components/Layout/MainLayout.razor`.
- Do not add sample UI for endpoints not implemented in the SDK.

## 7. Update README

- Update API coverage list.
- Add or update usage examples for new methods.
- Keep retry/error handling documentation aligned with actual behavior.

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
