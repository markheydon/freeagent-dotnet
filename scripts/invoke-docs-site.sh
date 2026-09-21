#!/usr/bin/env bash
# Build or preview the Jekyll documentation site via Docker/Podman (no local Ruby/Jekyll install required).
# Usage: ./scripts/invoke-docs-site.sh [build|serve|preview|stop] [--runtime docker|podman] [--serve-port N] [--preview-port N]

set -euo pipefail

COMMAND="build"
RUNTIME=""
SERVE_PORT=4000
PREVIEW_PORT=8080
SERVE_CONTAINER_NAME="freeagent-docs-serve"
PREVIEW_CONTAINER_NAME="freeagent-docs-preview"

usage() {
  cat <<'EOF'
Usage: invoke-docs-site.sh [build|serve|preview|stop] [options]

Commands:
  build     Production-parity build to docs/_site/ (default)
  serve     Jekyll dev server with live reload (empty baseurl for localhost)
  preview   Local static preview via nginx (empty baseurl)
  stop      Stop any serve/preview containers left running

Options:
  --runtime RUNTIME   Container runtime: docker or podman (auto-detected if omitted)
  --serve-port PORT   Port for serve command (default: 4000)
  --preview-port PORT Port for preview command (default: 8080)
  -h, --help          Show this help

Examples:
  ./scripts/invoke-docs-site.sh preview
  ./scripts/invoke-docs-site.sh serve --runtime docker
  ./scripts/invoke-docs-site.sh build
  ./scripts/invoke-docs-site.sh stop

Notes:
  serve and preview bind to 127.0.0.1 (use that URL on WSL; localhost may fail).
  build uses baseurl from docs/_config.yml (production parity).
  If Ctrl+C does not stop serve, run: ./scripts/invoke-docs-site.sh stop
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    build|serve|preview|stop)
      COMMAND="$1"
      shift
      ;;
    --runtime)
      RUNTIME="${2:-}"
      shift 2
      ;;
    --serve-port)
      SERVE_PORT="${2:-}"
      shift 2
      ;;
    --preview-port)
      PREVIEW_PORT="${2:-}"
      shift 2
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "error: unknown argument '$1'" >&2
      usage >&2
      exit 1
      ;;
  esac
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
DOCS_DIR="$REPO_ROOT/docs"
SITE_PATH="$DOCS_DIR/_site"
JEKYLL_IMAGE="docker.io/jekyll/jekyll:4"
NGINX_IMAGE="docker.io/library/nginx:alpine"

resolve_runtime() {
  if [[ -n "$RUNTIME" ]]; then
    if ! command -v "$RUNTIME" >/dev/null 2>&1; then
      echo "error: container runtime '$RUNTIME' not found on PATH" >&2
      exit 1
    fi
    return
  fi

  if command -v docker >/dev/null 2>&1; then
    RUNTIME="docker"
  elif command -v podman >/dev/null 2>&1; then
    RUNTIME="podman"
  else
    echo "error: no container runtime found. Install Docker or Podman, or pass --runtime." >&2
    exit 1
  fi
}

volume_mount() {
  local host_path="$1"
  local container_path="$2"
  local opts="${3:-}"

  if [[ "$RUNTIME" == "podman" ]]; then
    if [[ -n "$opts" ]]; then
      opts="${opts},Z"
    else
      opts="Z"
    fi
  fi

  if [[ -n "$opts" ]]; then
    printf '%s:%s:%s' "$host_path" "$container_path" "$opts"
  else
    printf '%s:%s' "$host_path" "$container_path"
  fi
}

bundle_setup_cmd='bundle config set --local path vendor/bundle && bundle install --jobs 4 --retry 3'

stop_container() {
  local name="$1"
  if "$RUNTIME" container exists "$name" >/dev/null 2>&1; then
    "$RUNTIME" stop -t 3 "$name" >/dev/null 2>&1 || true
    "$RUNTIME" rm -f "$name" >/dev/null 2>&1 || true
  fi
}

port_in_use() {
  local port="$1"
  if command -v ss >/dev/null 2>&1; then
    ss -tln 2>/dev/null | grep -qE "[:.]${port}[[:space:]]"
    return
  fi

  if command -v lsof >/dev/null 2>&1; then
    lsof -iTCP:"$port" -sTCP:LISTEN -t >/dev/null 2>&1
    return
  fi

  return 1
}

stop_containers_on_port() {
  local port="$1"
  local cid

  while IFS= read -r cid; do
    [[ -z "$cid" ]] && continue
    echo "Stopping container ${cid} (port ${port})..."
    "$RUNTIME" stop -t 3 "$cid" >/dev/null 2>&1 || true
    "$RUNTIME" rm -f "$cid" >/dev/null 2>&1 || true
  done < <("$RUNTIME" ps --filter "publish=${port}" --format '{{.ID}}' 2>/dev/null || true)
}

release_port() {
  local port="$1"
  stop_containers_on_port "$port"

  if port_in_use "$port"; then
    echo "error: port ${port} is still in use." >&2
    echo "Run: ./scripts/invoke-docs-site.sh stop" >&2
    echo "Or find the process: ss -tlnp | grep ':${port} '" >&2
    exit 1
  fi
}

stop_all_docs_containers() {
  stop_container "$SERVE_CONTAINER_NAME"
  stop_container "$PREVIEW_CONTAINER_NAME"
  stop_containers_on_port "$SERVE_PORT"
  stop_containers_on_port "35729"
  stop_containers_on_port "$PREVIEW_PORT"
}

publish_host_port() {
  printf '127.0.0.1:%s:%s' "$1" "$2"
}

wait_for_http() {
  local url="$1"
  local attempt

  for attempt in $(seq 1 60); do
    if curl -sf "$url" >/dev/null 2>&1; then
      return 0
    fi
    sleep 1
  done

  echo "error: timed out waiting for ${url}" >&2
  return 1
}

jekyll_container() {
  local -a jekyll_args=("$@")

  "$RUNTIME" run --rm \
    -v "$(volume_mount "$DOCS_DIR" /srv/jekyll)" \
    -w /srv/jekyll \
    "$JEKYLL_IMAGE" \
    sh -c "$bundle_setup_cmd && bundle exec jekyll ${jekyll_args[*]}"
}

follow_container_logs() {
  local name="$1"
  local port="$2"
  local extra_port="${3:-}"

  local cleanup
  cleanup() {
    trap - INT TERM
    echo ""
    echo "Stopping..."
    stop_container "$name"
    stop_containers_on_port "$port"
    if [[ -n "$extra_port" ]]; then
      stop_containers_on_port "$extra_port"
    fi
    exit 130
  }
  trap cleanup INT TERM

  if ! "$RUNTIME" logs -f "$name"; then
    cleanup
  fi

  cleanup
}

serve_docs() {
  stop_container "$SERVE_CONTAINER_NAME"
  release_port "$SERVE_PORT"
  release_port "35729"

  local serve_cmd
  serve_cmd="$bundle_setup_cmd && exec bundle exec jekyll serve --host 0.0.0.0 --port 4000 --baseurl '' --livereload --livereload-port 35729 --force_polling"

  echo "Starting Jekyll dev server at http://127.0.0.1:${SERVE_PORT}/ ..."
  echo "On WSL, use 127.0.0.1 rather than localhost."
  echo "Press Ctrl+C to stop (or run: ./scripts/invoke-docs-site.sh stop)"

  "$RUNTIME" run -d --rm --init \
    --name "$SERVE_CONTAINER_NAME" \
    -p "$(publish_host_port "$SERVE_PORT" 4000)" \
    -p "$(publish_host_port 35729 35729)" \
    -v "$(volume_mount "$DOCS_DIR" /srv/jekyll)" \
    -w /srv/jekyll \
    "$JEKYLL_IMAGE" \
    sh -c "$serve_cmd"

  wait_for_http "http://127.0.0.1:${SERVE_PORT}/"
  echo "Server ready."

  follow_container_logs "$SERVE_CONTAINER_NAME" "$SERVE_PORT" "35729"
}

preview_docs() {
  stop_container "$PREVIEW_CONTAINER_NAME"
  release_port "$PREVIEW_PORT"

  echo "Building Jekyll site for local preview..."
  jekyll_container build --destination _site --baseurl ''
  assert_site_index

  echo "Serving docs/_site at http://127.0.0.1:${PREVIEW_PORT}/ ..."
  echo "On WSL, use 127.0.0.1 rather than localhost."
  echo "Press Ctrl+C to stop (or run: ./scripts/invoke-docs-site.sh stop)"

  "$RUNTIME" run -d --rm --init \
    --name "$PREVIEW_CONTAINER_NAME" \
    -p "$(publish_host_port "$PREVIEW_PORT" 80)" \
    -v "$(volume_mount "$SITE_PATH" /usr/share/nginx/html ro)" \
    "$NGINX_IMAGE"

  wait_for_http "http://127.0.0.1:${PREVIEW_PORT}/"
  echo "Preview ready."

  follow_container_logs "$PREVIEW_CONTAINER_NAME" "$PREVIEW_PORT"
}

assert_site_index() {
  if [[ -f "$SITE_PATH/index.html" ]]; then
    return
  fi

  echo "error: Jekyll did not produce docs/_site/index.html" >&2
  exit 1
}

resolve_runtime

case "$COMMAND" in
  build)
    echo "Building Jekyll site to docs/_site..."
    jekyll_container build --destination _site
    assert_site_index
    echo "Done. Output: $SITE_PATH"
    ;;
  serve)
    serve_docs
    ;;
  preview)
    preview_docs
    ;;
  stop)
    stop_all_docs_containers
    echo "Stopped documentation containers."
    ;;
esac
