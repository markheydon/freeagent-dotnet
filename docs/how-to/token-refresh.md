# Token refresh

Access tokens expire. The SDK supports manual refresh and automatic refresh when constructed with an OAuth client and token.

## Manual refresh

```csharp
if (token.TimeUntilExpiry < TimeSpan.FromMinutes(5)
    && !string.IsNullOrEmpty(token.RefreshToken))
{
    token = await oauthClient.RefreshTokenAsync(token.RefreshToken);
    // Persist the updated token in your application storage
}
```

## Automatic refresh

When you construct `FreeAgentClient` with `FreeAgentOAuthClient` and `OAuthTokenResponse`, the HTTP layer refreshes the token when it is close to expiry:

```csharp
using var client = new FreeAgentClient(oauthClient, token);
var contacts = await client.Contacts.GetContactsPageAsync();
```

The refreshed token is held in the client instance. Persist tokens in your application if sessions survive beyond a single client lifetime.

## Scope boundary

The SDK does not manage user sessions, browser redirects, or token storage. Your application owns persistence and the authorisation UI.
