# Dev Container

.NET 10 on the official `mcr.microsoft.com/devcontainers/dotnet:10.0` image, with the [dotnet devcontainer feature](https://github.com/devcontainers/features/tree/main/src/dotnet) installing **.NET 8.0** side-by-side so `dotnet test` can run both `net8.0` and `net10.0`. Also includes GitHub CLI, [dev-agent-tools](https://github.com/markheydon/devcontainer-features) (`rg`, `fd`, `jq`, and similar), and Cursor CLI.

## Target frameworks

```bash
dotnet build FreeAgent.slnx -c Release -warnaserror
dotnet test FreeAgent.slnx -c Release
```

CI and release use the same dual-target validation. See [docs/contributing-setup.md](../docs/contributing-setup.md).

## HTTPS development certificates

The Blazor sample runs at `https://localhost:5001`.

On **first create**, `onCreateCommand` runs [setup-dotnet-dev-cert.sh](setup-dotnet-dev-cert.sh) (system CA trust) and installs `libnss3-tools`. On **each start**, `postStartCommand` runs `dotnet dev-certs https --trust`. `remoteEnv` sets `SSL_CERT_DIR` so OpenSSL-backed `HttpClient` also trusts `~/.aspnet/dev-certs/trust`.

Your **Windows** browser does not use the container trust store. After the container is ready, export the public cert and import it on Windows:

```bash
./scripts/export-dev-cert-for-windows-host.sh
```

Run the `$crt` / `Import-Certificate` lines the script prints in **Windows PowerShell**, then fully quit and reopen the browser.

The exported file is `.local/aspnetcore-localhost.crt` (gitignored).

## Rebuild after config changes

After edits to [devcontainer.json](devcontainer.json), run **Dev Containers: Rebuild Container** so the .NET 8 runtime is installed.

## Feature lock file

The first successful Dev Container build may refresh [devcontainer-lock.json](devcontainer-lock.json). Commit updates so Dependabot can propose `devcontainers` dependency PRs.