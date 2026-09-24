---
title: API coverage
nav_order: 4
has_children: true
permalink: /reference
---

# API coverage

This reference lists FreeAgent API resources implemented in the SDK today. It reflects **actual code**, not planned work.

For how resources relate to each other and a suggested implementation order, see the [API entity map](../explanation/api-entity-map.md).

## Implemented resources

| Resource | SDK entry point | FreeAgent docs |
|----------|-----------------|----------------|
| [Company](company.md) | `client.Company` | [Company](https://dev.freeagent.com/docs/company) |
| [Contacts](contacts.md) | `client.Contacts` | [Contacts](https://dev.freeagent.com/docs/contacts) |
| [Categories](categories.md) | `client.Categories` | [Categories](https://dev.freeagent.com/docs/categories) |
| [Users](users.md) | `client.Users` | [Users](https://dev.freeagent.com/docs/users) |
| [Projects](projects.md) | `client.Projects` | [Projects](https://dev.freeagent.com/docs/projects) |
| [Invoices](invoices.md) | `client.Invoices` | [Invoices](https://dev.freeagent.com/docs/invoices) |
| [Tasks](tasks.md) | `client.Tasks` | [Tasks](https://dev.freeagent.com/docs/tasks) |
| [Timeslips](timeslips.md) | `client.Timeslips` | [Timeslips](https://dev.freeagent.com/docs/timeslips) |
| [Notes](notes.md) | `client.Notes` | [Notes](https://dev.freeagent.com/docs/notes) |
| [Email addresses](email-addresses.md) | `client.EmailAddresses` | [Email addresses](https://dev.freeagent.com/docs/email_addresses) |
| [OAuth](oauth.md) | `FreeAgentOAuthClient` | [OAuth](https://dev.freeagent.com/docs/oauth) |
| [Currency code](currency-code.md) | `CurrencyCode` enum | [Currencies](https://dev.freeagent.com/docs/currencies) |

## Not yet implemented

Resources described in the [API entity map](../explanation/api-entity-map.md) but not yet implemented in the SDK are tracked via [GitHub Issues](https://github.com/markheydon/freeagent-dotnet/issues).
