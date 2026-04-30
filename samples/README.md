# FreeAgent SDK Sample App

An interactive Blazor Server app for testing the `FreeAgent.Client` SDK.
Use it to exercise SDK calls against the FreeAgent API as new services are added to the codebase.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A FreeAgent developer account and registered OAuth app
  - Production apps: <https://dev.freeagent.com>
  - Sandbox apps: <https://dev.sandbox.freeagent.com>

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

| Page | SDK call tested |
|------|----------------|
| [/company](https://localhost:5001/company) | `CompanyService.GetCompanyAsync()` |

Additional pages will be added as new services are implemented.

---

## Notes for Contributors

- **Token lifetime:** The token is held in memory. It is lost when the app restarts; reconnect to get a new one. The SDK handles automatic token refresh within an active session.
- **Sandbox vs production:** Choose the target environment from the Connect panel before starting OAuth. After connecting, disconnect to switch environments.
- **Port:** The sample uses port `5001` (HTTPS). If you change it in `Properties/launchSettings.json`, update the redirect URI in both your FreeAgent app registration and your user secrets.
- **Do not commit credentials.** `appsettings.json` contains only empty placeholders. User secrets are stored outside the repository in your OS user profile.
