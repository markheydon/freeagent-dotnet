---
title: Bank feeds
parent: API coverage
nav_order: 8.41
---

# Bank feeds

Read-only access to bank feeds that import transactions into [bank accounts](bank-accounts.md).

| | |
|---|---|
| **SDK service** | `client.BankFeeds` (`BankFeedsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/bank_feeds](https://dev.freeagent.com/docs/bank_feeds) |
| **Sample app** | `/bank-feeds` (list), `/bank-feeds/detail` (read-only GET) |

Connecting or refreshing a feed (including third-party bank aggregator OAuth) is outside the SDK scope.

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<BankFeed>> ListAsync(
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bank_feeds`

**Returns:** All bank feeds for the company (not paginated).

**Sample:**

```csharp
var bankFeeds = await client.BankFeeds.ListAsync();
```

---

### GetBankFeedAsync

```csharp
Task<BankFeed> GetBankFeedAsync(long bankFeedId, CancellationToken cancellationToken = default)

Task<BankFeed> GetBankFeedAsync(
    long bankFeedId,
    BankFeedGetOptions? options,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `bankFeedId` | `long` | Yes | - | path `:id` |
| `options` | `BankFeedGetOptions?` | No | `null` | - |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/bank_feeds/:id`

When `options.IncludeBankAccount` is `true`, the SDK fetches the bank account when the response contains only a URI.

**Sample:**

```csharp
var bankFeed = await client.BankFeeds.GetBankFeedAsync(7);
```

---

## Models

### BankFeed

| Property | Type | Wire field |
|----------|------|------------|
| `Url` | `string` | `url` |
| `ResourceId` | `long` | parsed from `url` |
| `BankAccount` | `BankAccount?` | nested `bank_account` when expanded |
| `BankAccountId` | `long?` | `bank_account` URI |
| `State` | `BankFeedState?` | `state` |
| `CreatedAt` | `DateTimeOffset?` | `created_at` |
| `UpdatedAt` | `DateTimeOffset?` | `updated_at` |
| `FeedType` | `BankFeedType?` | `feed_type` |
| `BankServiceName` | `string?` | `bank_service_name` |
| `ScaExpiresAt` | `DateTimeOffset?` | `sca_expires_at` (API feeds only) |

### BankFeedType

| Member | Wire value |
|--------|------------|
| `Api` | `api` |
| `OpenBanking` | `open_banking` |
