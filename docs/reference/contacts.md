---
title: Contacts
parent: API coverage
nav_order: 2
---

# Contacts

List, create, update, and delete contacts.

| | |
|---|---|
| **SDK service** | `client.Contacts` (`ContactService`) |
| **FreeAgent docs** | [dev.freeagent.com/docs/contacts](https://dev.freeagent.com/docs/contacts) |
| **Sample app** | `/contacts` (list), `/contacts/detail` (CRUD) |

Pagination patterns: see [Pagination](../how-to/pagination.md).

## Methods

### ListAsync

```csharp
Task<PaginatedResponse<Contact>> ListAsync(
    int page = 1,
    int perPage = 25,
    ContactView view = ContactView.Active,
    ApiSort<ContactSortField>? sort = null,
    DateTimeOffset? updatedSince = null,
    CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `page` | `int` | No | `1` | `page` |
| `perPage` | `int` | No | `25` | `per_page` (max `100`) |
| `view` | `ContactView` | No | `ContactView.Active` | `view` |
| `sort` | `ApiSort<ContactSortField>?` | No | `null` | `sort` |
| `updatedSince` | `DateTimeOffset?` | No | `null` | `updated_since` (ISO 8601) |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/contacts`

**Returns:** One page of contacts.

**Client validation:** `page` ≥ 1; `perPage` between 1 and 100; `view` must not be blank.

**Sample:**

```csharp
var page = await client.Contacts.ListAsync(
    page: 1,
    perPage: 25,
    view: ContactView.Clients,
    sort: new ApiSort<ContactSortField>(ContactSortField.UpdatedAt, descending: true));
```

---

### ListAutoPagingAsync

```csharp
IAsyncEnumerable<Contact> ListAutoPagingAsync(
    int perPage = 25,
    ContactView view = ContactView.Active,
    ApiSort<ContactSortField>? sort = null,
    DateTimeOffset? updatedSince = null,
    CancellationToken cancellationToken = default)
```

Same query parameters as `ListAsync` except `page` - the SDK advances pages automatically.

**Sample:**

```csharp
await foreach (var contact in client.Contacts.ListAutoPagingAsync(perPage: 50))
{
    Console.WriteLine(contact.DisplayName);
}
```

---

### GetContactAsync

```csharp
Task<Contact> GetContactAsync(long contactId, CancellationToken cancellationToken = default)
Task<Contact> GetContactAsync(ContactReference contact, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `contactId` / `contact` | `long` / `ContactReference` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `GET /v2/contacts/:id`

**Sample:**

```csharp
var contact = await client.Contacts.GetContactAsync(123);
```

---

### CreateContactAsync

```csharp
Task<Contact> CreateContactAsync(Contact contact, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `contact` | `Contact` | Yes | - | `contact` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `POST /v2/contacts`

**Sample:**

```csharp
var created = await client.Contacts.CreateContactAsync(new Contact
{
    FirstName = "Ada",
    LastName = "Lovelace",
    Email = "ada@example.com"
});
```

---

### UpdateContactAsync

```csharp
Task<Contact> UpdateContactAsync(long contactId, Contact contact, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `contactId` | `long` | Yes | - | path `:id` |
| `contact` | `Contact` | Yes | - | `contact` envelope |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `PUT /v2/contacts/:id`

---

### DeleteContactAsync

```csharp
Task DeleteContactAsync(long contactId, CancellationToken cancellationToken = default)
```

| Parameter | Type | Required | Default | Wire |
|-----------|------|----------|---------|------|
| `contactId` | `long` | Yes | - | path `:id` |
| `cancellationToken` | `CancellationToken` | No | `default` | - |

**HTTP:** `DELETE /v2/contacts/:id`

## List filters

### view (`ContactView`)

| Constant | Wire value |
|----------|------------|
| `ContactView.All` | `all` |
| `ContactView.Active` | `active` |
| `ContactView.Clients` | `clients` |
| `ContactView.Suppliers` | `suppliers` |
| `ContactView.ActiveProjects` | `active_projects` |
| `ContactView.CompletedProjects` | `completed_projects` |
| `ContactView.OpenClients` | `open_clients` |
| `ContactView.OpenSuppliers` | `open_suppliers` |
| `ContactView.Hidden` | `hidden` |

### sort (`ApiSort<ContactSortField>`)

Use `ContactSortField.Name`, `ContactSortField.CreatedAt`, or `ContactSortField.UpdatedAt`. Pass `descending: true` on `ApiSort<T>` for descending order.

## Contact model

Key read/write properties:

| Property | Type | Wire name |
|----------|------|-----------|
| `FirstName` | `string?` | `first_name` |
| `LastName` | `string?` | `last_name` |
| `OrganisationName` | `string?` | `organisation_name` |
| `Email` | `string?` | `email` |
| `BillingEmail` | `string?` | `billing_email` |
| `PhoneNumber` | `string?` | `phone_number` |
| `Mobile` | `string?` | `mobile` |
| `Status` | `ContactStatus?` | `status` |
| `Locale` | `ContactLocale?` | `locale` |
| `ChargeSalesTax` | `ChargeSalesTax?` | `charge_sales_tax` |
| `Url` | `string` | `url` |

See IntelliSense for address fields, direct debit mandate, CIS settings, and other documented attributes.

## Errors

Failures throw [`FreeAgentApiException`](../how-to/error-handling.md). Rate limiting surfaces as [`FreeAgentRateLimitException`](../how-to/error-handling.md).
