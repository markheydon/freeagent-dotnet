---
title: Capital asset types
parent: API coverage
nav_order: 20
---

# Capital asset types

List, create, update, and delete capital asset type reference data used by capital assets and journal entries.

| | |
|---|---|
| **SDK service** | `client.CapitalAssetTypes` (`CapitalAssetTypesService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/capital_asset_types](https://dev.freeagent.com/docs/capital_asset_types) |
| **Sample app** | `/capital-asset-types` (list), `/capital-asset-types/detail` (CRUD) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<CapitalAssetType>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/capital_asset_types`

**Returns:** All capital asset types for the company (not paginated).

---

### GetCapitalAssetTypeAsync

```csharp
Task<CapitalAssetType> GetCapitalAssetTypeAsync(long capitalAssetTypeId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/capital_asset_types/:id`

---

### CreateCapitalAssetTypeAsync

```csharp
Task<CapitalAssetType> CreateCapitalAssetTypeAsync(
    CreateCapitalAssetTypeRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/capital_asset_types`

Required request field: `name`.

**Sample:**

```csharp
using FreeAgent.Client.Models.CapitalAssetTypes;

var type = await client.CapitalAssetTypes.CreateCapitalAssetTypeAsync(new CreateCapitalAssetTypeRequest
{
    Name = "Workshop equipment"
});
```

---

### UpdateCapitalAssetTypeAsync

```csharp
Task<CapitalAssetType> UpdateCapitalAssetTypeAsync(
    long capitalAssetTypeId,
    UpdateCapitalAssetTypeRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/capital_asset_types/:id`

Only user-created types with no linked capital asset items can be updated.

---

### DeleteCapitalAssetTypeAsync

```csharp
Task DeleteCapitalAssetTypeAsync(long capitalAssetTypeId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/capital_asset_types/:id`

Only user-created types with no linked capital asset items can be deleted.

## Models

| Type | Role |
|------|------|
| `CapitalAssetType` | Read model (`url`, `name`, `system_default`, `created_at`, `updated_at`) |
| `CreateCapitalAssetTypeRequest` | Create payload (`name`) |
| `UpdateCapitalAssetTypeRequest` | Update payload (`name`) |
| `CapitalAssetTypeReference` | Typed URI reference for linked resources |
