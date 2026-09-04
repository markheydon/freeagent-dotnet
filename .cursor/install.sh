#!/usr/bin/env bash
# Cloud Agent install script for the FreeAgent .NET client.
#
# Installs the .NET SDKs this repository targets (see global.json and
# docs/contributing-setup.md) and restores solution dependencies. Designed to be
# idempotent: re-running it against a prepared machine is a fast no-op.
set -euo pipefail

# Repository root is the parent of this script's .cursor directory.
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# Shared install location that lands on the default PATH via a symlink below, so
# non-interactive shells and tooling (not just login shells) can find dotnet.
DOTNET_INSTALL_DIR="/usr/share/dotnet"

install_sdk_channel() {
    local channel="$1"
    curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    # dotnet-install.sh skips channels that are already present, so this stays
    # idempotent across repeated runs.
    sudo /tmp/dotnet-install.sh --channel "$channel" --install-dir "$DOTNET_INSTALL_DIR"
}

# .NET 10 is the primary target; .NET 8 is required because the SDK package
# multi-targets net8.0. CI installs both channels.
install_sdk_channel "10.0"
install_sdk_channel "8.0"

# Expose dotnet on the global PATH for every shell and tool.
sudo ln -sf "$DOTNET_INSTALL_DIR/dotnet" /usr/local/bin/dotnet

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

dotnet --info

# Restore NuGet dependencies for the whole solution.
dotnet restore "${REPO_ROOT}/FreeAgent.slnx"
