---
title: CI sandbox smoke
parent: Contributing
nav_order: 3
---

# CI sandbox smoke

The [Sandbox smoke workflow](https://github.com/markheydon/freeagent-dotnet/actions/workflows/sandbox-smoke.yml) runs on pull requests targeting `main`, pushes to `main`, and a weekly schedule. It includes:

- **File-based app smoke** — [`samples/FileBasedQuickStart/CompanyDeserializeSmoke.cs`](../../samples/FileBasedQuickStart/CompanyDeserializeSmoke.cs) (trim-safe JSON fixture deserialise; no sandbox credentials required).
- **Console Sample Smoke** — `samples/FreeAgent.Client.ConsoleSample` with `--seed-turpinverse --run-all` against the FreeAgent **sandbox** API when path rules and secrets allow.

The [CI workflow](https://github.com/markheydon/freeagent-dotnet/actions/workflows/ci.yml) is limited to formatting, unit tests, and the documentation site build (mocked HTTP in tests only).

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

## Skipping smoke via repository variable

Repository administrators can set an Actions **variable** named `FREEAGENT_BATCH_SKIP_SMOKE` (Settings → Secrets and variables → Actions → Variables):

| Value | Effect |
|-------|--------|
| Unset or `false` | Normal smoke eligibility rules apply (see below). |
| `true` | Forces `run_smoke=false` for every trigger, including weekly schedule and `workflow_dispatch`. |

Use this when you need to land a batch of changes without running the live 25-minute console smoke job (for example sandbox rate limits). Set the variable back to `false` or delete it before relying on CI smoke again.

```bash
gh variable set FREEAGENT_BATCH_SKIP_SMOKE --repo markheydon/freeagent-dotnet --body true
gh variable set FREEAGENT_BATCH_SKIP_SMOKE --repo markheydon/freeagent-dotnet --body false
```

## How the workflow behaves

Sandbox smoke is a **separate workflow** from [CI](https://github.com/markheydon/freeagent-dotnet/actions/workflows/ci.yml). It runs on the same triggers as CI for integration coverage: **pull requests targeting `main`**, **pushes to `main`**, and a weekly **schedule** (Monday 06:00 UTC). Each push to a PR branch starts **CI** and **Sandbox smoke** as distinct checks on the PR, so you can see smoke pass or fail directly in the PR checks list. CI and smoke build independently (smoke publishes its own console sample artefact).

**Fork pull requests** skip the smoke workflow (same-repo PRs only), so upstream sandbox secrets are never exercised for fork heads.

Manual runs use **workflow_dispatch** on the Sandbox smoke workflow.

The **Console Sample Smoke** job is omitted unless the run is smoke-eligible. Eligibility is decided in the `Detect smoke-relevant changes` job (against the CI commit SHA):

| Condition | Smoke job |
|-----------|-----------|
| Changes under `src/`, `samples/`, or `scripts/ci/` (compared to the PR base or previous commit) | Eligible |
| Documentation-only, tests-only, dependency-only (`.csproj` / lockfile), GitHub Actions-only, or other paths outside the list above | **Not** eligible — job skipped |
| Pull request opened by **Dependabot** | **Not** eligible — unit tests still run; live API smoke is deferred to the next successful CI on `main` |
| **workflow_dispatch** on Sandbox smoke | Always eligible (manual) |

When the job runs, credentials behave as before:

| Trigger | Secrets configured? | Result |
|---------|---------------------|--------|
| **Pull request** to `main` (eligible) | No | Console smoke **skipped** (PR stays unblocked without secrets) |
| **Pull request** to `main` (eligible) | Yes | Console smoke **runs** |
| **Push** to `main` (eligible) | No | Workflow **fails** (credentials required when smoke would run) |
| **Push** to `main` (eligible) | Yes | Console smoke **runs** |
| **Push** to `main` (not eligible) | Any | Console smoke job **skipped** |
| **Schedule** (eligible) | No | Workflow **fails** when smoke would run |
| **Schedule** (eligible) | Yes | Console smoke **runs** |
| Manual **workflow_dispatch** on `main` | No | Workflow **fails** when smoke is eligible |
| Manual **workflow_dispatch** on `main` | Yes | Smoke **runs** when eligible |

Before `--run-all`, the job:

1. Refreshes the access token via [`scripts/ci/refresh-freeagent-access-token.sh`](../../scripts/ci/refresh-freeagent-access-token.sh)
2. Registers the access token with GitHub Actions log masking (`::add-mask::`) before passing it to the smoke step — repository secrets are masked automatically, but step outputs are not
3. Seeds Turpinverse canon data with `--seed-turpinverse`, then runs the console sample smoke suite with `--run-all` using the fresh access token

Seeding upserts contacts, projects, tasks, sales documents, notes, and timeslips by stable keys. It is idempotent but does not remove stale sandbox records — delete unwanted probe data manually when refreshing a CI sandbox.

Turpinverse canon is pinned by commit SHA in the console sample `appsettings.json` (`Turpinverse:CanonRef`) so CI does not drift when upstream `main` changes. Bump the SHA deliberately when adopting new canon fixtures.

The seed step adds many API calls before smoke (list + upsert per canon item, with 600 ms pacing). The job timeout is 25 minutes, which should remain sufficient; watch the first CI run after enabling seed.

Canon `Paid` invoices are seeded as sent/open only; recording bank payments is out of scope for the Turpinverse fixtures.

If FreeAgent returns a new refresh token during that exchange, update the `FREEAGENT_REFRESH_TOKEN` repository secret manually (GitHub Actions cannot rotate secrets with the default `GITHUB_TOKEN`).

## When smoke breaks

Common causes:

- Refresh token revoked (disconnect the app in FreeAgent, or security revocation)
- OAuth client secret rotated without updating GitHub secrets
- Sandbox rate limits (the job runs after each successful CI on `main` when smoke-eligible, plus weekly via the CI schedule, when secrets exist)

Re-run bootstrap (`--bootstrap-refresh-token`) and update the three repository secrets.

## Related

- [Smoke and test limitations](smoke-and-test-limitations.md) — blocked coverage, Turpinverse data gaps, and menu-only examples
- [Console sample README](../../samples/FreeAgent.Client.ConsoleSample/README.md) — interactive examples and `--run-all`
- [#126](https://github.com/markheydon/freeagent-dotnet/issues/126) — post-v1.0 API freshness policy
- [#134](https://github.com/markheydon/freeagent-dotnet/issues/134) — linked-resource PUT sandbox verification
