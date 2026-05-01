# Architectural Decision Record: Core Technology Stack Selection

**ADR-0001**  
**Date:** 2026-04-29

---

## Status

**Accepted**

## Context
This decision is based on the constraints and assumptions outlined in the project kickoff specification:

- **Section 8: Constraints and Assumptions**
  - The SDK must be modern, idiomatic, and maintainable for .NET developers.
  - The project is solo-maintained, with a focus on maintainability and low operational overhead.
  - No database or persistent storage is required; the SDK is an API client only.
  - The SDK must be easily consumable by other .NET projects and distributed via NuGet.
  - The codebase should be testable and support automated testing.
  - The project should use open-source, widely adopted technologies to maximize community familiarity and support.
  - The SDK should be compatible with long-term support (LTS) versions of .NET.
  - The project should avoid unnecessary dependencies and complexity.

- **Section 9: Dependencies**
  - .NET 10.0 (LTS).
  - xUnit for testing.
  - Microsoft.Extensions.Http for HTTP client abstraction.
  - No database or ORM.
  - No UI or CLI dependencies.
  - No business-rule abstraction libraries.
  - No OAuth UI/flow libraries (programmatic token exchange only).
  - NuGet for package distribution.
  - GitHub for source control and collaboration.

## Decision
The following technology stack is adopted for the initial and ongoing development of FreeAgent.NET:

- **.NET 10.0 (LTS):** Primary runtime and language platform.
- **xUnit:** Unit testing framework.
- **Microsoft.Extensions.Http:** HTTP client abstraction.
- **No database:** API client only, no persistence layer.
- **SDK-oriented architecture:** Client + services + models + HTTP/auth support components, optimised for NuGet consumption rather than app-layer architecture.
- **Solo-maintained:** Project is maintained by a single developer.
- **NuGet:** Package distribution.
- **GitHub:** Source control and collaboration.

### Architecture clarification
FreeAgent.NET is a reusable library package, not an application host.
As such, conventions should prioritise a stable public API, clear service abstractions, strongly typed models, predictable exception behaviour, and backward-compatible package evolution.

Patterns like CQRS, repositories, and API controller boundaries are intentionally not required unless a future ADR introduces a specific need.

## Alternatives Considered
For each technology choice, common alternatives are listed below.

Rejection reasons for each alternative should be filled in by the developer where they are not yet documented.

- **.NET Version:**
  - .NET 8.0, .NET 6.0, .NET Framework, Mono.
- **Test Framework:**
  - NUnit, MSTest, xBehave, SpecFlow.
- **HTTP Library:**
  - RestSharp, Refit, Flurl.Http, HttpClientFactory alternatives.
- **Architecture:**
  - Layered Architecture, Hexagonal/Ports & Adapters, Feature Folders, Monolithic.
- **Package Manager:**
  - Paket, manual DLL distribution.
- **Version Control System:**
  - GitLab, Bitbucket, Azure DevOps, SVN, Mercurial.

## Stakeholders
- Mark Heydon (project owner).

## Consequences
- The project will be accessible to the .NET community, maintainable by a solo developer, and easy to consume via NuGet.
- The technology stack is intentionally minimal and modern, reducing maintenance burden and maximizing compatibility.
- Future changes to the stack should be documented in subsequent ADRs.

---
