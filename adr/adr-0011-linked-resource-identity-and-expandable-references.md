---
title: "ADR-0011: Linked Resource Identity and Expandable References"
status: "Accepted"
date: "2026-09-21"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "public-api", "serialisation", "developer-experience"]
supersedes: ""
superseded_by: ""
---

# ADR-0011: Linked Resource Identity and Expandable References

## Status

**Accepted**

## Context

FreeAgent models relationships between resources using **URI strings** on the wire (for example `contact` on a project). List endpoints may return those links as a URI string or as a **nested object** when an expand-style query parameter is used (for example `nested=true` on projects). Single-resource GET responses often return only the URI plus denormalised display fields (for example `contact_name` on projects), not the full linked resource.

The initial Projects implementation mapped link fields to `string?` and discarded nested contact objects. A follow-up attempt exposed `ExpandableField<T>` on public models, which leaked wire-format concepts (`.Value`) and conflicted with **G2** in [GOALS.md](../GOALS.md): consumers should not need FreeAgent API archaeology to use the SDK pleasantly.

As Invoices, Tasks, Timeslips, and Bills are implemented, each will carry multiple URI link fields. Without a shared pattern, the SDK will accumulate stringly-typed links and per-resource JSON converters.

## Decision

Adopt **typed resource references**, **flat public read properties**, and **optional explicit hydration** on single-resource GET. This is protocol-level convenience only - no lazy-loading properties, no ORM, and no fetches of other resources to validate payloads.

- **DEC-001**: Top-level resource models implement `IFreeAgentResource` (`Url` plus computed `ResourceId` from the URL, except `Company` which uses its documented `id` field).
- **DEC-002**: **Write payloads** on public models use `long? *Id` (and `CategoryNominalCode` for category links). The SDK maps those to URI strings internally. **List filter parameters** and **inbound URL parsing** (for example webhooks) use resource-specific reference types (`ContactReference`, `ProjectReference`, …) that serialise as URI strings. Build reference URIs with `client.Urls` when filters or webhook handlers need them.
- **DEC-003**: `ExpandableField<T>` is an **internal serialisation adapter** for wire fields that may be a URI string or nested object. It is not part of the public consumer API.
- **DEC-004**: `FreeAgentClient` exposes `Environment` and `Urls` so callers build environment-correct URIs without hard-coding hosts.
- **DEC-005**: Do not add per-resource one-off URI converters. Use the generic `ExpandableFieldJsonConverter<T>` and reference-type converters instead.
- **DEC-006**: Service methods may accept `long` identifiers as sugar when the client can construct the URI from `Environment` (for example `contactId` on project list filters).
- **DEC-007**: Public read models expose flat link properties: for example `Contact?`, `long? ContactId`, plus API denormalised display fields (`ContactName`). Writes set `ContactId` (or equivalent `*Id` per link) on the same model — not `*Reference` properties and not `client.Urls` at the call site.
- **DEC-008**: Single-resource GET methods may accept `*GetOptions` with `Include*` flags that perform additional GETs when requested (default `false`). Example: `GetProjectAsync(id, new ProjectGetOptions { IncludeContact = true })`.
- **DEC-009**: List methods do not auto-hydrate linked resources (no N+1). Use denormalised API fields or documented `nested` query parameters when the wire response includes nested objects.

## Consequences

### Positive

- **POS-001**: Callers navigate linked resources without manual URL parsing or host construction.
- **POS-002**: Natural read ergonomics: `project.Contact?.OrganisationName` after optional include, or `project.ContactName` for display without extra calls.
- **POS-003**: `nested=true` list responses populate `Contact` without callers knowing about internal wire adapters.
- **POS-004**: New endpoints follow one repeatable pattern instead of ad hoc `string` links.
- **POS-005**: Type-safe references prevent passing a project URI where a contact URI is expected.

### Negative

- **NEG-001**: Pre-GA breaking changes on link write ergonomics (`BillingContact` / `LinkedProject` reference properties replaced by `ContactId` / `ProjectId` on models).
- **NEG-002**: Additional public types (`*Reference`, `*GetOptions`, `FreeAgentResourceUrls`) increase surface area.
- **NEG-003**: `Company` uses `ResourceId` mapped from its serialised `id` field because the API already exposes company `id` on the wire.

## Alternatives Considered

### Keep `string` URIs with helper parsers only

- **ALT-001**: **Description**: Leave link fields as `string?`; add optional `Id` helpers and `client.Urls` builders.
- **ALT-002**: **Rejection Reason**: Callers still lose nested objects; no compile-time distinction between link target types; helpers proliferate in consumer code.

### Public `ExpandableField<T>` on read models

- **ALT-003**: **Description**: Expose `ExpandableField<Contact>?` on `Project.Contact` with `.Value` for nested data.
- **ALT-004**: **Rejection Reason**: Leaks wire semantics; poor G2 ergonomics; callers must learn adapter shape.

### Lazy-loading navigation properties

- **ALT-005**: **Description**: `project.Contact` triggers `GetContactAsync` on access.
- **ALT-006**: **Rejection Reason**: Out of scope per [SCOPE.md](../SCOPE.md); hides network I/O; complicates testing and cancellation.

### Always hydrate linked resources on single GET

- **ALT-007**: **Description**: Every `GetProjectAsync` fetches the billing contact automatically.
- **ALT-008**: **Rejection Reason**: Doubles HTTP cost for callers who only need `ContactName` or `ContactId`; explicit `Include*` flags preserve choice.

## Implementation Notes

- **IMP-001**: Add shared primitives under `FreeAgent.Client` and `Models/Shared/`; retrofit Projects as the reference implementation.
- **IMP-002**: Document the pattern in [docs/explanation/linked-resources.md](../docs/explanation/linked-resources.md) for Invoices, Tasks, and Timeslips.
- **IMP-003**: Update [CONVENTIONS.md](../CONVENTIONS.md), [plan/API_TYPE_MAPPING_POLICY.md](../plan/API_TYPE_MAPPING_POLICY.md), and the `implement-endpoint` skill in the same change set.
- **IMP-004**: Document breaking changes in [docs/how-to/upgrading.md](../docs/how-to/upgrading.md).

## References

- **REF-001**: [GOALS.md](../GOALS.md) - G2 developer experience
- **REF-002**: [adr-0004-api-type-mapping-contracts.md](adr-0004-api-type-mapping-contracts.md)
- **REF-003**: [adr-0009-public-sdk-surface-and-error-model.md](adr-0009-public-sdk-surface-and-error-model.md)
- **REF-004**: [docs/explanation/api-entity-map.md](../docs/explanation/api-entity-map.md)
