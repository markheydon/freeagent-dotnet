---
title: "ADR-0005: Handling Unknown or Undocumented Constrained Values in API Responses"
status: "Accepted"
date: "2026-05-01"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "error-handling", "enums"]
supersedes: ""
superseded_by: ""
---

# ADR-0005: Handling Unknown or Undocumented Constrained Values in API Responses

## Status

**Accepted**

## Context

The FreeAgent API occasionally returns values for constrained fields (e.g., enums) that are not documented or are added without notice. Most enums in the SDK are strict, mapping only documented values and throwing or failing on unknowns. However, for certain fields (e.g., `SalesTaxRegistrationStatus`), the SDK must tolerate unknown or undocumented values to avoid breaking deserialization and to aid diagnostics. This exception is justified by:

- Incomplete or outdated API documentation for some endpoints.
- Real-world evidence of undocumented values in production (e.g., "Unregistered").
- The need for developer diagnostics and resilience in the face of API drift.

## Decision

Adopt a selective tolerance policy for unknown or undocumented constrained values:

- By default, all enums and constrained fields are strict: only documented values are mapped.
- For fields with a history of undocumented or unstable values (e.g., `SalesTaxRegistrationStatus`), implement tolerant parsing with an `Unknown` fallback.
- Tolerant fields must be justified in code comments with a valid reason, such as missing documentation.
- All tolerant fields must surface unknown values in diagnostics and tests.
- Do not apply tolerant parsing to all enums; this is an explicit, case-by-case exception.

## Consequences

### Positive
- **POS-001**: Prevents runtime failures when FreeAgent returns undocumented values.
- **POS-002**: Enables rapid diagnosis and recovery from API drift.
- **POS-003**: Keeps most enums strict, preserving type safety and contract clarity.

### Negative
- **NEG-001**: Slightly increases implementation complexity for tolerant fields.
- **NEG-002**: May mask upstream API changes if diagnostics are not surfaced.
- **NEG-003**: Requires ongoing review of tolerant fields as API evolves.

## Alternatives Considered

### Strict enums everywhere
- **ALT-001**: **Description**: All enums throw or fail on unknown values.
- **ALT-002**: **Rejection Reason**: Breaks deserialization and developer experience when API returns undocumented values.

### Blanket tolerant parsing for all enums
- **ALT-003**: **Description**: All enums accept unknown values as `Unknown` or similar.
- **ALT-004**: **Rejection Reason**: Reduces type safety and makes contract drift harder to detect.

## Implementation Notes
- **IMP-001**: See `src/FreeAgent.Client/Models/Company/SalesTaxRegistrationStatus.cs` for tolerant enum implementation.
- **IMP-002**: See `tests/FreeAgent.Client.Tests/Services/Company/CompanyServiceTests.cs` for test coverage of unknown value handling.
- **IMP-003**: All tolerant fields must be documented in code including justification of the exception.

## References
- **REF-001**: [adr-0004-api-type-mapping-contracts.md](adr-0004-api-type-mapping-contracts.md)
- **REF-002**: [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md)
- **REF-003**: [src/FreeAgent.Client/Models/Company/SalesTaxRegistrationStatus.cs](../src/FreeAgent.Client/Models/Company/SalesTaxRegistrationStatus.cs)
- **REF-004**: [tests/FreeAgent.Client.Tests/Services/Company/CompanyServiceTests.cs](../tests/FreeAgent.Client.Tests/Services/Company/CompanyServiceTests.cs)
