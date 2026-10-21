#!/usr/bin/env bash
set -euo pipefail
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet run --project "$repo_root/scripts/GenerateFreeAgentJsonContext/GenerateFreeAgentJsonContext.csproj" -c Release
