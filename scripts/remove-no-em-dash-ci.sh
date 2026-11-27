#!/usr/bin/env bash
set -euo pipefail

dry_run=false
yes=false

usage() {
  echo "Usage: $0 [--dry-run] [--yes]" >&2
  exit 1
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --dry-run) dry_run=true ;;
    --yes) yes=true ;;
    -h | --help) usage ;;
    *) echo "Unknown option: $1" >&2; usage ;;
  esac
  shift
done

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "${script_dir}/.." && pwd)"

workflow="${ROOT}/.github/workflows/no-em-dash.yml"

if [[ ! -f "${workflow}" ]]; then
  echo "Em dash CI workflow already removed; nothing to do."
  exit 0
fi

if ! $yes && ! $dry_run; then
  read -r -p "Remove ${workflow}? [y/N] " reply
  case "${reply}" in
    [yY] | [yY][eE][sS]) ;;
    *) echo "Aborted."; exit 1 ;;
  esac
fi

if $dry_run; then
  echo "[dry-run] Would delete ${workflow}"
else
  rm -f "${workflow}"
  echo "Removed ${workflow}"
fi