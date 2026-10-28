# Dev Container

.NET 10 (with 8.0 side-by-side), GitHub CLI, and Cursor CLI on Ubuntu 24.04.

## HTTPS development certificates

The Blazor sample runs at `https://localhost:5001`. Inside the container, `postCreateCommand` runs [setup-dev-certs.sh](setup-dev-certs.sh) (`dotnet dev-certs https --trust`). `remoteEnv` sets `SSL_CERT_DIR` so OpenSSL-backed `HttpClient` also trusts `~/.aspnet/dev-certs/trust`.

Your **Windows** browser does not use the container trust store. After the container is ready, export the public cert and import it on Windows:

```bash
./scripts/export-dev-cert-for-windows-host.sh
```

Run the `$crt` / `Import-Certificate` lines the script prints in **Windows PowerShell**, then fully quit and reopen the browser.

The exported file is `.local/aspnetcore-localhost.crt` (gitignored).
