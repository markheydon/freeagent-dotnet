---
title: "ADR-0008: OAuth Protocol Helpers Scope Boundary"
status: "Accepted"
date: "2026-05-03"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "oauth", "scope"]
supersedes: ""
superseded_by: ""
---

# ADR-0008: OAuth Protocol Helpers Scope Boundary

## Status

**Accepted**

## Context

FreeAgent.NET previously stated that OAuth authorisation flows were out of scope beyond programmatic token exchange and refresh.

At the same time, the SDK includes `FreeAgentOAuthClient` with support for OAuth authorisation URL construction, code exchange, and token refresh. This introduced a scope mismatch between project documentation and implemented SDK capability.

The project requires a clear, stable boundary that reflects practical consumer expectations while preserving the SDK's architecture constraints (no UI/end-user tooling).

## Decision

Adopt and document a protocol-level OAuth boundary for v1.0:

- `DEC-001`: Keep OAuth protocol helpers in scope as SDK utilities, including authorisation URL construction, authorisation code exchange, and token refresh.
- `DEC-002`: Keep UI/browser journeys, callback endpoint hosting, and application-level OAuth orchestration out of scope.
- `DEC-003`: Keep app/session/token persistence concerns out of scope for the SDK.
- `DEC-004`: Align scope and goals documentation with the implemented OAuth helper surface.
- `DEC-005`: Treat OAuth helpers as SDK support primitives, not end-user flow management.

## Consequences

### Positive

- `POS-001`: Aligns documentation with actual SDK behaviour and removes scope ambiguity.
- `POS-002`: Matches typical developer expectations for API SDKs that integrate OAuth-protected APIs.
- `POS-003`: Preserves clean architecture boundaries by avoiding UI/framework-specific responsibilities.
- `POS-004`: Reduces duplicated consumer boilerplate for common OAuth protocol operations.

### Negative

- `NEG-001`: Expands perceived scope area, which may increase expectation pressure for broader OAuth features.
- `NEG-002`: Requires clear documentation to prevent assumptions that full web-flow orchestration is included.
- `NEG-003`: May require future API-surface tidy-up to ensure consumers do not rely on internal namespaces.

## Alternatives Considered

### Remove OAuth URL construction from the SDK

- `ALT-001`: **Description**: Limit SDK OAuth support to token exchange and refresh only, requiring consumers to build authorisation URLs themselves.
- `ALT-002`: **Rejection Reason**: Increases consumer friction and diverges from common SDK ergonomics.

### Keep current implementation but retain old out-of-scope wording

- `ALT-003`: **Description**: Leave functionality unchanged and avoid documentation updates.
- `ALT-004`: **Rejection Reason**: Preserves an explicit goals/scope contradiction and weakens project governance.

### Implement full OAuth browser flow orchestration in the SDK

- `ALT-005`: **Description**: Add callback hosting, state/session handling, and browser flow management into the SDK.
- `ALT-006`: **Rejection Reason**: Violates project boundaries by introducing UI/application concerns and framework coupling.

## Implementation Notes

- `IMP-001`: Update `GOALS.md` and `SCOPE.md` to include protocol-level OAuth helpers in scope and clarify out-of-scope orchestration concerns.
- `IMP-002`: Ensure docs consistently state that OAuth helpers are utilities, not complete end-user flow management.
- `IMP-003`: Review public namespace ergonomics in follow-on work so sample and consumers depend on intended public SDK surface only.

## References

- `REF-001`: [GOALS.md](../GOALS.md)
- `REF-002`: [SCOPE.md](../SCOPE.md)
- `REF-003`: [src/FreeAgent.Client/Infrastructure/Authentication/FreeAgentOAuthClient.cs](../src/FreeAgent.Client/Infrastructure/Authentication/FreeAgentOAuthClient.cs)
- `REF-004`: [samples/FreeAgent.Client.Sample/Services/OAuthService.cs](../samples/FreeAgent.Client.Sample/Services/OAuthService.cs)
- `REF-005`: [samples/FreeAgent.Client.Sample/Services/TokenStore.cs](../samples/FreeAgent.Client.Sample/Services/TokenStore.cs)
