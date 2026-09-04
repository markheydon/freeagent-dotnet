# FreeAgent.Client console sample

A minimal console app that shows the full OAuth flow and lists contact display names. Use it to smoke-test the SDK or as a starting point for your own integration.

The OAuth step uses a **local browser redirect**: the app prints an authorisation URL, opens your browser (when possible), listens on `http://127.0.0.1:8765/callback`, and exchanges the returned code for an access token. If the local listener cannot start (or you are on WSL2), paste the **full redirect URL** manually instead.

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

This project uses the same user-secrets ID as `FreeAgent.Client.BlazorSample`. If you already configured the Blazor sample, **you only need to share client ID and secret** — the console app ignores the Blazor `RedirectUri` from user-secrets and keeps its own default (`http://127.0.0.1:8765/callback`).

Otherwise, from this directory:

```bash
cd samples/FreeAgent.Client.ConsoleSample

dotnet user-secrets set "FreeAgent:ClientId"     "<your-client-id>"
dotnet user-secrets set "FreeAgent:ClientSecret" "<your-client-secret>"
```

Register `http://127.0.0.1:8765/callback` in your FreeAgent OAuth app (see step 1). You only need to set `FreeAgent:RedirectUri` in user-secrets if you use a non-default console redirect URI.

### Option B — local settings file

```bash
cp appsettings.local.json.example appsettings.local.json
```

Edit `appsettings.local.json` with your sandbox credentials. Never commit this file.

If you create or change this file after building, run `dotnet build` again so it is copied to the output directory.

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

1. The app prints a FreeAgent authorisation URL.
2. Log in to your sandbox account (if prompted) and approve access.
3. FreeAgent redirects to `http://127.0.0.1:8765/callback` — the app captures the code automatically when the browser can reach the local listener.
4. The app exchanges the code for tokens and prints each contact's `DisplayName`.

### WSL2 and manual paste

On **WSL2**, the browser usually runs on Windows while the sample listens inside Linux. A redirect to `http://127.0.0.1:8765/callback` therefore hits Windows loopback, not the WSL listener — **automatic callback capture will usually fail**. Copy the authorisation URL manually, approve access, then paste the **full redirect URL** from the browser address bar into the console when prompted.

On any platform, if the local listener cannot start (port in use, permissions, remote machine), use the same manual paste flow.

---

## SDK source: local project vs NuGet package

This sample is written like a **consumer app** (it imports `FreeAgent.Client` and calls the public API). In the repository it defaults to a **project reference** so contributors can run it against in-progress SDK changes without publishing.

To **smoke-test the published NuGet package** instead, pass `UseLocalFreeAgentClient=false`:

```bash
# From the repository root
dotnet run --project samples/FreeAgent.Client.ConsoleSample -p:UseLocalFreeAgentClient=false

# Or from this directory
dotnet run -p:UseLocalFreeAgentClient=false
```

To switch back to the local SDK (default):

```bash
dotnet run
# or explicitly:
dotnet run -p:UseLocalFreeAgentClient=true
```

The pinned NuGet version is defined in [`Directory.Packages.props`](../../Directory.Packages.props). Bump it when testing a newer published release.

### Copying this sample outside the repository

If you copy the project files into your own solution, remove the `UseLocalFreeAgentClient` property and project-reference block, then add the package:

```bash
dotnet add package FreeAgent.Client --version 0.1.0-alpha.2
```

---

## Related documentation

- [Getting started](../../docs/tutorial/getting-started.md)
- [Prerelease and OAuth scope](../../docs/explanation/prerelease-and-oauth.md)
- [Blazor sample](../FreeAgent.Client.BlazorSample/) — fuller interactive SDK workbench
