# Error handling

The SDK surfaces failures through a small typed exception hierarchy. Catch the most specific type first.

## Recommended pattern

```csharp
using FreeAgent.Client;

try
{
    var company = await client.Company.GetCompanyAsync();
}
catch (FreeAgentRateLimitException ex)
{
    // 429 Too Many Requests — honour Retry-After when scheduling a retry
    Console.WriteLine($"Rate limited after {ex.AttemptCount} attempts. Retry after {ex.RetryAfter}.");
}
catch (FreeAgentOAuthException ex)
{
    // OAuth token exchange or refresh failed
    Console.WriteLine($"OAuth error: {ex.Message}");
}
catch (FreeAgentApiException ex)
{
    // API error, transport failure after retries, or missing response payload
    Console.WriteLine($"API error ({ex.StatusCode}): {ex.Message}");
    Console.WriteLine($"Attempts: {ex.AttemptCount}");
}
```

## Retries

`FreeAgentHttpClient` applies bounded retries for transient failures on safe methods (`GET`, `DELETE` by default). Mutating methods (`POST`, `PUT`) are not retried unless configured via `FreeAgentHttpClientOptions.AdditionalRetriableMethods`.

Rate-limit responses honour `Retry-After` when present.

## Missing payload branches

Service methods validate response envelopes. If a required JSON branch is missing (for example `"company": null`), the SDK throws `FreeAgentApiException` rather than returning null silently.
