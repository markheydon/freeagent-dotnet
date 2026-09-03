# FreeAgent SDK Sample Apps

Samples for trying the `FreeAgent.Client` SDK against the FreeAgent API.

| Sample | Purpose |
|--------|---------|
| [FreeAgent.Client.ConsoleSample](FreeAgent.Client.ConsoleSample/) | Minimal console app — OAuth in the browser, list contact display names |
| [FreeAgent.Client.Sample](FreeAgent.Client.Sample/) | Blazor Server workbench for exercising SDK endpoints interactively |

---

## Console sample (quick start)

See [FreeAgent.Client.ConsoleSample/README.md](FreeAgent.Client.ConsoleSample/README.md) for a minimal OAuth + contacts listing example.

---

## Blazor sample (full workbench)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A FreeAgent developer account and registered OAuth app
  - Production apps: <https://dev.freeagent.com>
  - Sandbox apps: <https://dev.sandbox.freeagent.com>
- A FreeAgent **sandbox account** for OAuth approval (separate from the developer portal):
  - Sign up: <https://signup.sandbox.freeagent.com/signup>
  - Log in: <https://login.sandbox.freeagent.com/login>

---

## 1. Register a FreeAgent OAuth App

1. Sign in to the appropriate FreeAgent developer portal (sandbox recommended for development).
2. Create a new application.
3. Set the **OAuth Redirect URI** to exactly:

   ```
   https://localhost:5001/oauth/callback
   ```

   > **Important:** The redirect URI must match exactly — including the scheme (`https`), host (`localhost`), port (`:5001`), and path (`/oauth/callback`). A mismatch will cause the authorization flow to fail.

4. Note your **OAuth identifier** and **OAuth secret**.

---

## 2. Configure Credentials via User Secrets

Never put credentials in `appsettings.json`. Use .NET user secrets instead:

```bash
cd samples/FreeAgent.Client.Sample

dotnet user-secrets set "FreeAgent:ClientId"     "<your-client-id>"
dotnet user-secrets set "FreeAgent:ClientSecret" "<your-client-secret>"
```

The `FreeAgent:RedirectUri` value in `appsettings.json` defaults to `https://localhost:5001/oauth/callback` and should match what you registered in step 1. Override it via user secrets if you use a different port:

```bash
dotnet user-secrets set "FreeAgent:RedirectUri" "https://localhost:5001/oauth/callback"
```

---

## 3. Run the App

```bash
cd samples/FreeAgent.Client.Sample
dotnet run
```

The app starts at `https://localhost:5001`. Your browser may show a certificate warning for the local development certificate; accept it, or run `dotnet dev-certs https --trust` first.

---

## 4. Authorize the App

1. Navigate to <https://localhost:5001>.
2. Click **Connect to FreeAgent**.
3. You are redirected to FreeAgent's authorization page — approve the app.
4. FreeAgent redirects back to `https://localhost:5001/oauth/callback`.
5. The app exchanges the code for a token and shows **Connected** in the header.

---

## 5. Test SDK Calls

These pages exercise the SDK endpoints that exist today. Do not expect UI for resources that are not yet implemented.

| Page | SDK call tested | Probe features |
|------|-----------------|----------------|
| [/company](https://localhost:5001/company) | `CompanyService.GetCompanyAsync()` | Wire-to-model mapping, raw JSON |
| [/company/business-categories](https://localhost:5001/company/business-categories) | `CompanyService.GetBusinessCategoriesAsync()` | List output |
| [/company/tax-timeline](https://localhost:5001/company/tax-timeline) | `CompanyService.GetTaxTimelineAsync()` | List output |
| [/contacts](https://localhost:5001/contacts) | `ContactService.GetContactsPageAsync()` and `GetAllContactsAsync()` | Per-row mapping inspection, list filters |
| [/contacts/detail](https://localhost:5001/contacts/detail) | `GetContactAsync()`, `CreateContactAsync()`, `UpdateContactAsync()`, `DeleteContactAsync()` | CRUD probes, Turpinverse seed data, full-detail fixture, progress bar |

Contributors adding endpoints should follow [`docs/contributing/sample-probe-pages.md`](../docs/contributing/sample-probe-pages.md).


---

## Sandbox Rate-Limit Diagnostics

The sample app provides a dedicated developer tooling page for live rate-limit diagnostics:

- **Route:** `/sandbox/rate-limit-test` (visible only when running the sample app)
- **Purpose:** Allows developers to test and observe FreeAgent API rate-limiting behaviour in real time, without affecting production data.
- **Sandbox-only:** This page is restricted to sandbox OAuth sessions. Attempting to use it with a production connection will show an error.
- **X-RateLimit-Test header:** Optionally sends the `X-RateLimit-Test: true` header, which triggers test-mode rate-limiting in the FreeAgent sandbox API. This enables safe, repeatable limit testing without impacting real usage quotas.
- **Not an SDK endpoint:** This is a sample-app-only diagnostics tool. It does not expose new SDK surface or production API features.

Use this page to:

- Run multiple requests against the `/v2/company` endpoint
- Inspect returned `X-RateLimit-*` headers and `Retry-After` values
- Experiment with rate-limit handling and SDK retry logic

See the page in the running app for full details and usage instructions.


## Notes for Contributors

- **Probe pages:** When adding SDK endpoints, follow [`docs/contributing/sample-probe-pages.md`](../docs/contributing/sample-probe-pages.md). Copy patterns from Company and Contacts.
- **Token lifetime:** The OAuth session is restored from a short-lived browser cookie (~1 hour). Reconnect after expiry if needed.
- **OAuth CSRF state:** Pending authorisation state is also stored in a short-lived cookie (~15 minutes) so callbacks still validate after an app restart during local development.
- **Session security:** Tokens and pending OAuth state are stored as plaintext JSON in `HttpOnly` cookies. This is intentional for local development only — do not deploy the sample's cookie persistence pattern to production.
- **Sandbox vs production:** Choose the target environment from the Connect panel before starting OAuth. After connecting, disconnect to switch environments.
- **Port:** The sample uses port `5001` (HTTPS). If you change it in `Properties/launchSettings.json`, update the redirect URI in both your FreeAgent app registration and your user secrets.
- **Do not commit credentials.** `appsettings.json` contains only empty placeholders. User secrets are stored outside the repository in your OS user profile.
