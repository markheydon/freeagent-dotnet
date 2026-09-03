// Minimal console sample for FreeAgent.Client.
//
// OAuth in a nutshell (what this app demonstrates):
//   1. Send the user to FreeAgent's login/approve page (GetAuthorizationUrl).
//   2. FreeAgent redirects back to YOUR redirect URI with a short-lived "authorisation code".
//   3. Exchange that code for access + refresh tokens (ExchangeCodeForTokenAsync).
//   4. Call the API with FreeAgentClient using those tokens.
//
// The SDK provides step 1, 3, and 4 helpers only. YOUR app must own step 2 —
// i.e. receiving the redirect. Web apps use a callback route; this console app
// uses a temporary local HTTP listener (see ConsoleOAuthHelper).

using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;

// Load OAuth app credentials from user-secrets, appsettings, or environment variables.
// Register your app at https://dev.sandbox.freeagent.com (sandbox) or https://dev.freeagent.com (production).
var settings = AppSettings.Load();

// Always use Sandbox while learning. Production hits real customer data.
const FreeAgentEnvironment environment = FreeAgentEnvironment.Sandbox;

// FreeAgentOAuthClient handles the OAuth protocol: building the authorisation URL,
// exchanging codes for tokens, and refreshing expired access tokens.
// Pass the same redirect URI you registered in the FreeAgent developer dashboard.
using var oauthClient = new FreeAgentOAuthClient(
    settings.ClientId,
    settings.ClientSecret,
    settings.RedirectUri,
    environment);

Console.WriteLine("FreeAgent.Client console sample");
Console.WriteLine("Environment: Sandbox");
Console.WriteLine();

// --- OAuth step 1 & 2: get an authorisation code from the user ---
// ConsoleOAuthHelper opens the browser (when possible), listens on the redirect URI,
// and returns the ?code=... query parameter FreeAgent sends back.
// On WSL, copy-paste the printed URL into your browser if auto-open mangles it.
var authorizationCode = await ConsoleOAuthHelper.GetAuthorizationCodeAsync(
    oauthClient,
    settings.RedirectUri);

// --- OAuth step 3: swap the short-lived code for tokens ---
// The authorisation code expires in ~15 minutes and can only be used once.
// You receive an access token (for API calls) and a refresh token (to get new access tokens later).
Console.WriteLine("Exchanging authorisation code for an access token...");
var token = await oauthClient.ExchangeCodeForTokenAsync(authorizationCode);

// --- OAuth step 4: call the API ---
// Passing oauthClient + token lets FreeAgentClient refresh the access token automatically
// when it is close to expiring. Use the simpler FreeAgentClient(accessToken) overload
// only if you manage refresh yourself.
using var client = new FreeAgentClient(oauthClient, token, environment);

Console.WriteLine();
Console.WriteLine("Fetching contacts...");

// GetContactsPageAsync returns one page. Use GetAllContactsAsync for automatic pagination.
var contactsPage = await client.Contacts.GetContactsPageAsync(perPage: 100);

if (contactsPage.Items.Count == 0)
{
    Console.WriteLine("No contacts returned.");
    return;
}

Console.WriteLine($"Showing {contactsPage.Items.Count} of {contactsPage.Total} contact(s):");
Console.WriteLine();

foreach (var contact in contactsPage.Items)
{
    // DisplayName is a convenience property on the Contact model (not a separate API field).
    Console.WriteLine(contact.DisplayName);
}

if (contactsPage.HasNextPage)
{
    Console.WriteLine();
    Console.WriteLine("(Only the first page is shown. See docs/how-to/pagination.md for more.)");
}
