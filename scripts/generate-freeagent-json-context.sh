#!/usr/bin/env bash
set -euo pipefail
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet build "$repo_root/src/FreeAgent.Client/FreeAgent.Client.csproj" -c Release --no-restore 2>/dev/null \
  || dotnet build "$repo_root/src/FreeAgent.Client/FreeAgent.Client.csproj" -c Release
dotnet run --project "$repo_root/scripts/GenerateFreeAgentJsonContext/GenerateFreeAgentJsonContext.csproj" -c Release --no-build
