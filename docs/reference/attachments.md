---
title: Attachments
parent: API coverage
nav_order: 8.35
---

# Attachments

Fetch or delete attachment metadata and signed download URLs by identifier. Attachments are polymorphic: minimum access level matches the parent resource (for example an expense or bank transaction explanation).

| | |
|---|---|
| **SDK service** | `client.Attachments` (`AttachmentsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/attachments](https://dev.freeagent.com/docs/attachments) |
| **Sample app** | `/attachments/detail` (GET and DELETE) |

Upload flows are documented on parent resources (for example [Expenses](expenses.md) and [Bills](bills.md)). Purchases cluster context: see [Purchases entity map](../explanation/api-entity-map/purchases.md).

## Methods

### GetAttachmentAsync

```csharp
Task<Attachment> GetAttachmentAsync(
    long attachmentId,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `attachmentId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/attachments/:id`

**Sample:**

```csharp
var attachment = await client.Attachments.GetAttachmentAsync(3);
```

---

### DeleteAttachmentAsync

```csharp
Task DeleteAttachmentAsync(
    long attachmentId,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `attachmentId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/attachments/:id`

**Sample:**

```csharp
await client.Attachments.DeleteAttachmentAsync(3);
```

## Models

### Attachment

| Property | Type | Wire field | Notes |
|----------|------|------------|-------|
| `Url` | `string` | `url` | Resource URI |
| `ResourceId` | `long` | - | Parsed from `url` |
| `ContentSrc` | `string?` | `content_src` | Full-size file URL |
| `ContentSrcMedium` | `string?` | `content_src_medium` | Medium thumbnail when present |
| `ContentSrcSmall` | `string?` | `content_src_small` | Small thumbnail when present |
| `ExpiresAt` | `DateTimeOffset?` | `expires_at` | Refresh attachment after expiry |
| `ContentType` | `string?` | `content_type` | MIME type |
| `FileName` | `string?` | `file_name` | Original file name |
| `FileSize` | `int?` | `file_size` | Size in bytes |
| `Description` | `string?` | `description` | Free-text description |

### AttachmentReference

Typed URI for attachment links. Build with `client.Urls.Attachment(id)` or parse with `AttachmentReference.Parse(uri)`.
