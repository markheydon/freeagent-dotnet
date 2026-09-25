#!/usr/bin/env bash
# Exchanges a FreeAgent sandbox refresh token for a short-lived access token.
# Writes access_token and refresh_token to GITHUB_OUTPUT when running in GitHub Actions.
set -euo pipefail

: "${FREEAGENT_CLIENT_ID:?FREEAGENT_CLIENT_ID is required}"
: "${FREEAGENT_CLIENT_SECRET:?FREEAGENT_CLIENT_SECRET is required}"
: "${FREEAGENT_REFRESH_TOKEN:?FREEAGENT_REFRESH_TOKEN is required}"

endpoint="${FREEAGENT_TOKEN_ENDPOINT:-https://api.sandbox.freeagent.com/v2/token_endpoint}"

response="$(
  curl -fsS -X POST "$endpoint" \
    -d "grant_type=refresh_token" \
    -d "refresh_token=${FREEAGENT_REFRESH_TOKEN}" \
    -d "client_id=${FREEAGENT_CLIENT_ID}" \
    -d "client_secret=${FREEAGENT_CLIENT_SECRET}"
)"

access_token="$(jq -r '.access_token // empty' <<<"$response")"
refresh_token="$(jq -r '.refresh_token // empty' <<<"$response")"

if [[ -z "$access_token" ]]; then
  echo "Token refresh failed. Response:" >&2
  echo "$response" >&2
  exit 1
fi

# Step outputs are not auto-masked like secrets.*; register tokens before any later step logs env.
if [[ -n "${GITHUB_ACTIONS:-}" ]]; then
  echo "::add-mask::${access_token}"
  if [[ -n "$refresh_token" ]]; then
    echo "::add-mask::${refresh_token}"
  fi
fi

if [[ -n "${GITHUB_OUTPUT:-}" ]]; then
  {
    echo "access_token=${access_token}"
    echo "refresh_token=${refresh_token}"
  } >>"$GITHUB_OUTPUT"
else
  echo "access_token=${access_token}"
  echo "refresh_token=${refresh_token}"
fi
