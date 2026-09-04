# API coverage

This reference lists FreeAgent API resources implemented in the SDK today. It reflects **actual code**, not planned work.

For how resources relate to each other and a suggested implementation order, see the [API entity map](../explanation/api-entity-map.md).

Source: [`src/FreeAgent.Client/Services/`](../../src/FreeAgent.Client/Services/)

## Company

Docs: https://dev.freeagent.com/docs/company  
Sample: `/company`, `/company/business-categories`, `/company/tax-timeline`

| Method | SDK API |
|--------|---------|
| `GET /v2/company` | `client.Company.GetCompanyAsync()` |
| `GET /v2/company/business_categories` | `client.Company.GetBusinessCategoriesAsync()` |
| `GET /v2/company/tax_timeline` | `client.Company.GetTaxTimelineAsync()` |

## Contacts

Docs: https://dev.freeagent.com/docs/contacts  
Sample: `/contacts` (list + per-row mapping), `/contacts/detail` (CRUD + seed data)

| Method | SDK API |
|--------|---------|
| `GET /v2/contacts` | `client.Contacts.GetContactsPageAsync(...)` |
| Auto-pagination | `client.Contacts.GetAllContactsAsync(...)` |
| `GET /v2/contacts/:id` | `client.Contacts.GetContactAsync(id)` |
| `POST /v2/contacts` | `client.Contacts.CreateContactAsync(contact)` |
| `PUT /v2/contacts/:id` | `client.Contacts.UpdateContactAsync(id, contact)` |
| `DELETE /v2/contacts/:id` | `client.Contacts.DeleteContactAsync(id)` |

### List query parameters

- `view` — use constants on `ContactViews` (for example `ContactViews.Active`, `ContactViews.Clients`)
- `sort` — `name`, `created_at`, `updated_at`; prefix with `-` for descending
- `updated_since` — ISO 8601 timestamp (`DateTimeOffset`)

## Categories

Docs: https://dev.freeagent.com/docs/categories  
Sample: `/categories` (list + per-row mapping), `/categories/detail` (CRUD)

| Method | SDK API |
|--------|---------|
| `GET /v2/categories` | `client.Categories.GetCategoriesAsync(...)` |
| `GET /v2/categories/:nominal_code` | `client.Categories.GetCategoryAsync(nominalCode)` |
| `POST /v2/categories` | `client.Categories.CreateCategoryAsync(category)` |
| `PUT /v2/categories/:nominal_code` | `client.Categories.UpdateCategoryAsync(nominalCode, category)` |
| `DELETE /v2/categories/:nominal_code` | `client.Categories.DeleteCategoryAsync(nominalCode)` |

### List query parameters

- `sub_accounts` — when `true`, includes sub accounts instead of top-level accounts where they exist

## OAuth (protocol helpers)

| Capability | SDK API |
|------------|---------|
| Authorisation URL | `FreeAgentOAuthClient.GetAuthorizationUrl(...)` |
| Code exchange | `FreeAgentOAuthClient.ExchangeCodeForTokenAsync(...)` |
| Token refresh | `FreeAgentOAuthClient.RefreshTokenAsync(...)` |

## Not yet implemented

- Invoices and other MVP resources listed in [SCOPE.md](../../SCOPE.md) — tracked via GitHub Issues
