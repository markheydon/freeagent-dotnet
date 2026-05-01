---
title: "ADR-0004: API Type Mapping and Serialisation Contract Policy"
status: "Accepted"
date: "2026-05-01"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "type-mapping", "serialisation"]
supersedes: ""
superseded_by: ""
---

# ADR-0004: API Type Mapping and Serialisation Contract Policy

## Status

**Accepted**

## Context

The FreeAgent.NET SDK must map API fields to .NET types in a way that is robust, predictable, and maintainable. Prior to this ADR, model mapping was inconsistent, with stringly-typed fields, ambiguous date handling, and implicit enum serialisation. The project now requires:

- Canonical mapping of API field kinds to .NET types (e.g., string->enum, date->DateOnly).
- Explicit wire-value mapping for all API-facing enums.
- Use of `JsonPropertyName` and `JsonStringEnumMemberName` for all serialised properties
- Wrapper/envelope models for all responses.
- Pre-GA: breaking changes allowed to improve correctness.

This decision is driven by:
- The need to prevent model drift and ensure contract clarity.
- The requirement for strong typing and explicit serialisation for all API-facing models.
- The need for predictable, testable, and reviewable model changes.

## Decision

Adopt a strict API type mapping and serialisation contract policy:

- All API fields are mapped according to the canonical table in `plan/API_TYPE_MAPPING_POLICY.md`.
- All constrained string fields are mapped to enums or strong value types with explicit wire values.
- All date-only fields use `DateOnly`; all timestamps use `DateTimeOffset`.
- All serialised properties have `JsonPropertyName`; all enums have `JsonStringEnumMemberName`.
- Wrapper/envelope models are required for all responses.
- Breaking changes to model mapping are allowed pre-GA, but must be called out.

## Consequences

### Positive
- **POS-001**: Strongly-typed, predictable models with reduced ambiguity.
- **POS-002**: Easier to audit, test, and review model changes.
- **POS-003**: Enables safe, automated retrofitting of existing endpoints.

### Negative
- **NEG-001**: Requires retrofitting existing models and tests.
- **NEG-002**: May break consumer code pre-GA as types are tightened.
- **NEG-003**: Increases up-front effort for new endpoint implementation.

## Alternatives Considered

### Ad hoc type mapping
- **ALT-001**: **Description**: Allow contributors to map API fields as needed, with no enforced policy.
- **ALT-002**: **Rejection Reason**: Led to stringly-typed models, ambiguous serialisation, and model drift.

### Partial type mapping enforcement
- **ALT-003**: **Description**: Enforce only some type mapping rules (e.g., date handling but not enums).
- **ALT-004**: **Rejection Reason**: Incomplete enforcement allows ambiguity and technical debt to accumulate.

## Implementation Notes
- **IMP-001**: See `plan/API_TYPE_MAPPING_POLICY.md` for the canonical mapping table and policy.
- **IMP-002**: See `plan/IMPLEMENTING_ENDPOINTS.md` for the operational checklist.
- **IMP-003**: See `src/FreeAgent.Client/Models/Company/Company.cs` for a retrofitted model example.

## References
- **REF-001**: [adr-0001-core-technology-stack.md](adr-0001-core-technology-stack.md)
- **REF-002**: [adr-0002-package-and-project-naming.md](adr-0002-package-and-project-naming.md)
- **REF-003**: [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md)
- **REF-004**: [plan/IMPLEMENTING_ENDPOINTS.md](../plan/IMPLEMENTING_ENDPOINTS.md)
