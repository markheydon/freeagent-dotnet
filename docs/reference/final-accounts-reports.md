---
title: Final accounts reports
parent: API coverage
nav_order: 31
---

# Final accounts reports

Read final accounts report periods for a company, including filing deadlines and status. Mark reports as filed or unfiled when your OAuth access level allows.

| | |
|---|---|
| **SDK service** | `client.FinalAccountsReports` (`FinalAccountsReportsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/final_accounts_reports](https://dev.freeagent.com/docs/final_accounts_reports) |
| **Sample app** | `/final-accounts-reports`, `/final-accounts-reports/detail` |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<FinalAccountsReport>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/final_accounts_reports`

### GetAsync

```csharp
Task<FinalAccountsReport> GetAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/final_accounts_reports/:period_ends_on`

### MarkAsFiledAsync

```csharp
Task<FinalAccountsReport> MarkAsFiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/final_accounts_reports/:period_ends_on/mark_as_filed`

Requires **Full Access** (non practice-managed) or **Account Manager** (practice-managed), per FreeAgent docs.

### MarkAsUnfiledAsync

```csharp
Task<FinalAccountsReport> MarkAsUnfiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/final_accounts_reports/:period_ends_on/mark_as_unfiled`

## Models

`FinalAccountsReport` includes period dates, `filing_due_on`, filing metadata (`filing_status`, `filed_at`, `filed_reference`), and the resource `url`.

`FinalAccountsReportFilingStatus` — `draft`, `unfiled`, `pending`, `rejected`, `filed`, `marked_as_filed`

## Sandbox note

Final accounts reports apply to UK limited companies with appropriate access. Sole traders and other company types may return an empty list; sample apps document this and skip mutating examples when unavailable.
