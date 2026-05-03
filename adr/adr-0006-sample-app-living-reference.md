---
title: "ADR-0006: Sample App as Living Reference and Sync Requirement"
status: "Accepted"
date: "2026-05-01"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "sample-app", "reference"]
supersedes: ""
superseded_by: ""
---

# ADR-0006: Sample App as Living Reference and Sync Requirement

## Status

**Accepted**

## Context

The FreeAgent.NET SDK includes a Blazor sample app that exercises all implemented endpoints. Historically, sample apps often lag behind SDK changes, leading to confusion about actual capabilities. This project now requires:

- Every implemented endpoint must have a corresponding sample app page/component.
- Sample app navigation must be updated in the same change as the endpoint.
- The sample app is the living reference for what the SDK can do today.

This decision is driven by:
- The need for honest, up-to-date developer validation and diagnostics.
- The requirement for contributors and agents to verify endpoint coverage and behaviour.
- The desire to avoid aspirational or out-of-date sample UI.

## Decision

Adopt a mandatory sample app sync policy:

- Every endpoint implemented in the SDK must have a corresponding sample app page/component.
- Sample app navigation must be updated in the same change.
- Sample app must not include UI for endpoints not yet implemented in the SDK.
- The sample app is the authoritative reference for current SDK capability.

## Consequences

### Positive
- **POS-001**: Ensures sample app always reflects actual SDK capability.
- **POS-002**: Improves developer onboarding, diagnostics, and validation.
- **POS-003**: Prevents drift between SDK and sample app.

### Negative
- **NEG-001**: Increases up-front effort for every endpoint change.
- **NEG-002**: May slow down rapid prototyping or experimentation.
- **NEG-003**: Requires ongoing discipline to maintain sample sync.

## Alternatives Considered

### Optional sample app sync
- **ALT-001**: **Description**: Allow sample app to lag behind SDK changes.
- **ALT-002**: **Rejection Reason**: Leads to confusion, drift, and reduced developer trust.

### Aspirational sample UI
- **ALT-003**: **Description**: Add sample UI for endpoints not yet implemented in SDK.
- **ALT-004**: **Rejection Reason**: Misleads users and reviewers about actual SDK capability.

## Implementation Notes
- **IMP-001**: See `.github/copilot-instructions.md` for sample app sync requirements.
- **IMP-002**: See `plan/IMPLEMENTING_ENDPOINTS.md` for operational checklist.
- **IMP-003**: See `samples/FreeAgent.Client.Sample/Components/Pages/` for sample app implementation.

## References
- **REF-001**: [adr-0003-endpoint-implementation-workflow.md](adr-0003-endpoint-implementation-workflow.md)
- **REF-002**: [plan/IMPLEMENTING_ENDPOINTS.md](../plan/IMPLEMENTING_ENDPOINTS.md)
- **REF-003**: [.github/copilot-instructions.md](../.github/copilot-instructions.md)
