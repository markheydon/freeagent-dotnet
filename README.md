# FreeAgent .NET Client

A .NET client library for the [FreeAgent API](https://dev.freeagent.com/docs) with OAuth 2.0 support, rate limiting, and pagination.

[![NuGet](https://img.shields.io/nuget/v/FreeAgent.Client.svg)](https://www.nuget.org/packages/FreeAgent.Client/)

## Features

- ✅ OAuth 2.0 authentication with automatic token refresh
- ✅ Rate limiting support to respect API constraints
- ✅ Pagination support for large result sets
- ✅ Company API support
- ✅ Built for .NET 8.0
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
```

**Note:** The client implements `IDisposable` and should be disposed when done to release HTTP resources properly.

### Token Refresh

Tokens can be refreshed manually:

```csharp
if (token.IsExpiringSoon)
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

- **Company API**: Get company information

More endpoints will be added in future releases.

## Rate Limiting

The client automatically handles rate limiting:

- Respects `X-RateLimit-*` headers from the API
- Implements automatic retry with exponential backoff for 429 responses
- Default safety delay of 1 second between requests

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
}
catch (FreeAgentApiException ex)
{
    // Handle other API errors
    Console.WriteLine($"API error: {ex.Message}");
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
- .NET 8.0 SDK or later

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Resources

- [FreeAgent API Documentation](https://dev.freeagent.com/docs)
- [FreeAgent OAuth Guide](https://dev.freeagent.com/docs/quick_start)
- [API Introduction](https://dev.freeagent.com/docs/introduction)
