---
title: CIS settings
parent: API coverage
nav_order: 22
---

# CIS settings

Get and update Construction Industry Scheme settings for the company.

| | |
|---|---|
| **SDK service** | `client.CisSettings` (`CisSettingsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/cis_settings](https://dev.freeagent.com/docs/cis_settings) |
| **Sample app** | `/cis-settings` (get), `/cis-settings/detail` (update) |

## Methods

### GetCisSettingsAsync

```csharp
Task<CisSettings> GetCisSettingsAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/cis_settings`

---

### UpdateCisSettingsAsync

```csharp
Task<CisSettings> UpdateCisSettingsAsync(
    CisSettingsUpdate update,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/cis_settings`

Unspecified sections are omitted from the request. Use `PatchValue.Set<T>(null)` to deregister a section or clear nested `prior_deductions`.

**Sample (deregister contractor):**

```csharp
using FreeAgent.Client.Models.CisSettings;
using FreeAgent.Client.Models.Shared;

var settings = await client.CisSettings.UpdateCisSettingsAsync(new CisSettingsUpdate
{
    ContractorDetails = PatchValue.Set<CisContractorDetails>(null)
});
```

## Models

`CisSettings` exposes `ContractorDetails` and `SubcontractorDetails` (each `null` when not registered). Subcontractor settings include `cis_deduction_rates` (`CisDeductionRate`) and optional `prior_deductions`.
