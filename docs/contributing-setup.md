# Contributor setup

This guide is for developers **working on the SDK repository**, not for consumers installing the NuGet package. For package usage, start with [Getting started](../tutorial/getting-started.md).

## Prerequisites

Install **both** .NET SDKs:

| SDK | Why |
|-----|-----|
| **.NET 10.0** | Primary focus; required for the Blazor sample app |
| **.NET 8.0** | SDK package multi-targets `net8.0` |

CI installs both `8.0.x` and `10.0.x` (see [`.github/workflows/ci.yml`](../.github/workflows/ci.yml)).

### Verify installation

```bash
dotnet --list-sdks
```

You should see 8.x and 10.x entries. [global.json](../global.json) pins SDK `10.0.300` with `rollForward: latestFeature` for the build orchestrator.

### WSL / Ubuntu

Install from [Microsoft’s .NET install instructions](https://learn.microsoft.com/en-us/dotnet/core/install/linux) or use `dotnet-install.sh`. Avoid relying on the distro `dotnet` metapackage alone — it may not include the versions this repository targets.

### Common build failure

If `dotnet build` fails with an error about a missing **.NET 8.0 targeting pack** or SDK, install the .NET 8 SDK. The solution builds `net8.0` and `net10.0` projects even when your default SDK is 10.x.

## Clone and build

```bash
git clone https://github.com/markheydon/freeagent-dotnet.git
cd freeagent-dotnet
dotnet build FreeAgent.slnx
dotnet test
```

Use `dotnet test -f net10.0` or `dotnet test -f net8.0` to test a single target framework.

## Sample app

The Blazor sample requires .NET 10 and FreeAgent OAuth credentials. See [samples/README.md](../samples/README.md).

## Before contributing

Read [CONTRIBUTING.md](../CONTRIBUTING.md), [CONVENTIONS.md](../CONVENTIONS.md), and [AGENTS.md](../AGENTS.md).
