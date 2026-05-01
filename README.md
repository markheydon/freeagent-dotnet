# FreeAgent .NET Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, retries, typed transport errors, and pagination.

[![NuGet](https://img.shields.io/nuget/v/FreeAgent.Client.svg)](https://www.nuget.org/packages/FreeAgent.Client/)
[![NuGet (prerelease)](https://img.shields.io/nuget/vpre/FreeAgent.Client.svg?label=nuget%20prerelease)](https://www.nuget.org/packages/FreeAgent.Client/)

> ⚠️ **Prerelease software.** This package is currently in alpha. Public APIs may change between releases. See [VERSIONING.md](VERSIONING.md) for the full versioning policy and stability expectations.

## Features

- ✅ OAuth 2.0 authentication with automatic token refresh
- ✅ Rate limiting support to respect API constraints
- ✅ Bounded retries for transient failures
- ✅ Typed transport exception hierarchy
- ✅ Pagination support (single-page and auto-pagination)
- ✅ Company API support (company details, business categories, tax timeline)
- ✅ Contacts API support (list page and auto-pagination)
- ✅ Built for .NET 10.0
- ✅ Fully async/await
- ✅ Comprehensive XML documentation

## Installation

```bash
dotnet add package FreeAgent.Client
```

## Quick Start

### OAuth 2.0 Authentication

First, create an OAuth client to handle the authentication flow:

```csharp
using FreeAgent.Client.Authentication;

var oauthClient = new FreeAgentOAuthClient(
    clientId: "your-client-id",
    clientSecret: "your-client-secret",
    redirectUri: "http://localhost:5000/callback"
);

// Step 1: Generate authorization URL and redirect user
var authUrl = oauthClient.GetAuthorizationUrl(state: "optional-state");
// Redirect user to authUrl

// Step 2: Exchange authorization code for tokens (in your callback handler)
var token = await oauthClient.ExchangeCodeForTokenAsync(code);
```

### Using the Client

Once you have an access token, create a FreeAgent client:

```csharp
using FreeAgent.Client;

// Option 1: With just an access token
using var client = new FreeAgentClient("your-access-token");

// Option 2: With OAuth client for automatic token refresh
using var client = new FreeAgentClient(oauthClient, token);

// Get company information
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"Company: {company.Name}");
Console.WriteLine($"Currency: {company.Currency}");

// Get company business categories
var categories = await client.Company.GetBusinessCategoriesAsync();
Console.WriteLine($"Categories returned: {categories.Count}");

// Get upcoming tax events
var timeline = await client.Company.GetTaxTimelineAsync();
Console.WriteLine($"Upcoming tax events: {timeline.Count}");

// Contacts single-page access
var firstPage = await client.Contacts.GetContactsPageAsync(page: 1, perPage: 25);
Console.WriteLine($"Contacts page 1 items: {firstPage.Items.Count}");

// Contacts auto-pagination
await foreach (var contact in client.Contacts.GetAllContactsAsync(perPage: 50))
{
    Console.WriteLine(contact.ContactName);
}
```

**Note:** The client implements `IDisposable` and should be disposed when done to release HTTP resources properly.

### Token Refresh

Tokens can be refreshed manually:

```csharp
if (token.IsExpiringSoon && !string.IsNullOrEmpty(token.RefreshToken))
{
    var newToken = await oauthClient.RefreshTokenAsync(token.RefreshToken);
    // Update your stored token
}
```

Or use the client with automatic refresh:

```csharp
// This will automatically refresh the token when needed
var client = new FreeAgentClient(oauthClient, token);
```

## API Coverage

Currently, this library supports:

- **Company API**:
    - Get company information
    - List all business categories
    - Get upcoming tax events
- **Contacts API**:
    - Get a single contacts page
    - Auto-paginate all contacts

More endpoints will be added in future releases.

## Rate Limiting

The client automatically handles FreeAgent rate limiting:

- Respects `X-RateLimit-*` headers from the API
- Default safety delay of 1 second between requests

## Retries

The client applies bounded retries by default for transient failures:

- Retries up to `MaxNetworkRetries = 2` (in addition to the initial request)
- Uses exponential backoff with optional jitter
- Honors `Retry-After` for `429 Too Many Requests`
- Retries safe methods (`GET`, `DELETE`) by default
- Mutating methods are not retried unless explicitly opted in via `FreeAgentHttpClientOptions.AdditionalRetriableMethods`

You can configure retry behavior through `FreeAgentHttpClientOptions` on `FreeAgentClient` constructors.

## Error Handling

The library provides specific exception types:

```csharp
using FreeAgent.Client.Http;
using FreeAgent.Client.Authentication;

try
{
    var company = await client.Company.GetCompanyAsync();
}
catch (FreeAgentRateLimitException ex)
{
    // Handle rate limit exceeded
    Console.WriteLine($"Rate limit exceeded: {ex.Message}");
    Console.WriteLine($"Attempts: {ex.AttemptCount}");
    Console.WriteLine($"Retry-After: {ex.RetryAfter}");
}
catch (FreeAgentTimeoutException ex)
{
    // Handle timeout
    Console.WriteLine($"Timeout calling {ex.RequestPath}: {ex.Message}");
}
catch (FreeAgentNetworkException ex)
{
    // Handle network failures
    Console.WriteLine($"Network error calling {ex.RequestPath}: {ex.Message}");
}
catch (FreeAgentTransportException ex)
{
    // Handle other transport-layer failures
    Console.WriteLine($"Transport error: {ex.Message}");
}
catch (FreeAgentApiException ex)
{
    // Handle other API errors
    Console.WriteLine($"API error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Attempts: {ex.AttemptCount}");
}
catch (FreeAgentOAuthException ex)
{
    // Handle OAuth errors
    Console.WriteLine($"OAuth error: {ex.Message}");
}
```

## Building from Source

```bash
# Clone the repository
git clone https://github.com/markheydon/freeagent-dotnet.git
cd freeagent-dotnet

# Build
dotnet build

# Run tests
dotnet test

# Create NuGet package
dotnet pack src/FreeAgent.Client/FreeAgent.Client.csproj -c Release
```

## Development

Requirements:
- .NET 10.0 SDK or later

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Versioning

This package follows [Semantic Versioning](https://semver.org/). Until the MVP is complete, all releases carry a prerelease tag (e.g. `0.1.0-alpha.1`). Prerelease packages do not carry stability guarantees — public APIs may change between versions.

See [VERSIONING.md](VERSIONING.md) for the full policy, stage transition criteria, and when the first stable `1.0.0` will be released.

## Resources

- [FreeAgent API Documentation](https://dev.freeagent.com/docs)
- [FreeAgent OAuth Guide](https://dev.freeagent.com/docs/quick_start)
- [API Introduction](https://dev.freeagent.com/docs/introduction)
