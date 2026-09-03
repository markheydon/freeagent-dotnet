# FreeAgent.NET documentation

Documentation in this folder is for **developers using the `FreeAgent.Client` NuGet package** — not for internal project planning.

Internal engineering policy lives in the repository root (`GOALS.md`, `SCOPE.md`, `CONVENTIONS.md`) and in [`plan/`](../plan/). Architectural decisions are in [`adr/`](../adr/).

## Documentation map

| Document | Type | Audience |
|----------|------|----------|
| [Getting started](tutorial/getting-started.md) | Tutorial | New SDK consumers |
| [Pagination](how-to/pagination.md) | How-to | Consumers listing resources |
| [Error handling](how-to/error-handling.md) | How-to | Consumers handling API failures |
| [Token refresh](how-to/token-refresh.md) | How-to | Consumers managing OAuth tokens |
| [Upgrading](how-to/upgrading.md) | How-to | Consumers on prerelease versions |
| [API coverage](reference/api-coverage.md) | Reference | What the SDK implements today |
| [Prerelease and OAuth scope](explanation/prerelease-and-oauth.md) | Explanation | Versioning and OAuth boundaries |
| [Contributor setup](contributing-setup.md) | How-to | Contributors cloning this repository |
| [Sample probe pages](contributing/sample-probe-pages.md) | How-to | Building wire-to-model sample pages |

## Related links

- [README](../README.md) — repository overview and quick start
- [VERSIONING.md](../VERSIONING.md) — prerelease policy
- [SUPPORT.md](../SUPPORT.md) — help and issue routing
- [FreeAgent API documentation](https://dev.freeagent.com/docs)
