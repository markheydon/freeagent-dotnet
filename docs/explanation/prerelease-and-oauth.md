# Prerelease policy and OAuth scope

## Prerelease policy

The SDK is in **alpha** (`0.x.y-alpha.n`). Prerelease packages:

- May introduce breaking public API changes without a major version bump
- Are suitable for integration trials and feedback, not production stability guarantees

See [VERSIONING.md](../../VERSIONING.md) for stage progression (alpha → beta → stable `1.0.0`) and exit criteria.

## OAuth scope boundary

**In scope (SDK utilities):**

- Constructing the OAuth authorisation URL
- Exchanging an authorisation code for tokens
- Refreshing access tokens with a refresh token

**Out of scope:**

- Browser or UI orchestration of the authorisation flow
- Hosting callback endpoints
- Persisting tokens or managing user sessions
- App-level OAuth state machines

Your application integrates the protocol helpers into its own auth stack. The [sample app](../../samples/README.md) demonstrates one possible Blazor Server integration for development — it is not part of the NuGet package.

## Goals alignment

This boundary supports [GOALS.md](../../GOALS.md) **G6**: protocol-level OAuth helpers without UI or flow management abstractions.
