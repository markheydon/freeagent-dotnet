# Project Kickoff Spec

**Project Name:** FreeAgent.NET  
**Owner:** Mark Heydon  
**Date:** 29 April 2026  
**Status:** Draft

---

## 1. Summary

FreeAgent.NET is an open‑source, modern .NET SDK for the FreeAgent REST API.  

It aims to provide a first‑class developer experience comparable to Stripe.NET, with strongly typed models, clean service abstractions, automatic pagination, and predictable error handling.  

The package will be published to NuGet for use both by the author's own applications and the wider .NET community.

## 2. Problem Statement

The FreeAgent API is powerful but lacks an official high‑quality, modern, and actively maintained .NET client library.

.NET developers integrating with FreeAgent are forced to write bespoke HTTP wrappers or rely on inconsistent third‑party solutions.

This project removes duplicated effort, improves reliability, and provides a stable shared foundation for multiple FreeAgent‑integrated applications.

## 3. Target Users

- UK‑based .NET developers building internal tools or SaaS products that integrate with FreeAgent.
- Developers automating accounting, invoicing, or reporting workflows for FreeAgent clients.
- The project owner, using FreeAgent.NET as a shared dependency across multiple commercial and internal projects.

## 4. Goals

What must this project achieve?

- **G1:** Provide a clean, strongly typed .NET SDK for the FreeAgent REST API.
- **G2:** Deliver a Stripe.NET‑quality developer experience (discoverable, consistent, pleasant to use).
- **G3:** Support automatic pagination without consumers managing pages explicitly.
- **G4:** Be safe, stable, and boring to depend on in production systems.
- **G5:** Enable external contributions without destabilising the public API.

## 5. Non-Goals

What is explicitly out of scope for this version?

- **NG1:** No UI, CLI, or end‑user tooling.
- **NG2:** No business‑rule abstraction (e.g. VAT logic, accounting rules, reporting opinions).
- **NG3:** No handling of OAuth authorisation flows beyond obtaining and refreshing OAuth tokens programmatically (i.e., no UI or browser-based flows; only direct token exchange and refresh are supported).
- **NG4:** No attempt to reshape FreeAgent concepts into alternative domain models.
- **NG5:** No support for undocumented or experimental FreeAgent API endpoints.

## 6. MVP Scope

What will be in the first version? Keep this ruthlessly small.

- Authenticated HTTP client for the FreeAgent API.
- Stripe‑style service classes per core resource (e.g. Contacts, Invoices).
- Strongly typed request and response models.
- Fully abstracted pagination.
- Typed exception hierarchy for API and transport errors.
- NuGet packaging and publishing pipeline.
- Basic documentation and usage examples.

## 7. Key User Journeys

In this project, "user" means a developer consuming this NuGet package in their own codebase.

These journeys describe consumer experience and documentation priorities, not implementation backlog items for this repository.

They should inform developer-facing/public-facing documentation and examples, rather than be translated directly into GitHub Issues for this codebase.

1. Install FreeAgent.NET from NuGet and initialise a client using an OAuth access token.
2. Retrieve a paginated list of FreeAgent resources using a simple async API.
3. Handle API errors via predictable, strongly typed exceptions.
4. Upgrade the SDK without breaking existing integrations.

## 8. Constraints and Assumptions

| Type | Detail |
|---|---|
| Technical constraint | Must conform to the FreeAgent API design and limitations. |
| Technical constraint | Target .NET 10 LTS. |
| Budget/time constraint | Solo‑maintained alongside paid client work. |
| Assumption | FreeAgent API remains broadly stable over time. |
| Assumption | Initial users are comfortable reading documentation and examples. |

## 9. Dependencies

External systems, APIs, tools, or decisions this project depends on.

- FreeAgent REST API.
- Quality and stability of FreeAgent API documentation.
- NuGet.org for package distribution.
- GitHub for source control, issues, and collaboration.

## 10. Risks / Open Questions

- Risk of FreeAgent API breaking changes with limited notice.
- Risk of over‑engineering abstractions before real‑world usage feedback.
- Long‑term maintenance expectations once publicly published.
- Handling API inconsistencies or undocumented behaviour cleanly.

## 11. Success Criteria

Specific, measurable. These feed into GOALS.md.

- The SDK is used by at least one production application owned by the author.
- Pagination and error handling work without consumer workarounds.
- Package is published to NuGet with documentation and examples.
- No breaking public API changes within the first minor version.

## 12. Kill Criteria

I'll stop if...

- FreeAgent releases an official .NET SDK that supersedes this work.
- Maintenance cost outweighs personal or commercial value.
- The FreeAgent API proves too unstable to wrap cleanly.
- The SDK is not used in any real project after initial integration.

## 13. Next Steps

- [ ] Create repo with standard structure
- [ ] Populate GOALS.md from Section 4 + 11
- [ ] Populate SCOPE.md from Section 5 + 6
- [ ] Create GitHub Issues for each item in Section 6 (MVP Scope)
- [ ] Create/expand developer-facing docs and examples that cover each journey in Section 7
- [ ] Write `.github/copilot-instructions.md`
- [ ] Write first ADR: technology choices
