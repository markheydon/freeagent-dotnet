# Scope

**Project:** FreeAgent.NET
**Last updated:** 3 May 2026

---

## In Scope - v1.0

The following are committed for the first release:

- Authenticated HTTP client for the FreeAgent API.
- Stripe‑style service classes per core resource (e.g. Contacts, Invoices).
- Strongly typed request and response models.
- Fully abstracted pagination.
- Typed exception hierarchy for API and transport errors.
- NuGet packaging and publishing pipeline.
- Basic documentation and usage examples.
- Protocol-level OAuth helpers: authorisation URL construction, code exchange, and token refresh as SDK utilities (excluding UI/browser orchestration and app-level flow management).

---

## Out of Scope - v1.0

The following have been explicitly considered and deferred or rejected for v1.0. Do not implement these without a deliberate decision to change scope.

- No UI, CLI, or end-user tooling.
- No business-rule abstraction (e.g. VAT logic, accounting rules, reporting opinions).
- No SDK-managed UI/browser journeys, callback endpoint hosting, app/session/token persistence, or orchestration of end-user OAuth flows. Protocol-level helpers (authorisation URL construction, code exchange, token refresh) are in scope; UI and flow management are not.
- No attempt to reshape FreeAgent concepts into alternative domain models.
- No support for undocumented or experimental FreeAgent API endpoints.

---

## Possible v2 Candidates
(Deferred, not rejected)

// Add v2 candidates here as they are identified.
