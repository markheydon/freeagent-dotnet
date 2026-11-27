---
title: Contributor setup
parent: Contributing
nav_order: 1
---

# Contributor setup

This guide is for developers **working on the SDK repository**, not for consumers installing the NuGet package. For package usage, start with [Getting started](../tutorial/getting-started.md).

## Dev Container

Open the repo in a [Dev Container](../.devcontainer/devcontainer.json) (VS Code or GitHub Codespaces). The base image is **.NET 10**; the **dotnet feature** also installs **.NET 8.0** so full-solution `dotnet test` works.

After the container is created (or **rebuilt** after devcontainer changes):

```bash
dotnet build FreeAgent.slnx -c Release -warnaserror
dotnet test FreeAgent.slnx -c Release
```

HTTPS notes for the Blazor sample: [.devcontainer/README.md](../.devcontainer/README.md) and [samples/README.md](../samples/README.md).

## Prerequisites (bare-metal)

For **full local multi-target parity** (optional outside the Dev Container), install **both** SDKs:

| SDK | Why |
|-----|-----|
| **.NET 10.0** | Primary focus; required for the Blazor sample app |
| **.NET 8.0** | Run `net8.0` tests; SDK package still multi-targets `net8.0` (see [ADR-0013](../adr/adr-0013-post-eol-net8-and-forward-sdk-validation.md)) |

CI installs **8.0.x** and **10.0.x** (see [global.json](../global.json) and [`.github/workflows/ci.yml`](../.github/workflows/ci.yml)). A **forward SDK 11** job installs **11.0.x** when the runner provides it, sets `DOTNET_ROLL_FORWARD=LatestMajor` so that SDK is actually selected, then runs the same build and test steps without publishing `net11.0` yet. Live sandbox smoke is in [`.github/workflows/sandbox-smoke.yml`](../.github/workflows/sandbox-smoke.yml).

### Verify installation

```bash
dotnet --list-sdks
```

You should see 8.x and 10.x entries. [global.json](../global.json) sets the minimum .NET 10 SDK for the build orchestrator (`rollForward: latestFeature` allows newer 10.0 patches on your machine).

### WSL / Ubuntu

Install from [Microsoft’s .NET install instructions](https://learn.microsoft.com/en-us/dotnet/core/install/linux) or use `dotnet-install.sh`. Avoid relying on the distro `dotnet` metapackage alone - it may not include the versions this repository targets.

### Common build failure

If `dotnet build` fails with an error about a missing **.NET 8.0 targeting pack** or SDK, install the .NET 8 SDK. The solution builds `net8.0` and `net10.0` projects even when your default SDK is 10.x.

## Clone and build

```bash
git clone https://github.com/markheydon/freeagent-dotnet.git
cd freeagent-dotnet
dotnet clean FreeAgent.slnx
dotnet restore FreeAgent.slnx
dotnet build FreeAgent.slnx --no-restore --configuration Release -warnaserror
dotnet test FreeAgent.slnx --no-build --configuration Release
```

Or as a single chain:

```bash
dotnet clean FreeAgent.slnx && \
dotnet restore FreeAgent.slnx && \
dotnet build FreeAgent.slnx --no-restore --configuration Release -warnaserror && \
dotnet test FreeAgent.slnx --no-build --configuration Release
```

Warnings fail the build (`TreatWarningsAsErrors` in [`Directory.Build.props`](../Directory.Build.props); `-warnaserror` on the CLI matches that when you invoke `dotnet build` directly).

Use `dotnet test -f net10.0` or `dotnet test -f net8.0` to test a single target framework.

## Verify formatting

CI runs `dotnet format` with `--verify-no-changes`. Check locally before pushing:

```bash
dotnet format FreeAgent.slnx --verify-no-changes
```

If changes are reported, run `dotnet format FreeAgent.slnx` and commit the result.

## Sample app

The Blazor sample requires .NET 10 and FreeAgent OAuth credentials. See [samples/README.md](../samples/README.md).

## CI sandbox smoke

To enable live API smoke on `main`, configure GitHub Actions secrets using [CI sandbox smoke](contributing/ci-sandbox-smoke.md). The smoke suite performs live writes in sandbox via `--run-all`.

When adding or retrofitting SDK endpoints, build sample probe pages per [Sample probe pages](contributing/sample-probe-pages.md). Use **Company** and **Contacts** as references.

## Documentation site

Consumer docs live in [`docs/`](../docs/) and publish to GitHub Pages. Preview locally with Docker or Podman (no Ruby install):

```bash
./scripts/invoke-docs-site.sh serve
./scripts/invoke-docs-site.sh preview
```

See [docs/README.md](../docs/README.md) for details. When adding SDK reference pages, follow the existing pages under `docs/reference/`.

## Before contributing

Read [CONTRIBUTING.md](../CONTRIBUTING.md), [CONVENTIONS.md](../CONVENTIONS.md), and [AGENTS.md](../AGENTS.md).
