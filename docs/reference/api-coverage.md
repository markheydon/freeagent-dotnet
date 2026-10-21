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
| [Estimates](estimates.md) | `client.Estimates` | [Estimates](https://dev.freeagent.com/docs/estimates) |
| [Recurring invoices](recurring-invoices.md) | `client.RecurringInvoices` | [Recurring invoices](https://dev.freeagent.com/docs/recurring_invoices) |
| [Credit notes](credit-notes.md) | `client.CreditNotes` | [Credit notes](https://dev.freeagent.com/docs/credit_notes) |
| [Credit note reconciliations](credit-note-reconciliations.md) | `client.CreditNoteReconciliations` | [Credit note reconciliations](https://dev.freeagent.com/docs/credit_note_reconciliations) |
| [Bills](bills.md) | `client.Bills` | [Bills](https://dev.freeagent.com/docs/bills) |
| [Capital asset types](capital-asset-types.md) | `client.CapitalAssetTypes` | [Capital asset types](https://dev.freeagent.com/docs/capital_asset_types) |
| [CIS bands](cis-bands.md) | `client.CisBands` | [CIS bands](https://dev.freeagent.com/docs/cis_bands) |
| [CIS settings](cis-settings.md) | `client.CisSettings` | [CIS settings](https://dev.freeagent.com/docs/cis_settings) |
| [Profit and loss](profit-and-loss.md) | `client.ProfitAndLoss` | [Profit and loss](https://dev.freeagent.com/docs/profit_and_loss) |
| [Balance sheet](balance-sheet.md) | `client.BalanceSheet` | [Balance sheet](https://dev.freeagent.com/docs/balance_sheet) |
| [Trial balance](trial-balance.md) | `client.TrialBalance` | [Trial balance](https://dev.freeagent.com/docs/trial_balance) |
| [Cashflow](cashflow.md) | `client.Cashflow` | [Cashflow](https://dev.freeagent.com/docs/cashflow) |
| [Transactions](transactions.md) | `client.Transactions` | [Transactions](https://dev.freeagent.com/docs/transactions) |
| [VAT returns](vat-returns.md) | `client.VatReturns` | [VAT returns](https://dev.freeagent.com/docs/vat_returns) |
| [Corporation tax returns](corporation-tax-returns.md) | `client.CorporationTaxReturns` | [Corporation tax returns](https://dev.freeagent.com/docs/corporation_tax_returns) |
| [Depreciation profiles](depreciation-profiles.md) | `CapitalAssetPurchaseWrite` / `DepreciationProfile` | [Depreciation profiles](https://dev.freeagent.com/docs/depreciation_profiles) |
| [Bank accounts](bank-accounts.md) | `client.BankAccounts` | [Bank accounts](https://dev.freeagent.com/docs/bank_accounts) |
| [Bank feeds](bank-feeds.md) | `client.BankFeeds` | [Bank feeds](https://dev.freeagent.com/docs/bank_feeds) |
| [Bank transaction explanations](bank-transaction-explanations.md) | `client.BankTransactionExplanations` | [Bank transaction explanations](https://dev.freeagent.com/docs/bank_transaction_explanations) |
| [Bank transactions](bank-transactions.md) | `client.BankTransactions` | [Bank transactions](https://dev.freeagent.com/docs/bank_transactions) |
| [Hire purchases](hire-purchases.md) | `client.HirePurchases` | [Hire purchases](https://dev.freeagent.com/docs/hire_purchases) |
| [Expenses](expenses.md) | `client.Expenses` | [Expenses](https://dev.freeagent.com/docs/expenses) |
| [Attachments](attachments.md) | `client.Attachments` | [Attachments](https://dev.freeagent.com/docs/attachments) |
| [Stock items](stock-items.md) | `client.StockItems` | [Stock items](https://dev.freeagent.com/docs/stock_items) |
| [Price list items](price-list-items.md) | `client.PriceListItems` | [Price list items](https://dev.freeagent.com/docs/price_list_items) |
| [Tasks](tasks.md) | `client.ProjectTasks` | [Tasks](https://dev.freeagent.com/docs/tasks) |
| [Timeslips](timeslips.md) | `client.Timeslips` | [Timeslips](https://dev.freeagent.com/docs/timeslips) |
| [Notes](notes.md) | `client.Notes` | [Notes](https://dev.freeagent.com/docs/notes) |
| [Email addresses](email-addresses.md) | `client.EmailAddresses` | [Email addresses](https://dev.freeagent.com/docs/email_addresses) |
| [OAuth](oauth.md) | `FreeAgentOAuthClient` | [OAuth](https://dev.freeagent.com/docs/oauth) |
| [Currency code](currency-code.md) | `CurrencyCode` enum | [Currencies](https://dev.freeagent.com/docs/currencies) |

## Not yet implemented

Resources described in the [API entity map](../explanation/api-entity-map.md) but not yet implemented in the SDK are tracked via [GitHub Issues](https://github.com/markheydon/freeagent-dotnet/issues).
