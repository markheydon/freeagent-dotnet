---
title: Home
layout: default
nav_order: 1
permalink: /
---

# FreeAgent.NET documentation

Documentation in this folder is for **developers using the `FreeAgent.Client` NuGet package** — not for internal project planning.

Published site: [markheydon.me.uk/freeagent-dotnet](https://markheydon.me.uk/freeagent-dotnet/)

Internal engineering policy lives in the repository root (`GOALS.md`, `SCOPE.md`, `CONVENTIONS.md`) and in [`plan/`](../plan/). Architectural decisions are in [`adr/`](../adr/).

## Documentation map

| Document | Type | Audience |
|----------|------|----------|
| [Getting started](tutorial/getting-started.md) | Tutorial | New SDK consumers |
| [Pagination](how-to/pagination.md) | How-to | Consumers listing resources |
| [Error handling](how-to/error-handling.md) | How-to | Consumers handling API failures |
| [Token refresh](how-to/token-refresh.md) | How-to | Consumers managing OAuth tokens |
| [Upgrading](how-to/upgrading.md) | How-to | Consumers on prerelease versions |
| [API coverage](reference/api-coverage.md) | Reference | Index of implemented resources and SDK reference pages |
| [API entity map](explanation/api-entity-map.md) | Explanation | How FreeAgent resources link; SDK sequencing guide |
| [Linked resources](explanation/linked-resources.md) | Explanation | SDK pattern for URI links, hydration, and writes |
| [Prerelease and OAuth scope](explanation/prerelease-and-oauth.md) | Explanation | Versioning and OAuth boundaries |
| [Contributor setup](contributing-setup.md) | How-to | Contributors cloning this repository |
| [Sample probe pages](contributing/sample-probe-pages.md) | How-to | Building wire-to-model sample pages |

## Writing a page

Add YAML front matter to every new page:

```yaml
---
title: Page title
parent: Section name   # Tutorial, How-to, Reference, Explanation, or Contributing
nav_order: 10
---
```

Use UK English spelling. Keep relative `.md` links between pages. Reference pages belong under `docs/reference/` and should be linked from [API coverage](reference/api-coverage.md).

## Local preview

No local Ruby or Jekyll install is required. From the repository root, use Docker or Podman:

```bash
./scripts/invoke-docs-site.sh serve     # live reload at http://127.0.0.1:4000/
./scripts/invoke-docs-site.sh preview   # static build + nginx at http://127.0.0.1:8080/
./scripts/invoke-docs-site.sh build     # production-parity output to docs/_site/
```

On WSL, use `127.0.0.1` rather than `localhost` (Podman binds IPv4 only).

On Windows (PowerShell):

```powershell
.\scripts\Invoke-DocsSite.ps1 serve
.\scripts\Invoke-DocsSite.ps1 preview
.\scripts\Invoke-DocsSite.ps1 build
```

The first run downloads the `jekyll/jekyll` image and installs gems into `docs/vendor/` (gitignored). If the port is in use or Ctrl+C does not stop the server, run `./scripts/invoke-docs-site.sh stop`.

## Related links

- [README](../README.md) — repository overview and quick start
- [VERSIONING.md](../VERSIONING.md) — prerelease policy
- [SUPPORT.md](../SUPPORT.md) — help and issue routing
- [FreeAgent API documentation](https://dev.freeagent.com/docs)
