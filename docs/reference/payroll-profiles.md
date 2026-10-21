---
title: Payroll profiles
parent: API coverage
nav_order: 43
---

# Payroll profiles

Read-only payroll profiles for employees in a tax year.

| | |
|---|---|
| **SDK service** | `client.PayrollProfiles` (`PayrollProfilesService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/payroll_profiles](https://dev.freeagent.com/docs/payroll_profiles) |
| **Sample app** | `/payroll-profiles` |

Minimum access level: Tax and Limited Accounting. UK payroll companies only.

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<PayrollProfile>> ListAsync(
    int taxYearEnd,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/payroll_profiles/:year`

---

### ListProfilesForUserAsync

```csharp
Task<IReadOnlyList<PayrollProfile>> ListProfilesForUserAsync(
    int taxYearEnd,
    UserReference user,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/payroll_profiles/:year?user=:user_uri`

Pass a `UserReference` (for example from `UserReference.ForEnvironment(environment, userId)`).

---

## Models

| Type | Role |
|------|------|
| `PayrollProfile` | Employee payroll profile for a tax year |
| `PayrollProfilesListResponse` | Response envelope (`profiles` array) |

`PayrollProfile` exposes `UserId` and optional nested `User` from the `user` link. Gender remains a string because HMRC defines the allowed values outside this SDK.

The nested `current_payroll_profile` on [Users](users.md) is a smaller read-only subset when payroll is configured.

## Sample

```csharp
var taxYearEnd = 2021;
var profiles = await client.PayrollProfiles.ListAsync(taxYearEnd);

var user = UserReference.ForEnvironment(client.Environment, 107);
var userProfiles = await client.PayrollProfiles.ListProfilesForUserAsync(taxYearEnd, user);
```
