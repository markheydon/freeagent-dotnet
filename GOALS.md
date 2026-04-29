
# Goals

**Project:** FreeAgent.NET
**Owner:** Mark Heydon
**Last updated:** 29 April 2026

---

## Why This Exists
The FreeAgent API is powerful but lacks an official high‑quality, modern, and actively maintained .NET client library. .NET developers integrating with FreeAgent are forced to write bespoke HTTP wrappers or rely on inconsistent third‑party solutions. This project removes duplicated effort, improves reliability, and provides a stable shared foundation for multiple FreeAgent‑integrated applications.

---

## Goals for v1.0

These are the outcomes this project must achieve to be considered successful.

- **G1:** Provide a clean, strongly typed .NET SDK for the FreeAgent REST API.
- **G2:** Deliver a Stripe.NET‑quality developer experience (discoverable, consistent, pleasant to use).
- **G3:** Support automatic pagination without consumers managing pages explicitly.
- **G4:** Be safe, stable, and boring to depend on in production systems.
- **G5:** Enable external contributions without destabilising the public API.

---

## Success Looks Like
- The SDK is used by at least one production application owned by the author.
- Pagination and error handling work without consumer workarounds.
- Package is published to NuGet with documentation and examples.
- No breaking public API changes within the first minor version.

---

## Kill Criteria
I will stop work on this project if:
- FreeAgent releases an official .NET SDK that supersedes this work.
- Maintenance cost outweighs personal or commercial value.
- The FreeAgent API proves too unstable to wrap cleanly.
- The SDK is not used in any real project after initial integration.

---

## What This Is NOT For
(See also: SCOPE.md)
- No UI, CLI, or end‑user tooling.
- No business‑rule abstraction (e.g. VAT logic, accounting rules, reporting opinions).
- No handling of OAuth authorisation flows beyond obtaining and refreshing OAuth tokens programmatically (i.e., no UI or browser-based flows; only direct token exchange and refresh are supported).
- No attempt to reshape FreeAgent concepts into alternative domain models.
- No support for undocumented or experimental FreeAgent API endpoints.

---

## Revision History
| Date | Change | Reason |
|---|---|---|
| 29 April 2026 | Initial draft | Project kickoff |
