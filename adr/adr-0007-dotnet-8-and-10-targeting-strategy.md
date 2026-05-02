---
title: "ADR-0007: .NET 8 and .NET 10 Targeting Strategy"
status: "Proposed"
date: "2026-05-01"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "target-frameworks", "compatibility"]
supersedes: ""
superseded_by: ""
---

# ADR-0007: .NET 8 and .NET 10 Targeting Strategy

## Status

**Proposed**

## Context

FreeAgent.NET is currently modernised around .NET 10 and has begun multi-targeting the SDK package for both `net8.0` and `net10.0`.

The project needs a clear support position before NuGet publication so that:

- Consumers on current LTS platforms can adopt the SDK without waiting for .NET 10 uptake.
- The SDK remains forward-focused on modern runtime capabilities.
- CI and release workflows provide explicit confidence for every advertised target.

Historical ADRs, especially ADR-0001, reflect the original baseline decision and should remain as historical records.

## Decision

Adopt a dual-target framework strategy for the SDK and its validation pipeline:

- `DEC-001`: Support `net8.0` and `net10.0` as official target frameworks for `FreeAgent.Client`.
- `DEC-002`: Treat `.NET 10` as the primary engineering focus for ongoing implementation choices.
- `DEC-003`: Validate both frameworks explicitly in CI and release workflows before package publication.
- `DEC-004`: Keep documentation explicit that both frameworks are supported, with .NET 10 recommended.
- `DEC-005`: Preserve historical ADR text; evolve policy through new ADRs rather than rewriting accepted history.
- `DEC-006`: Keep the Blazor sample app on `net10.0` only as an intentional tooling choice; this does not reduce SDK support commitments for `net8.0`.
- `DEC-007`: Interpret sample sync policy (ADR-0006) as endpoint capability parity, not target framework parity.

## Consequences

### Positive

- `POS-001`: Broadens immediate adoption by supporting the currently established LTS runtime.
- `POS-002`: Maintains a modern-first direction by keeping .NET 10 as the primary focus.
- `POS-003`: Reduces support risk through explicit per-framework build and test validation.
- `POS-004`: Clarifies expectations for contributors and package consumers.
- `POS-005`: Prevents unnecessary sample-app backport work while preserving honest endpoint coverage.

### Negative

- `NEG-001`: Increases CI execution time due to per-framework validation.
- `NEG-002`: Raises maintenance overhead when framework-specific behaviour diverges.
- `NEG-003`: Introduces potential pressure to retain dual-target support longer than desired.
- `NEG-004`: Creates an intentional mismatch between SDK target list and sample app target list that must be documented clearly.

## Alternatives Considered

### Keep .NET 10 only

- `ALT-001`: **Description**: Publish and validate only `net10.0` assets.
- `ALT-002`: **Rejection Reason**: Reduces near-term consumer reach while .NET 10 adoption matures.

### Make .NET 8 primary and .NET 10 secondary

- `ALT-003`: **Description**: Optimise engineering choices for .NET 8 first.
- `ALT-004`: **Rejection Reason**: Conflicts with project intent to stay modern-first.

### Treat .NET 8 as unofficial/experimental

- `ALT-005`: **Description**: Keep `net8.0` target but avoid support commitment.
- `ALT-006`: **Rejection Reason**: Creates ambiguity for consumers and weakens release confidence.

## Implementation Notes

- `IMP-001`: Multi-target `tests/FreeAgent.Client.Tests` for explicit parity with SDK targets.
- `IMP-002`: Keep `samples/FreeAgent.Client.Sample` on `net10.0` because it uses current Blazor and runtime APIs that are not available on .NET 8.
- `IMP-003`: Update CI and release workflows to run build/test checks for both `net8.0` and `net10.0` before pack/publish.
- `IMP-004`: Keep conditional compilation minimal and introduce framework-specific branches only when required by verified incompatibility.
- `IMP-005`: Review framework support position at regular release milestones and revise via a follow-on ADR if needed.
- `IMP-006`: When applying ADR-0006 sample sync rules, require endpoint/page parity only; do not require the sample app to mirror every SDK target framework.
- `IMP-007`: If a future cross-platform sample tool is added (for example CLI), it may target both frameworks without changing the Blazor sample's `net10.0` positioning.

## References

- `REF-001`: [adr-0001-core-technology-stack.md](adr-0001-core-technology-stack.md)
- `REF-002`: [src/FreeAgent.Client/FreeAgent.Client.csproj](../src/FreeAgent.Client/FreeAgent.Client.csproj)
- `REF-003`: [tests/FreeAgent.Client.Tests/FreeAgent.Client.Tests.csproj](../tests/FreeAgent.Client.Tests/FreeAgent.Client.Tests.csproj)
- `REF-004`: [samples/FreeAgent.Client.Sample/FreeAgent.Client.Sample.csproj](../samples/FreeAgent.Client.Sample/FreeAgent.Client.Sample.csproj)
- `REF-005`: [.github/workflows/ci.yml](../.github/workflows/ci.yml)
- `REF-006`: [.github/workflows/release.yml](../.github/workflows/release.yml)
