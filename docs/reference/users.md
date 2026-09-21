---
title: Users
parent: API coverage
nav_order: 4
---

# Users

List, create, update, and delete company users; read and update the authenticated user's profile.

| | |
|---|---|
| **SDK service** | `client.Users` (`UserService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/users](https://dev.freeagent.com/docs/users) |
| **Sample app** | `/users` (list), `/users/detail` (CRUD + personal profile) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<User>> ListAsync(
    string view = UserViews.All,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `view` | `string` | No | `UserViews.All` | `view` |
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `GET /v2/users`

**Returns:** All users for the selected view (not paginated).

**Sample:**

```csharp
using FreeAgent.Client.Models.Users;

var staff = await client.Users.ListAsync(UserViews.ActiveStaff);
```

---

### GetUserAsync

```csharp
Task<User> GetUserAsync(long userId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `userId` | `long` | Yes | — | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `GET /v2/users/:id`

---

### GetCurrentUserAsync

```csharp
Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `GET /v2/users/me`

**Access:** Minimum FreeAgent access level — Time.

**Sample:**

```csharp
var me = await client.Users.GetCurrentUserAsync();
```

---

### CreateUserAsync

```csharp
Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `user` | `User` | Yes | — | `user` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `POST /v2/users`

---

### UpdateUserAsync

```csharp
Task<User> UpdateUserAsync(long userId, User user, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `userId` | `long` | Yes | — | path `:id` |
| `user` | `User` | Yes | — | `user` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `PUT /v2/users/:id`

---

### UpdateCurrentUserAsync

```csharp
Task<User> UpdateCurrentUserAsync(User user, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `user` | `User` | Yes | — | `user` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `PUT /v2/users/me`

**Access:** Minimum FreeAgent access level — Time.

---

### DeleteUserAsync

```csharp
Task DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `userId` | `long` | Yes | — | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `DELETE /v2/users/:id`

## List filters

### view (`UserViews`)

| Constant | Wire value |
|----------|------------|
| `UserViews.All` | `all` |
| `UserViews.Staff` | `staff` |
| `UserViews.ActiveStaff` | `active_staff` |
| `UserViews.Advisors` | `advisors` |
| `UserViews.ActiveAdvisors` | `active_advisors` |

## User model

Key properties:

| Property | Type | Wire name |
|----------|------|-----------|
| `Email` | `string?` | `email` |
| `FirstName` | `string?` | `first_name` |
| `LastName` | `string?` | `last_name` |
| `PermissionLevel` | `UserPermissionLevel?` | `permission_level` |
| `Role` | `UserRole?` | `role` |
| `Hidden` | `bool?` | `hidden` |
| `Url` | `string` | `url` |

See IntelliSense for payroll profile and tax reference fields.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
