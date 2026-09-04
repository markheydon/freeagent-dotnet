# API to SDK Alignment

## Goal

Keep the SDK aligned with implemented FreeAgent API behaviour, with a repeatable process for detecting and fixing drift.

## Non-Goal

This document does not replicate FreeAgent API reference pages.

## Source of Truth Model

Use these sources in order:

1. FreeAgent API docs for endpoint contract and field semantics.
2. SDK code for implemented behaviour.
3. SDK tests for expected contract behaviour.
4. Sample app for real usage of implemented endpoints.

Any mismatch across these sources is drift.

## Drift Detection Workflow

Per endpoint:

1. Inventory every operation heading on the official FreeAgent docs page.
2. Confirm endpoint and payload shape against FreeAgent docs.
3. Compare model field types and wrappers with [plan/API_TYPE_MAPPING_POLICY.md](API_TYPE_MAPPING_POLICY.md).
4. Compare service behaviour, pagination, and payload guards — including whether each documented create/update variant has a typed SDK method.
5. Confirm tests cover mapping, envelope handling, errors, pagination, and at least one test per write variant.
6. Confirm sample probe pages and navigation reflect current implementation (see [`docs/contributing/sample-probe-pages.md`](../docs/contributing/sample-probe-pages.md)).
7. Record and classify drift before implementing changes.

## Drift Severity Matrix

| Drift Type | Severity | Typical Action |
|---|---|---|
| Missing endpoint implementation | High | Implement endpoint or explicitly mark out of scope |
| Collapsed or missing documented use-case variant | High | Add typed request and service method per variant; remove generic create/update |
| Constrained write value left as `string` with "see API docs" | High | Add per-variant or per-discriminator enum/factory per [adr-0010](../adr/adr-0010-documented-operations-to-sdk-methods.md) |
| Type mismatch against mapping policy | High | Retrofit model and tests |
| Wrapper or payload guard mismatch | High | Fix service and tests |
| Sample app out of sync with SDK | Medium | Update sample pages and navigation |
| Test gaps for implemented behaviour | Medium | Add tests before merge |
| Documentation wording mismatch | Low | Update docs |

## Audit Cadence

- Per endpoint PR: targeted drift check.
- Before release: full pass on all implemented endpoints.
- After notable FreeAgent API changes: targeted re-audit.

## Handling Intentional Deviations

1. Record the deviation and rationale in the PR.
2. Update [plan/API_TYPE_MAPPING_POLICY.md](API_TYPE_MAPPING_POLICY.md) when policy-level change is intended.
3. Add or update tests to lock in intentional behaviour.
4. Ensure sample app and docs reflect the chosen behaviour.

## Alignment Workflow

```mermaid
flowchart TD
    A[Select Endpoint] --> B[Read FreeAgent Docs]
    B --> C[Compare SDK Models and Services]
    C --> D{Drift Found}
    D -->|No| E[Confirm Tests and Sample]
    D -->|Yes| F[Classify Severity]
    F --> G[Plan Retrofit or Implementation]
    G --> H[Update Code Tests Sample Docs]
    H --> I[Validate Build and Tests]
    E --> I
```

## Implementation Notes

Use this document together with:

- [adr/adr-0010-documented-operations-to-sdk-methods.md](../adr/adr-0010-documented-operations-to-sdk-methods.md)
- [plan/IMPLEMENTING_ENDPOINTS.md](IMPLEMENTING_ENDPOINTS.md)
- [plan/API_TYPE_MAPPING_POLICY.md](API_TYPE_MAPPING_POLICY.md)

Together they define how endpoint work is planned, validated, and kept aligned over time.
