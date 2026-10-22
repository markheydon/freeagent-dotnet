---
title: Account locks
parent: API coverage
nav_order: 3
---

# Account locks

List company account locks and manage the user account lock. Minimum access level for writes is **Full Access**.

| | |
|---|---|
| **SDK service** | `client.AccountLocks` (`AccountLocksService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/account_locks](https://dev.freeagent.com/docs/account_locks) |
| **Sample app** | `/account-locks` (list), `/account-locks/detail` (user lock PUT/DELETE) |

## Methods

### ListAsync

```csharp
Task<AccountLocksListResult> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/account_locks`

Returns `AccountLocks` plus optional `EarliestLockDate` and `LatestLockDate` when the API allows setting a user lock.

### SetUserAccountLockAsync

```csharp
Task SetUserAccountLockAsync(DateOnly lockedToDate, CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/account_locks`

Request body:

```json
{
  "account_lock": {
    "locked_to_date": "2024-01-01"
  }
}
```

**Response:** `204 No Content`

### DeleteUserAccountLockAsync

```csharp
Task DeleteUserAccountLockAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/account_locks`

**Response:** `204 No Content`

## Models

`AccountLock` exposes `LockedToDate`, `UserLock`, `LockedByDescription`, and optional `LockedByUserId` parsed from `locked_by_url`.
