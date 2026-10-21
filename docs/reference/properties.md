---
title: Properties
parent: API coverage
nav_order: 21
---

# Properties

List, create, update, and delete landlord rental property addresses. Properties apply only when the company type is `UkUnincorporatedLandlord`.

| | |
|---|---|
| **SDK service** | `client.Properties` (`PropertiesService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/properties](https://dev.freeagent.com/docs/properties) |
| **Sample app** | `/properties` (list), `/properties/detail` (CRUD) |

## Methods

### ListAsync

```csharp
Task<IReadOnlyList<LandlordProperty>> ListAsync(CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/properties`

**Returns:** All properties for the company (not paginated).

---

### GetPropertyAsync

```csharp
Task<LandlordProperty> GetPropertyAsync(long propertyId, CancellationToken cancellationToken = default)
```

**HTTP:** `GET /v2/properties/:id`

---

### CreatePropertyAsync

```csharp
Task<LandlordProperty> CreatePropertyAsync(
    CreatePropertyRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `POST /v2/properties`

Required request field: `address1`.

**Sample:**

```csharp
using FreeAgent.Client.Models.Properties;

var property = await client.Properties.CreatePropertyAsync(new CreatePropertyRequest
{
    Address1 = "221B Baker Street",
    Town = "London",
    Postcode = "NW1 6XE"
});
```

---

### UpdatePropertyAsync

```csharp
Task<LandlordProperty> UpdatePropertyAsync(
    long propertyId,
    UpdatePropertyRequest request,
    CancellationToken cancellationToken = default)
```

**HTTP:** `PUT /v2/properties/:id`

---

### DeletePropertyAsync

```csharp
Task DeletePropertyAsync(long propertyId, CancellationToken cancellationToken = default)
```

**HTTP:** `DELETE /v2/properties/:id`

## Models

| Type | Role |
|------|------|
| `LandlordProperty` | Read model (`url`, address fields, `country`) |
| `CreatePropertyRequest` | Create payload (address attributes; `address1` required) |
| `UpdatePropertyRequest` | Update payload (address attributes to change) |
| `PropertyReference` | Typed URI reference for linked resources (`client.Urls.Property(id)`) |

## Sandbox and smoke notes

The console sample and `--run-all` smoke harness **skip** property examples when the connected company is not `UkUnincorporatedLandlord`. The Blazor probe pages show a company-type warning in that case.
