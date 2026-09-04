---
title: "ADR-0010: Documented Operations Map to Typed SDK Methods"
status: "Accepted"
date: "2026-09-04"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "public-api", "developer-experience", "workflow"]
supersedes: ""
superseded_by: ""
---

# ADR-0010: Documented Operations Map to Typed SDK Methods

## Status

**Accepted**

## Context

The FreeAgent API docs describe operations by **heading** (for example, "Create an income category", "Create a cost of sales category"), not only by HTTP route. Several resources share the same `POST` or `PUT` path but accept different attribute sets. The Categories resource was the first endpoint implemented under the plan-first `implement-endpoint` workflow; the initial implementation exposed a single `CreateCategoryAsync(CategoryWritePayload)` method. That forced consumers to read FreeAgent docs to learn which fields apply to each category type.

This conflicts with:

- **G2** in [GOALS.md](../GOALS.md): Stripe.NET-quality developer experience — discoverable, consistent, pleasant to use.
- **G1**: strongly typed SDK for the FreeAgent REST API.
- [SCOPE.md](../SCOPE.md): strongly typed request and response models.
- [ADR-0003](adr-0003-endpoint-implementation-workflow.md): plan-first workflow with model guardrails.

The workflow skill told agents to plan "endpoint use cases and HTTP routes", which was interpreted as **one public method per HTTP verb**. Contacts has only one create/update shape, so the gap did not surface until Categories.

A related problem appears at the field level: constrained write values documented in tables (for example `tax_reporting_name` on Categories) vary by operation variant and by company type. Leaving these as `string` with "see the API docs" remarks recreates the same developer-experience failure.

## Decision

Adopt **documented operation heading** as the unit of public SDK surface design, not HTTP route alone.

- **DEC-001**: Before implementation, inventory every operation heading (`##` / `###`) on the official FreeAgent docs page for the entity.
- **DEC-002**: Each documented operation heading maps to a public service method (or is explicitly marked out of scope with rationale in the plan).
- **DEC-003**: When the same HTTP route has multiple create or update shapes, expose a **separate public request type and service method per variant**. Each request type includes only the attributes documented for that variant. Fixed wire values (for example `category_group`) are set by the SDK — callers must not supply them.
- **DEC-004**: Do not ship a generic create/update method that unions all variant attributes and forces consumers to the API docs.
- **DEC-005**: Documented allowed-value tables on write operations are constrained strings and must be typed. When the allowed set differs by operation variant, use a **distinct enum on that request only**. When the set also differs by a documented discriminator (for example company type), use **discriminator-specific enums and factory methods** (for example `CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(..., UkLimitedCompanyCostOfSalesTaxReportingName.Purchases)`). Do not flatten into one enum and do not add a runtime validator that fetches Company or account settings.
- **DEC-006**: Feature-flagged values (for example CIS-only tax reporting names) remain on the relevant enum with XML remarks. FreeAgent remains the authority for live account state.

## Consequences

### Positive

- **POS-001**: Consumers can perform documented write operations from IntelliSense without reconstructing payloads from FreeAgent docs.
- **POS-002**: Compile-time type safety prevents selecting values invalid for the chosen operation variant or company type.
- **POS-003**: Aligns SDK design with G2 (Stripe.NET-quality discoverability) without crossing into business-rule abstraction.
- **POS-004**: Gives agents and contributors an unambiguous checklist: every docs heading must have a typed SDK counterpart.

### Negative

- **NEG-001**: More public types and methods when a resource has many documented variants (Categories has twelve create/update headings).
- **NEG-002**: Discriminator-specific factories add API surface area compared to a single enum or string field.
- **NEG-003**: Requires retrofit of other endpoints that still expose generic write payloads or string constrained fields.

## Alternatives Considered

### One public method per HTTP verb with a generic payload

- **ALT-001**: **Description**: Expose `CreateCategoryAsync(CategoryWritePayload)` and `UpdateCategoryAsync(nominalCode, CategoryWritePayload)` for all category types.
- **ALT-002**: **Rejection Reason**: Forces consumers to read FreeAgent docs for required fields and valid values; fails G2.

### Runtime validator on string fields

- **ALT-003**: **Description**: Keep `tax_reporting_name` as `string` and validate against allowed values at runtime, optionally fetching Company to determine company type.
- **ALT-004**: **Rejection Reason**: Fails late, still requires docs knowledge to pick values, and fetching Company couples writes to another resource and account-settings logic (business-rule/orchestration territory per SCOPE.md).

### Single union enum of all allowed values

- **ALT-005**: **Description**: One `TaxReportingName` enum containing every wire key from every company type and category variant.
- **ALT-006**: **Rejection Reason**: IntelliSense offers values invalid for the caller's company type and operation variant; easy to get wrong.

## Implementation Notes

- **IMP-001**: See [.agents/skills/implement-endpoint/SKILL.md](../.agents/skills/implement-endpoint/SKILL.md) Step 1 (heading inventory), Step 2 (plan table: heading → route → method → request → fields), Step 4 (variant rule), and Step 5 (one test per write variant).
- **IMP-002**: See [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md) for constrained write value rules.
- **IMP-003**: See [CONVENTIONS.md](../CONVENTIONS.md) for request/method naming when variants exist.
- **IMP-004**: Categories typed methods and per-discriminator `tax_reporting_name` enums with factory methods are the reference implementation for ADR-0010.

## References

- **REF-001**: [adr-0003-endpoint-implementation-workflow.md](adr-0003-endpoint-implementation-workflow.md)
- **REF-002**: [adr-0004-api-type-mapping-contracts.md](adr-0004-api-type-mapping-contracts.md)
- **REF-003**: [GOALS.md](../GOALS.md)
- **REF-004**: [SCOPE.md](../SCOPE.md)
- **REF-005**: [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md)
- **REF-006**: [.agents/skills/implement-endpoint/SKILL.md](../.agents/skills/implement-endpoint/SKILL.md)
