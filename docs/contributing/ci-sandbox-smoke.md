---
title: CI sandbox smoke
parent: Contributing
nav_order: 3
---

# CI sandbox smoke

The **Console Sample Smoke** job in [`.github/workflows/ci.yml`](../../.github/workflows/ci.yml) runs `samples/FreeAgent.Client.ConsoleSample` with `--seed-turpinverse --run-all` against the FreeAgent **sandbox** API.

Default CI (`verify`, `build-and-test`) uses mocked HTTP handlers only. Live sandbox smoke is an additional signal that the SDK still works against a real OAuth-protected API.

## Data mutations

`--run-all` is a **live integration harness**: it creates, updates, and deletes probe data in the connected sandbox account (invoices, contacts, notes, and similar). Use a dedicated CI sandbox identity (recommended below), not a production account.

The console sample refuses `--run-all` and `--seed-turpinverse` when the resolved environment is not sandbox. See [#136](https://github.com/markheydon/freeagent-dotnet/issues/136) for the write-guard contract.

## What you need

| Item | Purpose |
|------|---------|
| Sandbox OAuth app | Client ID and secret from [dev.sandbox.freeagent.com](https://dev.sandbox.freeagent.com) |
| Sandbox user account | Approves OAuth once when you obtain the refresh token ([sandbox signup](https://signup.sandbox.freeagent.com/signup)) |
| Refresh token | Long-lived credential stored as `FREEAGENT_REFRESH_TOKEN` in GitHub Actions |

These are **not** your FreeAgent login password. CI never opens a browser; it reuses a refresh token you obtained earlier.

### Recommended: dedicated CI identity

Create a separate sandbox OAuth application (for example `freeagent-dotnet-ci`) and approve it with a dedicated sandbox user. That keeps CI credentials isolated from day-to-day development.

Register this redirect URI on the OAuth app:

```
http://127.0.0.1:8765/callback
```

## One-time bootstrap (local)

1. Configure client ID and secret (shared with the Blazor sample):

   ```bash
   cd samples/FreeAgent.Client.ConsoleSample
   dotnet user-secrets set "FreeAgent:ClientId"     "<sandbox-client-id>"
   dotnet user-secrets set "FreeAgent:ClientSecret" "<sandbox-client-secret>"
   ```

2. Obtain and print a refresh token:

   ```bash
   dotnet run -- --bootstrap-refresh-token
   ```

   Complete OAuth in the browser. On WSL, copy the authorisation URL manually and paste the full redirect URL back into the terminal when prompted.

3. Verify locally before configuring GitHub:

   ```bash
   export FREEAGENT_REFRESH_TOKEN="<refresh-token-from-step-2>"
   dotnet run -- --seed-turpinverse --run-all
   ```

## Configure GitHub repository secrets

Set three repository secrets (Settings → Secrets and variables → Actions):

| Secret | Value |
|--------|--------|
| `FREEAGENT_CLIENT_ID` | Sandbox OAuth identifier |
| `FREEAGENT_CLIENT_SECRET` | Sandbox OAuth secret |
| `FREEAGENT_REFRESH_TOKEN` | Refresh token from bootstrap |

Or via the GitHub CLI:

```bash
gh secret set FREEAGENT_CLIENT_ID --repo markheydon/freeagent-dotnet
gh secret set FREEAGENT_CLIENT_SECRET --repo markheydon/freeagent-dotnet
gh secret set FREEAGENT_REFRESH_TOKEN --repo markheydon/freeagent-dotnet
```

## How the workflow behaves

| Trigger | Secrets configured? | Result |
|---------|---------------------|--------|
| Pull request | No | Smoke **skipped** (forks and draft work stay unblocked) |
| Pull request | Yes | Smoke **runs** (secrets are available to workflows from this repository) |
| Push to `main` | No | Workflow **fails** (credentials required on main) |
| Push to `main` | Yes | Smoke **runs** |
| Weekly schedule / manual dispatch on `main` | No | Workflow **fails** |
| Weekly schedule / manual dispatch on `main` | Yes | Smoke **runs** |

Before `--run-all`, the job:

1. Refreshes the access token via [`scripts/ci/refresh-freeagent-access-token.sh`](../../scripts/ci/refresh-freeagent-access-token.sh)
2. Registers the access token with GitHub Actions log masking (`::add-mask::`) before passing it to the smoke step — repository secrets are masked automatically, but step outputs are not
3. Seeds Turpinverse canon data with `--seed-turpinverse`, then runs the console sample smoke suite with `--run-all` using the fresh access token

Seeding upserts contacts, projects, tasks, sales documents, notes, and timeslips by stable keys. It is idempotent but does not remove stale sandbox records — delete unwanted probe data manually when refreshing a CI sandbox.

The seed step adds many API calls before smoke (list + upsert per canon item, with 600 ms pacing). The job timeout is 25 minutes, which should remain sufficient; watch the first CI run after enabling seed.

If FreeAgent returns a new refresh token during that exchange, update the `FREEAGENT_REFRESH_TOKEN` repository secret manually (GitHub Actions cannot rotate secrets with the default `GITHUB_TOKEN`).

## When smoke breaks

Common causes:

- Refresh token revoked (disconnect the app in FreeAgent, or security revocation)
- OAuth client secret rotated without updating GitHub secrets
- Sandbox rate limits (the job runs weekly plus on each `main` push when secrets exist)

Re-run bootstrap (`--bootstrap-refresh-token`) and update the three repository secrets.

## Related

- [Console sample README](../../samples/FreeAgent.Client.ConsoleSample/README.md) — interactive examples and `--run-all`
- [#126](https://github.com/markheydon/freeagent-dotnet/issues/126) — post-v1.0 API freshness policy
- [#134](https://github.com/markheydon/freeagent-dotnet/issues/134) — linked-resource PUT sandbox verification
