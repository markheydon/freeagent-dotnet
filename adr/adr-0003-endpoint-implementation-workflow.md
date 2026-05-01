---
title: "ADR-0003: Endpoint Implementation Workflow and Guardrail Enforcement"
status: "Accepted"
date: "2026-05-01"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "workflow", "guardrails"]
supersedes: ""
superseded_by: ""
---

# ADR-0003: Endpoint Implementation Workflow and Guardrail Enforcement

## Status

**Accepted**

## Context

The FreeAgent.NET SDK must ensure that all API endpoint implementations are consistent, robust, and maintainable. Prior to this ADR, endpoint coverage was ad hoc, with inconsistent model mapping, test coverage, and sample app sync. The project now requires:

- Plan-first workflow for all endpoint work (new or retrofit).
- Strict model guardrails (serialisation, type mapping, validation).
- Mandatory agent routing for code vs. documentation.
- Sample app and navigation sync for every endpoint.

This decision is driven by:
- The need to prevent model drift and technical debt.
- The requirement for predictable, testable, and reviewable endpoint changes.
- The use of AI/agent-based workflows for implementation and documentation.

## Decision

Adopt a mandatory, plan-first endpoint implementation workflow for all SDK endpoints:

- All endpoint work begins with a plan that validates the entity against FreeAgent docs and audits existing code for guardrail violations.
- Guardrails enforced: every serialised property has `JsonPropertyName`, date-only fields use `DateOnly`, constrained strings use enums/strong values, wrappers are explicit, and services validate payloads.
- Agent routing is required: C# Expert for code/tests, Tech Writer for docs.
- Sample app and navigation must be updated in the same change as the endpoint.
- Breaking changes are allowed pre-GA if they improve correctness, but must be called out.

## Consequences

### Positive
- **POS-001**: Consistent, robust endpoint implementations with reduced technical debt.
- **POS-002**: Predictable review and onboarding for contributors and agents.
- **POS-003**: Sample app always reflects actual SDK capability.

### Negative
- **NEG-001**: Slower initial implementation due to plan-first and audit steps.
- **NEG-002**: Increased up-front effort for retrofitting existing endpoints.
- **NEG-003**: Requires ongoing discipline to maintain agent routing and sample sync.

## Alternatives Considered

### Ad hoc endpoint implementation
- **ALT-001**: **Description**: Allow contributors to implement endpoints as needed, with no enforced workflow or guardrails.
- **ALT-002**: **Rejection Reason**: Led to model drift, inconsistent tests, and sample app falling out of sync.

### Partial guardrail enforcement
- **ALT-003**: **Description**: Enforce only some model guardrails (e.g., serialisation but not type mapping).
- **ALT-004**: **Rejection Reason**: Incomplete enforcement allows technical debt and ambiguity to accumulate.

## Implementation Notes
- **IMP-001**: See `.github/prompts/implement-endpoint.prompt.md` for the canonical workflow and guardrail checklist.
- **IMP-002**: See `plan/IMPLEMENTING_ENDPOINTS.md` for the operational checklist.
- **IMP-003**: See `AGENTS.md` for agent routing and documentation requirements.

## References
- **REF-001**: [adr-0001-core-technology-stack.md](adr-0001-core-technology-stack.md)
- **REF-002**: [adr-0002-package-and-project-naming.md](adr-0002-package-and-project-naming.md)
- **REF-003**: [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md)
- **REF-004**: [plan/IMPLEMENTING_ENDPOINTS.md](../plan/IMPLEMENTING_ENDPOINTS.md)
- **REF-005**: [.github/prompts/implement-endpoint.prompt.md](../.github/prompts/implement-endpoint.prompt.md)
- **REF-006**: [AGENTS.md](../AGENTS.md)
