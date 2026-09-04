# FreeAgent.Client Blazor sample

Blazor Server workbench for exercising implemented SDK endpoints interactively. Use it as a living reference when contributing probe pages, or as a starting point for your own Blazor integration.

For OAuth setup, redirect URI registration, and running the app, see the [samples README](../README.md#blazor-sample-full-workbench).

---

## SDK source: local project vs NuGet package

This sample is written like a **consumer app** (it imports `FreeAgent.Client` and calls the public API). In the repository it defaults to a **project reference** so contributors can run it against in-progress SDK changes without publishing.

To **smoke-test the published NuGet package** instead, pass `UseLocalFreeAgentClient=false`:

```bash
# From the repository root
dotnet run --project samples/FreeAgent.Client.BlazorSample -p:UseLocalFreeAgentClient=false

# Or from this directory
dotnet run -p:UseLocalFreeAgentClient=false
```

To switch back to the local SDK (default):

```bash
dotnet run
# or explicitly:
dotnet run -p:UseLocalFreeAgentClient=true
```

The pinned NuGet version is defined in [`Directory.Packages.props`](../../Directory.Packages.props). Bump it when testing a newer published release.

### Copying this sample outside the repository

If you copy the project files into your own solution, remove the `UseLocalFreeAgentClient` property and project-reference block, then add the package:

```bash
dotnet add package FreeAgent.Client --version 0.1.0-alpha.1
```

---

## Related documentation

- [Samples overview](../README.md)
- [Sample probe pages](../../docs/contributing/sample-probe-pages.md)
- [Console sample](../FreeAgent.Client.ConsoleSample/) — minimal OAuth + contacts listing
