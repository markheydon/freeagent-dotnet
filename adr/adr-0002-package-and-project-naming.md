---
title: "ADR-0002: Package and Project Naming"
status: "Accepted"
date: "2026-04-30"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "naming", "packaging"]
supersedes: ""
superseded_by: ""
---

# ADR-0002: Package and Project Naming

## Status

**Accepted**

## Context

The project currently uses mixed naming across repository, documentation, and NuGet metadata. Documentation and project context files describe the SDK as "FreeAgent.NET", while the published package identifier is "FreeAgent.Client".

A separate historical package attempt using the "FreeAgent.NET" naming pattern already exists on NuGet. To avoid ambiguity and future collision risk, naming must be explicitly standardised before MVP implementation work expands the public API surface.

## Decision

Adopt a three-part naming strategy:

- **DEC-001**: Product and documentation name remains **FreeAgent.NET**.
- **DEC-002**: Repository name remains **freeagent-dotnet**.
- **DEC-003**: NuGet package identifier remains **FreeAgent.Client** for MVP and prerelease phases.

Rationale:

- **DEC-004**: Keeps the user-facing brand aligned with existing project intent and documentation.
- **DEC-005**: Avoids direct package-name conflict and confusion with historical "FreeAgent.NET" package lineage.
- **DEC-006**: Minimises technical churn before MVP by preserving current package identity used in docs and install instructions.

## Consequences

### Positive

- **POS-001**: Immediate naming consistency is established across project governance and package distribution.
- **POS-002**: MVP delivery risk is reduced by avoiding a package rename during active prerelease iteration.
- **POS-003**: The project can continue marketing under "FreeAgent.NET" while using a pragmatic, available package ID.

### Negative

- **NEG-001**: Package name "FreeAgent.Client" is less distinctive than the preferred brand name.
- **NEG-002**: Brand-to-package mismatch may require occasional clarification in README and release notes.
- **NEG-003**: A future rename remains possible and would require migration guidance for consumers.

## Alternatives Considered

### Use `FreeAgentNet.Client` as package ID now

- **ALT-001**: **Description**: Rename the current NuGet package to a brand-adjacent unique identifier.
- **ALT-002**: **Rejection Reason**: Deferred to avoid additional rename churn before MVP and to preserve continuity for current prerelease consumers.

### Use `MarkHeydon.FreeAgent.Client` as package ID

- **ALT-003**: **Description**: Adopt an owner-prefixed package ID to maximise uniqueness.
- **ALT-004**: **Rejection Reason**: Rejected for now due to reduced discoverability and less natural naming for consumers.

### Use `FreeAgent.NET` as package ID

- **ALT-005**: **Description**: Align package ID exactly with project brand.
- **ALT-006**: **Rejection Reason**: Rejected due to existing historical usage and high ambiguity/conflict risk on NuGet.

## Implementation Notes

- **IMP-001**: Keep current package metadata (`PackageId=FreeAgent.Client`) unchanged for MVP/prerelease period.
- **IMP-002**: Ensure docs consistently explain: product name "FreeAgent.NET", install package "FreeAgent.Client".
- **IMP-003**: Re-evaluate package ID before first stable `1.0.0`; if changed, publish migration notes and compatibility guidance.

## References

- **REF-001**: [ADR-0001 Core Technology Stack](./adr-0001-core-technology-stack.md)
- **REF-002**: [README](../README.md)
- **REF-003**: [VERSIONING](../VERSIONING.md)
