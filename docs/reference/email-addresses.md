---
title: Email addresses
parent: API coverage
nav_order: 6
---

# Email addresses

List verified sender email addresses for the authenticated company.

| | |
|---|---|
| **SDK service** | `client.EmailAddresses` (`EmailAddressesService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/email_addresses](https://dev.freeagent.com/docs/email_addresses) |
| **Sample app** | `/email-addresses` |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<string>> ListAsync(CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | — |

**HTTP:** `GET /v2/email_addresses`

**Returns:** Verified sender addresses formatted as `Name <email@example.com>`.

**Access:** Minimum FreeAgent access level — Time.

**Sample:**

```csharp
var addresses = await client.EmailAddresses.ListAsync();
foreach (var address in addresses)
{
    Console.WriteLine(address);
}
```

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md).
