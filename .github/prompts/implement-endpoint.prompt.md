---
name: implement-endpoint
description: Implement or retrofit one FreeAgent endpoint end-to-end with strict model guardrails, sample app sync, and plan-first workflow.
argument-hint: Entity name, e.g. "Company". Optional docs URL override.
---

## Usage

- Primary: `/implement-endpoint Company`
- Optional override: `/implement-endpoint Company https://dev.freeagent.com/docs/company`

## Goal

Implement one FreeAgent endpoint page end-to-end for this repository, including:

- SDK models and wrappers
- Service methods for documented use cases
- Tests
- Sample app page and navigation sync
- README/API coverage updates

If the entity already exists, retrofit it to current guardrails in the same run.

## Required Inputs

- `EntityName` (required)
- `DocsUrlOverride` (optional)

## Step 1 - Validate Entity Before Coding

1. Resolve the docs index: https://dev.freeagent.com/docs/index
2. Validate that `EntityName` maps to a real docs page.
3. If no matching endpoint page exists, stop immediately and report the invalid entity.
4. If `DocsUrlOverride` is provided, validate and use that page.

## Step 2 - Plan First (New or Retrofit)

1. Produce a concise implementation plan before editing code.
2. **If the entity already exists**, audit the existing models and services against these guardrails first:
   - Every serialized property has `JsonPropertyName`
   - Date-only API fields use `DateOnly` (not `DateTime`)
   - Timestamp fields use `DateTimeOffset` (not `DateTime`)
   - Constrained string fields are enums or strong value mappings with exact wire values
   - Response payloads use explicit wrapper/envelope models
   - Services validate wrappers and throw `FreeAgentApiException` on missing payload
3. Flag any guardrail violations found in existing code as retrofit tasks in the plan.
4. Include in the plan:
   - Endpoint use cases and HTTP routes (new) or existing service gaps (retrofit)
   - New files vs retrofit files (with specific violations listed)
   - Breaking API-surface changes expected
   - Test updates required
   - Sample app changes
   - Documentation changes

Proceed to implementation only after the plan is complete and all retrofit violations are listed.

## Step 3 - Delegate to C# Expert for Implementation and Retrofit

After the plan is approved, invoke the **C# Expert** agent to handle:

- **Audit and fix violations** in existing models (if any were flagged in the plan)
- Models and strongly-typed contracts — both new and retrofitted
- Service methods and pagination (new or existing gaps)
- Unit tests (updated for retrofitted code, new for new code)
- Sample app page implementation (Step 6 code only)

Provide the plan with explicit violation list and these guardrails to C# Expert:

### C# Expert Work - Guardrails to Apply

Apply these guardrails to all new and retrofitted models:

1. Use `System.Text.Json`.
2. Every serialized/deserialized property must have `JsonPropertyName`.
3. Use `DateOnly` for date-only API fields.
4. Use `DateTime`/`DateTimeOffset` only for timestamp fields per existing repo convention.
5. Constrained string fields must use enums or strong value mappings with exact wire-value behavior.
6. API-facing enums must use explicit `JsonStringEnumMemberName` wire values for each enum member.
7. When docs are ambiguous for constrained values, do not silently guess; mark unresolved mapping and add a follow-up issue.
8. Response payloads must use explicit wrapper/envelope models.
9. Missing required payload branches must throw `FreeAgentApiException`.

### C# Expert Work - Common Retrofit Violations to Check For

When auditing existing models, look for and fix:

1. **DateTime used instead of DateOnly** for date-only fields (e.g., `CompanyStartDate`, `TradingStartDate`)
   - API returns `"2024-01-15"` (no time component) → should be `DateOnly`
   - API returns `"2024-01-15T12:30:45Z"` (with time) → should be `DateTimeOffset`

2. **String fields that should be enums** (constrained values):
   - `Currency` → should be `CurrencyCode` enum (GBP, USD, EUR, etc.)
   - `Type` → should be `CompanyType` enum (UkLimitedCompany, UkSoleTrader, etc.)
   - `MileageUnits` → should be `MileageUnit` enum (miles, kilometres)
   - Any field documented with "e.g. value1, value2, value3" in API docs → needs enum

3. **Missing JsonPropertyName** on any serialized property

4. **Response wrappers not validated** in service methods (should throw `FreeAgentApiException`)

5. **DateTime used for timestamps** instead of `DateTimeOffset` (e.g., `CreatedAt`, `UpdatedAt`)

Fix all violations found in the plan before adding new functionality.

## Step 6 - C# Expert: Service and Pagination Rules

1. Follow existing service structure under `src/FreeAgent.Client/Services/`.
2. Keep methods async and accept `CancellationToken`.
3. For list endpoints, provide both:
   - Single-page method for deterministic control
   - Auto-pagination method for convenience
4. Respect FreeAgent `per_page` maximum 100.

## Step 7 - C# Expert: Tests Are Mandatory

C# Expert must add or update tests to cover:

- URL construction
- Envelope/wrapper deserialization
- Date handling (`DateOnly`)
- Enum/string wire mapping exactness
- Missing payload branch exceptions
- Pagination behavior and cancellation

## Step 8 - C# Expert: Sample App Sync Is Mandatory

When adding or changing endpoint support:

1. Add or update page(s) under `samples/FreeAgent.Client.Sample/Components/Pages/`.
2. Update navigation in `samples/FreeAgent.Client.Sample/Components/Layout/MainLayout.razor`.
3. Do not add sample UI for endpoints not implemented in SDK.
4. For developer-focused pages, show complete endpoint output (all mapped model properties).
5. Include a readable raw JSON output section for full payload inspection.

---

## Step 9 - Delegate to Tech Writer for Documentation

After C# Expert completes Steps 3–8, invoke the **Tech Writer** agent to handle:

- README updates (API coverage, usage examples)
- `plan/IMPLEMENTING_ENDPOINTS.md` alignment (if process changes were made)
- Any other Markdown documentation related to the endpoint

## Step 10 - Final Validation

Run from repository root to verify all code compiles and tests pass:

```bash
dotnet build
dotnet test
```

---

## Output Expectations (for you)

After both agents complete their work:

- Verify the plan was created or updated.
- Review C# Expert changes: models (including retrofit violations fixed), services, tests, sample page.
- Review Tech Writer changes: README and docs updates.
- Confirm `dotnet build` and `dotnet test` both pass.
- Highlight any breaking changes applied during retrofit (DateTime → DateOnly conversions, string → enum conversions).
