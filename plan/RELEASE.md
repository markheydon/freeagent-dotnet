# Release runbook

Maintainer guide for publishing `FreeAgent.Client` to NuGet.org. Consumer-facing versioning policy is in [VERSIONING.md](../VERSIONING.md).

## Prerequisites

- [ ] NuGet.org **Trusted Publishing** policy configured for `markheydon/freeagent-dotnet` and workflow `release.yml`, scoped to `FreeAgent.Client`
- [ ] `NUGET_USER` repository secret set to your nuget.org username (profile name, not email)
- [ ] `<Version>` in [FreeAgent.Client.csproj](../src/FreeAgent.Client/FreeAgent.Client.csproj) matches the intended tag
- [ ] `main` is green on CI
- [ ] Contacts and Company resources complete for the release scope

## Publish steps

1. **Bump version** (if needed) in `src/FreeAgent.Client/FreeAgent.Client.csproj`:

   ```xml
   <Version>0.1.0-alpha.1</Version>
   ```

2. **Merge** release changes to `main`.

3. **Create and push a tag** matching the csproj version (with `v` prefix):

   ```bash
   git tag v0.1.0-alpha.1
   git push origin v0.1.0-alpha.1
   ```

4. **Monitor** the [Release workflow](../.github/workflows/release.yml):
   - Validates tag matches csproj version
   - Builds and tests `net8.0` and `net10.0`
   - Exchanges a GitHub OIDC token for a short-lived NuGet API key (Trusted Publishing)
   - Packs and pushes to NuGet (`--skip-duplicate` for safe re-runs)
   - Creates a GitHub Release (prerelease when tag contains `-alpha`, `-beta`, etc.)

5. **Verify**:
   - https://www.nuget.org/packages/FreeAgent.Client/
   - GitHub Releases page shows the new tag
   - README NuGet badge resolves

## Local dry run (optional)

```bash
dotnet pack src/FreeAgent.Client/FreeAgent.Client.csproj -c Release -o ./nupkg
```

Inspect `./nupkg/*.nupkg` without publishing.

## Troubleshooting

| Symptom | Action |
|---------|--------|
| Tag/csproj version mismatch | Workflow fails validation — align `<Version>` and tag |
| `403` on NuGet push | Check Trusted Publishing policy (repo, workflow file, package glob) and `NUGET_USER` secret |
| Package already exists | Expected on re-run; workflow uses `--skip-duplicate` |
| Missing `net8.0` build on CI | Ensure setup-dotnet installs `8.0.x` and `10.0.x` |

## Stable releases

Promote to `1.0.0` only when [VERSIONING.md](../VERSIONING.md) criteria are met. Use tag `v1.0.0` with no prerelease segment; GitHub Release will not be marked prerelease.
