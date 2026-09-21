---
title: OAuth
parent: API coverage
nav_order: 7
---

# OAuth protocol helpers

Protocol-level OAuth 2.0 utilities for authorisation URL construction, code exchange, and token refresh. The SDK does **not** host callback endpoints or orchestrate browser flows - your application owns that.

| | |
|---|---|
| **SDK type** | `FreeAgentOAuthClient` |
| **FreeAgent docs** | [dev.freeagent.com/docs/oauth](https://dev.freeagent.com/docs/oauth) |

For token refresh patterns with `FreeAgentClient`, see [Token refresh](../how-to/token-refresh.md). For scope boundaries, see [Prerelease and OAuth scope](../explanation/prerelease-and-oauth.md).

## Constructors

```csharp
FreeAgentOAuthClient(
    string clientId,
    string clientSecret,
    string redirectUri,
    FreeAgentEnvironment environment = FreeAgentEnvironment.Production)

FreeAgentOAuthClient(
    string clientId,
    string clientSecret,
    string redirectUri,
    HttpClient httpClient,
    FreeAgentEnvironment environment = FreeAgentEnvironment.Production)
```

Pass the same `FreeAgentEnvironment` to both `FreeAgentOAuthClient` and `FreeAgentClient`.

## Methods

### GetAuthorizationUrl

```csharp
string GetAuthorizationUrl(string? state = null)
```

| Parameter | Type | Required | Default | Purpose |
|-----------|------|----------|---------|---------|
| `state` | `string?` | No | `null` | CSRF protection value echoed on callback |

**Returns:** URL to redirect the user to FreeAgent for authorisation.

**Sample:**

```csharp
var authUrl = oauthClient.GetAuthorizationUrl(state: "optional-csrf-state");
```

---

### ExchangeCodeForTokenAsync

```csharp
Task<OAuthTokenResponse> ExchangeCodeForTokenAsync(
    string code,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `code` | `string` | Yes | - | `code` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**Returns:** Access and refresh tokens.

**Throws:** `FreeAgentOAuthException` on token endpoint failure.

**Sample:**

```csharp
var token = await oauthClient.ExchangeCodeForTokenAsync(authorizationCode);
using var client = new FreeAgentClient(oauthClient, token, environment);
```

---

### RefreshTokenAsync

```csharp
Task<OAuthTokenResponse> RefreshTokenAsync(
    string refreshToken,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `refreshToken` | `string` | Yes | - | `refresh_token` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**Returns:** New access token (and typically a new refresh token).

**Throws:** `FreeAgentOAuthException` on token endpoint failure.

## OAuthTokenResponse

| Property | Type | Wire name | Notes |
|----------|------|-----------|-------|
| `AccessToken` | `string` | `access_token` | Bearer token for API calls |
| `TokenType` | `string` | `token_type` | Usually `Bearer` |
| `RefreshToken` | `string?` | `refresh_token` | For obtaining new access tokens |
| `ExpiresIn` | `int` | `expires_in` | Lifetime in seconds |
| `ExpiresAtUtc` | `DateTimeOffset?` | `expires_at_utc` | Persisted absolute expiry |
| `IsExpired` | `bool` | - | Computed |
| `IsExpiringSoon` | `bool` | - | True within five minutes of expiry |
| `TimeUntilExpiry` | `TimeSpan` | - | Remaining lifetime |

Call `InitialiseExpiryUtc()` after deserialising a stored token if `ExpiresAtUtc` is not set.

## Errors

Token endpoint failures throw `FreeAgentOAuthException`. See [Error handling](../how-to/error-handling.md).
