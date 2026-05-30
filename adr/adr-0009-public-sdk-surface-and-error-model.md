---
title: "ADR-0009: Public SDK Surface Boundary and Error Model"
status: "Accepted"
date: "2026-05-30"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "public-api", "exceptions", "oauth"]
supersedes: ""
superseded_by: ""
---

# ADR-0009: Public SDK Surface Boundary and Error Model

## Status

**Accepted**

## Context

The senior review remediation branch introduced a coordinated set of pre-1.0 changes to improve API ergonomics and reduce accidental coupling to internal implementation details.

Before this decision set:

- Several consumer-facing types were exposed from `Infrastructure.*` namespaces.
- Some environment and transport helpers intended for SDK internals were effectively discoverable by consumers.
- The public exception surface included multiple transport-layer subtypes that did not carry distinct consumer value.
- OAuth token expiry calculations relied on transient issue-time semantics that were not resilient to persistence round-trips.
- The sample app depended on internal namespace shapes to perform environment display and diagnostics tasks.

These conditions increased cognitive load for consumers, made public API intent less clear, and created avoidable maintenance pressure while the SDK remains pre-GA.

## Decision

Adopt a stricter public-surface boundary and simplified consumer model:

- `DEC-001`: Consumer-facing types must live in the top-level `FreeAgent.Client` namespace, including environment, options, OAuth, and core exception types.
- `DEC-002`: Internal transport and endpoint mapping helpers remain non-consumer-facing; `Infrastructure.*` is an implementation namespace, not a package-consumer contract.
- `DEC-003`: The public exception model is intentionally small and centred on `FreeAgentApiException`, `FreeAgentRateLimitException`, and `FreeAgentOAuthException`.
- `DEC-004`: OAuth token expiry semantics use a serialisable absolute UTC expiry representation to survive persistence round-trips.
- `DEC-005`: Sample application code must consume the public SDK surface and avoid direct dependency on internal endpoint helper types.

## Consequences

### Positive

- `POS-001`: Improves SDK discoverability and package-consumer ergonomics.
- `POS-002`: Clarifies what is stable public contract versus internal implementation detail.
- `POS-003`: Reduces long-term public API maintenance burden by keeping the exception hierarchy intentionally small.
- `POS-004`: Makes token expiry behaviour deterministic across in-memory and persisted token scenarios.
- `POS-005`: Reinforces sample-app honesty by keeping it aligned with intended consumer usage patterns.

### Negative

- `NEG-001`: Introduces pre-GA breaking changes for existing consumers importing `Infrastructure.*` namespaces.
- `NEG-002`: Requires migration updates in tests, sample code, and documentation in the same change set.
- `NEG-003`: Reduces granularity of transport-specific catch paths for consumers who preferred subtype catches.
- `NEG-004`: Requires continued discipline to prevent future internal types from leaking into the consumer surface.

## Alternatives Considered

### Keep current public namespace exposure

- `ALT-001`: **Description**: Continue exposing mixed consumer and infrastructure types under existing namespaces.
- `ALT-002`: **Rejection Reason**: Preserves ambiguity in the public contract and encourages tight coupling to internals.

### Retain the larger transport exception hierarchy

- `ALT-003`: **Description**: Keep `FreeAgentTransportException`, `FreeAgentNetworkException`, and `FreeAgentTimeoutException` as public consumer types.
- `ALT-004`: **Rejection Reason**: Added hierarchy depth without sufficient distinct consumer-facing behaviour to justify ongoing surface complexity.

### Keep transient token issue-time expiry model

- `ALT-005`: **Description**: Continue deriving expiry from transient issued-at state held in memory.
- `ALT-006`: **Rejection Reason**: Not robust for persistence round-trips and can misstate lifetime after process restarts.

## Implementation Notes

- `IMP-001`: Apply namespace and public-surface updates across SDK, tests, and sample in a single coordinated branch to avoid drift.
- `IMP-002`: Treat pre-1.0 breaking clean-up as acceptable when it reduces long-term public surface risk.
- `IMP-003`: Keep transport-specific test hooks internal-only and avoid re-expanding public method seams for diagnostics convenience.
- `IMP-004`: Keep documentation and examples aligned with the final public namespaces and exception model.

## References

- `REF-001`: [adr-0003-endpoint-implementation-workflow.md](adr-0003-endpoint-implementation-workflow.md)
- `REF-002`: [adr-0006-sample-app-living-reference.md](adr-0006-sample-app-living-reference.md)
- `REF-003`: [adr-0008-oauth-protocol-helpers-scope-boundary.md](adr-0008-oauth-protocol-helpers-scope-boundary.md)
- `REF-004`: [src/FreeAgent.Client/FreeAgentClient.cs](../src/FreeAgent.Client/FreeAgentClient.cs)
- `REF-005`: [src/FreeAgent.Client/Infrastructure/Http/FreeAgentHttpClient.cs](../src/FreeAgent.Client/Infrastructure/Http/FreeAgentHttpClient.cs)
- `REF-006`: [src/FreeAgent.Client/Infrastructure/Authentication/OAuthTokenResponse.cs](../src/FreeAgent.Client/Infrastructure/Authentication/OAuthTokenResponse.cs)
- `REF-007`: [README.md](../README.md)
