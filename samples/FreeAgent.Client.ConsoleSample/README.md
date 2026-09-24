# FreeAgent.Client console sample

A real-world console reference for the FreeAgent .NET SDK. It demonstrates typical integration patterns — OAuth, dependency injection, and read-only API calls — through an interactive menu of examples grouped by resource area.

Use the [Blazor sample](../FreeAgent.Client.BlazorSample/) when you need wire-to-model probe pages. Use this console sample when you want to see **how the SDK feels in a normal .NET app**.

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

### Option A - user secrets (recommended, shared with the Blazor sample)

This project uses the same user-secrets ID as `FreeAgent.Client.BlazorSample`. If you already configured the Blazor sample, **you only need to share client ID and secret** - the console app ignores the Blazor `RedirectUri` from user-secrets and keeps its own default (`http://127.0.0.1:8765/callback`).

Otherwise, from this directory:

```bash
cd samples/FreeAgent.Client.ConsoleSample

dotnet user-secrets set "FreeAgent:ClientId"     "<your-client-id>"
dotnet user-secrets set "FreeAgent:ClientSecret" "<your-client-secret>"
```

Register `http://127.0.0.1:8765/callback` in your FreeAgent OAuth app (see step 1). You only need to set `FreeAgent:RedirectUri` in user-secrets if you use a non-default console redirect URI.

### Option B - local settings file

```bash
cp appsettings.local.json.example appsettings.local.json
```

Edit `appsettings.local.json` with your sandbox credentials. Never commit this file.

If you create or change this file after building, run `dotnet build` again so it is copied to the output directory.

### Option C - environment variables

```bash
export FREEAGENT_CLIENT_ID="your-client-id"
export FREEAGENT_CLIENT_SECRET="your-client-secret"
export FREEAGENT_REDIRECT_URI="http://127.0.0.1:8765/callback"
```

---

## 3. Run the interactive menu

From the repository root:

```bash
dotnet run --project samples/FreeAgent.Client.ConsoleSample
```

Or from this directory:

```bash
dotnet run
```

### What happens

1. The app completes OAuth (browser flow or token from environment variables).
2. A category menu appears (Company, Contacts, Categories, Projects, Tasks, Timeslips, Notes, Users, Email addresses, Invoices).
3. Pick a category, then an example. Each example runs without further input — IDs and parent resources are resolved automatically from your sandbox data.
4. Examples that cannot run (for example, no projects in the account) report a clear skip message.

### Example catalogue

| Category | Examples |
|----------|----------|
| Company | Get company profile; list business categories; list tax timeline |
| Contacts | List active contacts; stream all contacts; get contact detail |
| Categories | List category sets; get category by nominal code |
| Projects | List projects; list active projects; get project detail; list projects for contact |
| Tasks | List tasks; list tasks for project; get task detail |
| Timeslips | List timeslips; list unbilled timeslips; list timeslips for task; get timeslip detail |
| Notes | List contact notes; list project notes; get note detail; create contact note; create project note; update note |
| Users | List users; get current user; get user by ID |
| Email addresses | List email addresses |
| Invoices | List invoices; list invoices filtered by contact; list invoice timeline; get invoice by id; create draft invoice with one line item; update invoice comments; duplicate invoice; send invoice email (template); mark invoice as sent; mark invoice as scheduled; mark invoice as draft; mark invoice as cancelled; get invoice PDF; get default additional text; update default additional text; delete default additional text |

New SDK resource areas should add a matching `*Samples.cs` provider class under `Samples/`.

### WSL2 and manual paste

On **WSL2**, the browser usually runs on Windows while the sample listens inside Linux. A redirect to `http://127.0.0.1:8765/callback` therefore hits Windows loopback, not the WSL listener - **automatic callback capture will usually fail**. Copy the authorisation URL manually, approve access, then paste the **full redirect URL** from the browser address bar into the console when prompted.

On any platform, if the local listener cannot start (port in use, permissions, remote machine), use the same manual paste flow.

---

## 4. Run all examples (smoke test)

Use `--run-all` to execute every registered example without the interactive menu. Output follows a `dotnet test`-style report with pass, fail, and skip counts.

```bash
dotnet run --project samples/FreeAgent.Client.ConsoleSample -- --run-all
```

Filter to one category:

```bash
dotnet run --project samples/FreeAgent.Client.ConsoleSample -- --run-all --category Contacts
```

### Non-interactive authentication

`--run-all` requires a token without browser interaction. Use a **refresh token** for CI and any multi-example run:

```bash
export FREEAGENT_REFRESH_TOKEN="your-refresh-token"
```

An access token is supported for quick one-off runs only; it cannot refresh, so a long `--run-all` batch may fail once the token expires:

```bash
export FREEAGENT_ACCESS_TOKEN="your-access-token"
```

Client ID and secret are still required (user secrets or environment variables).

`--category` requires `--run-all` (for example `--run-all --category Contacts`).

Examples marked `ExcludeFromRunAll` (such as **Stream all contacts**) appear in the interactive menu only.

### CI integration

The repository CI workflow includes a smoke step that runs `--run-all` when `FREEAGENT_REFRESH_TOKEN` is configured. Configure these GitHub secrets to enable live API smoke coverage:

- `FREEAGENT_CLIENT_ID`
- `FREEAGENT_CLIENT_SECRET`
- `FREEAGENT_REFRESH_TOKEN`

When the refresh token secret is absent, the step exits successfully without running examples so PRs are not blocked. The smoke run fails if no examples pass.

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
dotnet add package FreeAgent.Client --version 0.1.0-alpha.3
```

---

## Related documentation

- [Getting started](../../docs/tutorial/getting-started.md)
- [Prerelease and OAuth scope](../../docs/explanation/prerelease-and-oauth.md)
- [Blazor sample](../FreeAgent.Client.BlazorSample/) - interactive SDK probe workbench
