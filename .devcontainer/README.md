# Dev Container

.NET 10 SDK on the official `mcr.microsoft.com/devcontainers/dotnet:10.0` image, plus GitHub CLI, [dev-agent-tools](https://github.com/markheydon/devcontainer-features) (`rg`, `fd`, `jq`, and similar), and Cursor CLI.

## Target frameworks

The container ships the **.NET 10 SDK only**. You can **build** projects that target `net8.0` and `net10.0`. Running **`dotnet test` for the full solution** may fail without the **.NET 8 runtime** installed locally.

For day-to-day work in the container, prefer:

```bash
dotnet build FreeAgent.slnx -c Release -warnaserror
dotnet test FreeAgent.slnx -c Release -f net10.0
```

CI and release workflows validate **both** `net8.0` and `net10.0` on every change. See [docs/contributing-setup.md](../docs/contributing-setup.md).

## HTTPS development certificates

The Blazor sample runs at `https://localhost:5001`.

On **first create**, `onCreateCommand` runs [setup-dotnet-dev-cert.sh](setup-dotnet-dev-cert.sh) (system CA trust) and installs `libnss3-tools`. On **each start**, `postStartCommand` runs `dotnet dev-certs https --trust`. `remoteEnv` sets `SSL_CERT_DIR` so OpenSSL-backed `HttpClient` also trusts `~/.aspnet/dev-certs/trust`.

Your **Windows** browser does not use the container trust store. After the container is ready, export the public cert and import it on Windows:

```bash
./scripts/export-dev-cert-for-windows-host.sh
```

Run the `$crt` / `Import-Certificate` lines the script prints in **Windows PowerShell**, then fully quit and reopen the browser.

The exported file is `.local/aspnetcore-localhost.crt` (gitignored).

## Feature lock file

The first successful Dev Container build may refresh [devcontainer-lock.json](devcontainer-lock.json). Commit updates so Dependabot can propose `devcontainers` dependency PRs.