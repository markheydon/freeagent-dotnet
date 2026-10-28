#!/usr/bin/env bash
# Ensures the ASP.NET Core HTTPS development certificate exists and is trusted
# inside the Dev Container (Kestrel + OpenSSL-backed HttpClient).
set -euo pipefail

dotnet dev-certs https --trust
dotnet dev-certs https --check --trust
