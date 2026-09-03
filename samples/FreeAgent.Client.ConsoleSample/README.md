# FreeAgent.Client console sample

A minimal console app that shows the full OAuth flow and lists contact display names. Use it to smoke-test the SDK or as a starting point for your own integration.

The OAuth step uses a **local browser redirect**: the app prints an authorisation URL, opens your browser (when possible), listens on `http://127.0.0.1:8765/callback`, and exchanges the returned code for an access token. If the local listener cannot start, you can paste the redirect URL manually instead.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A FreeAgent **sandbox** developer application ([sandbox developer portal](https://dev.sandbox.freeagent.com))
- A FreeAgent **sandbox account** to approve the OAuth request ([sandbox signup](https://signup.sandbox.freeagent.com/signup))

---

## 1. Register the redirect URI

In your sandbox OAuth app settings, add this **exact** redirect URI:

```
http://127.0.0.1:8765/callback
```

Note the `http` scheme, `127.0.0.1` host, port `8765`, and `/callback` path.

---

## 2. Configure credentials

### Option A — user secrets (recommended, shared with the Blazor sample)

This project uses the same user-secrets ID as `FreeAgent.Client.Sample`. If you already configured the Blazor sample, **you do not need to set secrets again** — just run the console app.

Otherwise, from this directory:

```bash
cd samples/FreeAgent.Client.ConsoleSample

dotnet user-secrets set "FreeAgent:ClientId"     "<your-client-id>"
dotnet user-secrets set "FreeAgent:ClientSecret" "<your-client-secret>"
```

The console sample defaults `RedirectUri` to `http://127.0.0.1:8765/callback` in `appsettings.json`. If your user secrets include `FreeAgent:RedirectUri` from the Blazor sample (`https://localhost:5001/oauth/callback`), override it for this app:

```bash
dotnet user-secrets set "FreeAgent:RedirectUri" "http://127.0.0.1:8765/callback"
```

### Option B — local settings file

```bash
cp appsettings.local.json.example appsettings.local.json
```

Edit `appsettings.local.json` with your sandbox credentials. Never commit this file.

### Option C — environment variables

```bash
export FREEAGENT_CLIENT_ID="your-client-id"
export FREEAGENT_CLIENT_SECRET="your-client-secret"
export FREEAGENT_REDIRECT_URI="http://127.0.0.1:8765/callback"
```

---

## 3. Run the sample

From the repository root:

```bash
dotnet run --project samples/FreeAgent.Client.ConsoleSample
```

Or from this directory:

```bash
dotnet run
```

### What happens

1. The app prints a FreeAgent authorisation URL. **On WSL, copy-paste the URL manually** — auto-open can truncate query parameters and break OAuth.
2. Log in to your sandbox account (if prompted) and approve access.
3. FreeAgent redirects to `http://127.0.0.1:8765/callback` — the app captures the code automatically.
4. The app exchanges the code for tokens and prints each contact's `DisplayName`.

If the browser cannot reach the local callback (for example on a remote machine), copy the **full redirect URL** from the browser address bar and paste it into the console when prompted.

---

## Testing against the published NuGet package

This sample references the SDK project directly for contributors. To test the **published** package instead:

```bash
dotnet remove package FreeAgent.Client   # only if added
dotnet add package FreeAgent.Client --version 0.1.0-alpha.1
```

Remove the `ProjectReference` from `FreeAgent.Client.ConsoleSample.csproj` when switching to the NuGet package.

---

## Related documentation

- [Getting started](../../docs/tutorial/getting-started.md)
- [Prerelease and OAuth scope](../../docs/explanation/prerelease-and-oauth.md)
- [Blazor sample](../FreeAgent.Client.Sample/) — fuller interactive SDK workbench
