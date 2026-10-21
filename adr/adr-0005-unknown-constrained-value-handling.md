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

The FreeAgent API occasionally returns values for constrained fields (e.g., enums) that are not documented or are added without notice. SDK wire enums must tolerate unknown tokens on **deserialise** so responses remain readable when FreeAgent adds values. Some fields (e.g., `SalesTaxRegistrationStatus`) also need bespoke normalisation. This is justified by:

- Incomplete or outdated API documentation for some endpoints.
- Real-world evidence of undocumented values in production (e.g., "Unregistered").
- The need for developer diagnostics and resilience in the face of API drift.

## Decision

Adopt a tolerant-on-read policy for wire enums and selected constrained value types:

- Public wire enums using `JsonStringEnumMemberNameCompatibleConverter<T>` include an `Unknown` member and map unrecognised wire strings to `Unknown` on deserialise (see issue #179).
- Serialising `Unknown` to the API throws `JsonException`; callers must use documented enum members for writes.
- Fields with normalisation or open-ended wire tokens (e.g., `SalesTaxRegistrationStatus`, `SecondSalesTaxStatus` with `WireValue`) use dedicated converters or structs.
- `WireEnumConventionsTests` guards that compatible-converter enums define `Unknown`.

## Consequences

### Positive
- **POS-001**: Prevents runtime failures when FreeAgent returns undocumented values.
- **POS-002**: Enables rapid diagnosis and recovery from API drift.
- **POS-003**: Documented enum members remain discoverable; `Unknown` signals API drift without failing GET deserialisation.

### Negative
- **NEG-001**: Slightly increases implementation complexity for tolerant fields.
- **NEG-002**: May mask upstream API changes if diagnostics are not surfaced.
- **NEG-003**: Requires ongoing review of tolerant fields as API evolves.

## Alternatives Considered

### Strict enums everywhere
- **ALT-001**: **Description**: All enums throw or fail on unknown values.
- **ALT-002**: **Rejection Reason**: Breaks deserialisation and developer experience when API returns undocumented values.

### Blanket tolerant parsing for all enums
- **ALT-003**: **Description**: All enums accept unknown values as `Unknown` or similar.
- **ALT-004**: **Rejection Reason**: Reduces type safety and makes contract drift harder to detect.

## Implementation Notes
- **IMP-001**: See `JsonStringEnumMemberNameCompatibleConverter<T>` for default unknown wire handling.
- **IMP-002**: See `src/FreeAgent.Client/Models/Company/SalesTaxRegistrationStatus.cs` for normalised tolerant enum implementation.
- **IMP-003**: See `src/FreeAgent.Client/Models/Shared/SecondSalesTaxStatus.cs` for preserving raw wire tokens on account-specific second sales tax codes.
- **IMP-004**: See `tests/FreeAgent.Client.Tests/Infrastructure/Serialization/WireEnumConventionsTests.cs` for enum guardrails.

## References
- **REF-001**: [adr-0004-api-type-mapping-contracts.md](adr-0004-api-type-mapping-contracts.md)
- **REF-002**: [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md)
- **REF-003**: [src/FreeAgent.Client/Models/Company/SalesTaxRegistrationStatus.cs](../src/FreeAgent.Client/Models/Company/SalesTaxRegistrationStatus.cs)
- **REF-004**: [tests/FreeAgent.Client.Tests/Services/Company/CompanyServiceTests.cs](../tests/FreeAgent.Client.Tests/Services/Company/CompanyServiceTests.cs)
