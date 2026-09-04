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
| `POST /v2/categories` (income) | `client.Categories.CreateIncomeCategoryAsync(request)` |
| `POST /v2/categories` (cost of sales) | `client.Categories.CreateCostOfSalesCategoryAsync(request)` |
| `POST /v2/categories` (admin expenses) | `client.Categories.CreateAdminExpensesCategoryAsync(request)` |
| `POST /v2/categories` (current asset) | `client.Categories.CreateCurrentAssetCategoryAsync(request)` |
| `POST /v2/categories` (liabilities) | `client.Categories.CreateLiabilitiesCategoryAsync(request)` |
| `POST /v2/categories` (equity) | `client.Categories.CreateEquityCategoryAsync(request)` |
| `PUT /v2/categories/:nominal_code` (income) | `client.Categories.UpdateIncomeCategoryAsync(nominalCode, request)` |
| `PUT /v2/categories/:nominal_code` (cost of sales) | `client.Categories.UpdateCostOfSalesCategoryAsync(nominalCode, request)` |
| `PUT /v2/categories/:nominal_code` (admin expenses) | `client.Categories.UpdateAdminExpensesCategoryAsync(nominalCode, request)` |
| `PUT /v2/categories/:nominal_code` (current asset) | `client.Categories.UpdateCurrentAssetCategoryAsync(nominalCode, request)` |
| `PUT /v2/categories/:nominal_code` (liabilities) | `client.Categories.UpdateLiabilitiesCategoryAsync(nominalCode, request)` |
| `PUT /v2/categories/:nominal_code` (equity) | `client.Categories.UpdateEquityCategoryAsync(nominalCode, request)` |
| `DELETE /v2/categories/:nominal_code` | `client.Categories.DeleteCategoryAsync(nominalCode)` |

### List query parameters

- `sub_accounts` — when `true`, includes sub accounts instead of top-level accounts where they exist

### Write request factories

Category create/update requests use static factory methods. Variants that require `tax_reporting_name` expose discriminator-specific factories (for example `CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(..., UkLimitedCompanyCostOfSalesTaxReportingName.Purchases)`). Income and equity variants use `Create(...)`.

## Users

Docs: https://dev.freeagent.com/docs/users  
Sample: `/users` (list + per-row mapping), `/users/detail` (CRUD + personal profile)

| Method | SDK API |
|--------|---------|
| `GET /v2/users` | `client.Users.GetUsersAsync(...)` |
| `GET /v2/users/:id` | `client.Users.GetUserAsync(id)` |
| `GET /v2/users/me` | `client.Users.GetCurrentUserAsync()` |
| `POST /v2/users` | `client.Users.CreateUserAsync(user)` |
| `PUT /v2/users/:id` | `client.Users.UpdateUserAsync(id, user)` |
| `PUT /v2/users/me` | `client.Users.UpdateCurrentUserAsync(user)` |
| `DELETE /v2/users/:id` | `client.Users.DeleteUserAsync(id)` |

### List query parameters

- `view` — use constants on `UserViews` (for example `UserViews.All`, `UserViews.ActiveStaff`)

## OAuth (protocol helpers)

| Capability | SDK API |
|------------|---------|
| Authorisation URL | `FreeAgentOAuthClient.GetAuthorizationUrl(...)` |
| Code exchange | `FreeAgentOAuthClient.ExchangeCodeForTokenAsync(...)` |
| Token refresh | `FreeAgentOAuthClient.RefreshTokenAsync(...)` |

## Currencies (reference enum)

Docs: https://dev.freeagent.com/docs/currencies

FreeAgent documents supported ISO 4217 currency codes but does **not** expose a `/v2/currencies` REST resource. The SDK provides `CurrencyCode` in `FreeAgent.Client.Models.Shared` for typed wire values on implemented models:

| Model | Property |
|-------|----------|
| `Company` | `Currency` |
| `DirectDebitMandate` | `Currency` |

There is no `CurrenciesService`. Additional resources will use `CurrencyCode` for currency fields as they are implemented.

## Email addresses

Docs: https://dev.freeagent.com/docs/email_addresses  
Sample: `/email-addresses`

| Method | SDK API |
|--------|---------|
| `GET /v2/email_addresses` | `client.EmailAddresses.GetEmailAddressesAsync()` |

## Not yet implemented

- Invoices and other MVP resources listed in [SCOPE.md](../../SCOPE.md) — tracked via GitHub Issues
