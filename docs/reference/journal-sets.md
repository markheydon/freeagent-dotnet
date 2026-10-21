---
title: Journal sets
parent: API coverage
nav_order: 21
---

# Journal sets

List, create, update, and delete manual journal corrections, and read the opening balances journal set.

| | |
|---|---|
| **SDK service** | `client.JournalSets` (`JournalSetsService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/journal_sets](https://dev.freeagent.com/docs/journal_sets) |
| **Sample app** | `/journal-sets` (list and opening balances), `/journal-sets/detail` (CRUD) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<JournalSet>> ListAsync(
    DateOnly? fromDate = null,
    DateOnly? toDate = null,
    DateTimeOffset? updatedSince = null,
    string? tag = null,
    CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/journal_sets`

Optional query parameters: `from_date`, `to_date`, `updated_since`, `tag`.

**Returns:** All journal sets matching the filters (not paginated).

---

### GetJournalSetAsync

```csharp
Task<JournalSet> GetJournalSetAsync(long journalSetId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/journal_sets/:id`

---

### GetOpeningBalancesAsync

```csharp
Task<JournalSet> GetOpeningBalancesAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/journal_sets/opening_balances`

Returns the special opening balances journal set, including read-only `bank_accounts` and `stock_items` rows when present.

---

### CreateJournalSetAsync

```csharp
Task<JournalSet> CreateJournalSetAsync(
    CreateJournalSetRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/journal_sets`

Required request fields: `Description`, `JournalEntries` (balancing debits). Set `CategoryNominalCode`, `UserId`, `ContactId`, `StockItemId`, `CapitalAssetTypeId`, and `PropertyId` on each `JournalEntry` instead of supplying resource URIs.

**Sample:**

```csharp
using FreeAgent.Client.Models.JournalSets;

var journalSet = await client.JournalSets.CreateJournalSetAsync(new CreateJournalSetRequest
{
    Description = "Sales correction",
    DatedOn = new DateOnly(2026, 1, 15),
    Tag = "MY-APP",
    JournalEntries =
    [
        new JournalEntry { CategoryNominalCode = "001", DebitValue = -100m },
        new JournalEntry { CategoryNominalCode = "901", UserId = 1, DebitValue = 100m }
    ]
});
```

---

### UpdateJournalSetAsync

```csharp
Task<JournalSet> UpdateJournalSetAsync(
    long journalSetId,
    UpdateJournalSetRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/journal_sets/:id`

Update existing journal entries using the entry URL from GET (or `EntryId` parsed from the URL). Delete entries with `Destroy = true`. Add new entries without a URL.

---

### DeleteJournalSetAsync

```csharp
Task DeleteJournalSetAsync(long journalSetId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/journal_sets/:id`

---

## Models

| Type | Role |
|------|------|
| `JournalSet` | Journal set with `JournalEntries`, optional opening-balance `BankAccounts` / `StockItems` |
| `JournalEntry` | Line with `CategoryNominalCode`, `DebitValue`, and optional linked-resource identifiers |
| `CreateJournalSetRequest` / `UpdateJournalSetRequest` | Write contracts |
| `JournalSetOpeningBalanceBankAccount` | Read-only bank opening balance row |
| `JournalSetOpeningBalanceStockItem` | Read-only stock opening balance row |

Linked journal entry fields follow [linked resource identity](../../explanation/linked-resources.md): use `*Id` and `CategoryNominalCode` on writes; expanded models are available on reads when the API nests objects.

## Client validation

The SDK fails fast on journal entry constraints that the [official journal sets docs](https://dev.freeagent.com/docs/journal_sets) state without needing account state:

| Check | When |
|-------|------|
| At least one journal entry on create | `CreateJournalSetAsync` |
| `CategoryNominalCode` and `DebitValue` on each new line | Create; update when adding a line without a URL |
| `CapitalAssetTypeId` when the category base code is 601–607 (including sub-codes such as `602-1`) | Create and update when the effective category code is known |
| URL or parsed entry id when `Destroy` is true | `UpdateJournalSetAsync` |

Not validated locally (depends on category type or company settings; FreeAgent returns API errors instead):

- **User categories** — `user` required per docs, but which nominal codes are user categories is account-specific.
- **Stock categories** — `stock_item` and `stock_altering_quantity` required per docs, but stock categories are not identified by a documented code range on the journal sets page.
- **Opening balance contact** — `contact` for Trade Debtors (681) or Trade Creditors (796) only in the opening balances journal set.
- **Landlord property** — `property` for UK unincorporated landlord profit and loss categories.
- **Balancing debits** — journal sets must balance; that is enforced by FreeAgent, not pre-checked in the client.
