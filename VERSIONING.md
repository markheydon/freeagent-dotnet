# Versioning Policy

**Project:** FreeAgent.NET
**Last updated:** 22 October 2026

---

## Overview

FreeAgent.NET uses [Semantic Versioning 2.0.0](https://semver.org/). Until the MVP scope defined in [SCOPE.md](SCOPE.md) is complete, all published packages carry a prerelease tag.

> ⚠️ **Prerelease packages do not carry stability guarantees.** Public APIs may change between prerelease versions without a major version bump. Pin to an exact version if stability matters to your project at this stage.

---

## Current Stage: Beta

The package is currently published with the `beta` prerelease tag (e.g. `0.1.0-beta.1`).

### What beta means

- MVP scope in [SCOPE.md](SCOPE.md) is feature-complete for the company FreeAgent API (see [API coverage](docs/reference/api-coverage.md)).
- Breaking changes are avoided where possible and will be called out in release notes.
- Suitable for integration testing and feedback; stable `1.0.0` still requires the criteria in *Criteria for First Stable 1.0.0* below.

---

## Stage Progression

| Stage | Tag format | Entry criteria | Exit criteria |
|---|---|---|---|
| **Alpha** | `0.x.y-alpha.n` | Project inception | All MVP scope items in [SCOPE.md](SCOPE.md) are implemented and tested. |
| **Beta** | `0.x.y-beta.n` | MVP feature-complete | No known blocking issues; API surface considered stable for feedback. |
| **Stable** | `1.0.0` and above | Beta sign-off complete | All goals in [GOALS.md](GOALS.md) are met (see *Criteria for 1.0.0* below). |

Minor and patch bumps within a prerelease stage (e.g. `0.1.0-alpha.1` → `0.1.0-alpha.2`) signal bug fixes or incremental additions with no intentional breaking changes.

---

## Criteria for First Stable 1.0.0

The package will be promoted to stable `1.0.0` when all of the following are true:

1. All items in the [In Scope - v1.0](SCOPE.md) section of `SCOPE.md` are implemented and covered by tests.
2. All goals listed in [Goals for v1.0](GOALS.md) (`G1`–`G5`) are met.
3. The SDK is integrated into at least one production application owned by the author (see *Success Looks Like* in `GOALS.md`).
4. No known breaking changes are planned for the immediate future.
5. The public API surface has been reviewed and is considered stable.

---

## Release Workflow

Releases are triggered by pushing a Git tag that matches `v*.*.*` (e.g. `v0.1.0-alpha.1`, `v0.1.0-beta.1`, `v1.0.0`).

See [plan/RELEASE.md](plan/RELEASE.md) for the maintainer runbook.

The release workflow will:

- Build and test the solution.
- Pack the NuGet package using the version embedded in `FreeAgent.Client.csproj`.
- Publish the package to [NuGet.org](https://www.nuget.org/packages/FreeAgent.Client/).
- Create a GitHub Release, automatically marked as a prerelease when the tag contains a prerelease segment.

---

## Compatibility Expectations for Prerelease Consumers

- **Alpha releases**: Expect breaking changes. Treat each alpha as potentially incompatible with the previous one.
- **Beta releases**: Breaking changes are avoided where possible and will be called out explicitly in release notes.
- **Stable releases**: Follow standard Semantic Versioning guarantees - breaking changes only in major versions.

If you depend on a prerelease version of this package, pin to an exact version in your project file:

```xml
<PackageReference Include="FreeAgent.Client" Version="0.1.0-beta.1" />
```

---

## Revision History

| Date | Change | Reason |
|---|---|---|
| 22 October 2026 | Promote to beta (`0.1.0-beta.1`) | Company API coverage complete |
| 30 April 2026 | Initial draft | Adopt prerelease versioning policy |
