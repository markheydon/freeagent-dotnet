---
title: "ADR-0013: Post-EOL net8.0 Window and Forward SDK Validation"
status: "Accepted"
date: "2026-11-27"
authors: "Mark Heydon (Project Owner)"
tags: ["architecture", "decision", "target-frameworks", "compatibility", "ci"]
supersedes: ""
superseded_by: ""
---

# ADR-0013: Post-EOL net8.0 Window and Forward SDK Validation

## Status

**Accepted**

## Context

[ADR-0007](adr-0007-dotnet-8-and-10-targeting-strategy.md) established dual targeting for `net8.0` and `net10.0`. .NET 8 **standard support ended** in November 2026, but published packages and consumers may still depend on `net8.0` assets for a transition period.

The repository also needs early signal when a **future .NET SDK** (for example .NET 11) would break existing target frameworks **before** adding a `net11.0` asset to the NuGet package.

Development environments (Dev Container) should stay lean (.NET 10 SDK image) while CI and release retain full multi-target validation.

## Decision

- **DEC-001**: Continue publishing and CI-testing `net8.0` alongside `net10.0` after .NET 8 EOL, with a **planned review by November 2027** to remove or extend `net8.0`.
- **DEC-002**: Treat **.NET 10** as the primary engineering and Dev Container SDK; do not require .NET 8 SDK in the Dev Container for routine development (full `net8.0` **test** runs remain a CI/release gate).
- **DEC-003**: CI installs **both** `8.0.x` and `10.0.x` SDKs/runtimes for verify, build, test, and release workflows.
- **DEC-004**: Add a **forward SDK 11** CI job that installs `11.0.x` (when available on the runner) and runs the same build and test steps as main CI, without adding `net11.0` to `TargetFrameworks` yet.
- **DEC-005**: Record consumer-facing wording in [README.md](../README.md) and [VERSIONING.md](../VERSIONING.md): `net10.0` recommended; `net8.0` still shipped and validated during the transition window; no `net11.0` asset until a follow-on decision.

## Consequences

### Positive

- Consumers on `net8.0` keep a predictable sunset window instead of an abrupt drop at EOL.
- Forward SDK job surfaces breaking tool-chain changes early.
- Dev Container setup aligns with the api-client template while CI retains rigour.

### Negative

- Continued dual-target maintenance cost until the November 2027 review.
- Forward SDK job may be flaky while .NET 11 previews roll out (`continue-on-error` until stable).

## Implementation Notes

- Dev Container: `mcr.microsoft.com/devcontainers/dotnet:10.0`; document `dotnet test -f net10.0` for in-container loops.
- [`.github/workflows/ci.yml`](../.github/workflows/ci.yml): `forward-sdk-11` job; flip from `continue-on-error` when green on `main`.
- [plan/RELEASE.md](../plan/RELEASE.md): note dual SDK install and forward job gating.

## References

- [ADR-0007](adr-0007-dotnet-8-and-10-targeting-strategy.md)
- [docs/contributing-setup.md](../docs/contributing-setup.md)
- [dotnet-api-client-template Dev Container](https://github.com/markheydon/dotnet-api-client-template)