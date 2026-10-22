---
title: Sales
parent: API entity map
nav_order: 2
---

# Sales

Customer-facing documents: invoices, estimates, recurring invoices, credit notes, and stock referenced on invoice lines.

[← Back to entity map hub](../api-entity-map.md)

## Relationship diagram

```mermaid
flowchart TB
  Contact[Contact]
  Project[Project]
  BankAccount[Bank_account]
  RecurringInvoice[Recurring_invoice]
  Property[Property]
  Invoice[Invoice]
  InvoiceItem[Invoice_item]
  Estimate[Estimate]
  EstimateItem[Estimate_item]
  CreditNote[Credit_note]
  CreditNoteReconciliation[Credit_note_reconciliation]
  StockItem[Stock_item]
  Category[Category]
  Timeslip[Timeslip]
  Contact -->|contact| Invoice
  Contact -->|contact| Estimate
  Contact -->|contact| RecurringInvoice
  Contact -->|contact| CreditNote
  Invoice -.->|project| Project
  Invoice -.->|bank_account| BankAccount
  Invoice -.->|property| Property
  Invoice -.->|recurring_invoice| RecurringInvoice
  Invoice -->|invoice_items| InvoiceItem
  InvoiceItem -.->|category| Category
  InvoiceItem -.->|project| Project
  InvoiceItem -.->|stock_item| StockItem
  Estimate -.->|project| Project
  Estimate -.->|property| Property
  Estimate -->|estimate_items| EstimateItem
  EstimateItem -.->|category| Category
  Estimate -.->|invoice| Invoice
  CreditNote -.->|project| Project
  CreditNote -.->|bank_account| BankAccount
  CreditNoteReconciliation -->|invoice| Invoice
  CreditNoteReconciliation -->|credit_note| CreditNote
  Timeslip -.->|billed_on_invoice| Invoice
  StockItem -.->|cost_of_sale_category| Category
```

Invoice items and credit note items are nested on their parent resource pages (no separate top-level docs URL for invoice items). Estimate items have a dedicated endpoint.

## Resource catalogue

| Resource | Official docs | SDK |
|----------|---------------|-----|
| Invoices | [Invoices](https://dev.freeagent.com/docs/invoices) | `client.Invoices` |
| Estimates | [Estimates](https://dev.freeagent.com/docs/estimates) | `client.Estimates` |
| Recurring invoices | [Recurring invoices](https://dev.freeagent.com/docs/recurring_invoices) | `client.RecurringInvoices` |
| Credit notes | [Credit notes](https://dev.freeagent.com/docs/credit_notes) | `client.CreditNotes` |
| Credit note reconciliations | [Credit note reconciliations](https://dev.freeagent.com/docs/credit_note_reconciliations) | `client.CreditNoteReconciliations` |
| Stock items | [Stock items](https://dev.freeagent.com/docs/stock_items) | `client.StockItems` |
| Price list items | [Price list items](https://dev.freeagent.com/docs/price_list_items) | `client.PriceListItems` |
| EC VAT MOSS rates | [Sales tax and VAT](https://dev.freeagent.com/docs/sales_tax) | `client.EcMoss` |

## Sales tax and VAT

Cross-cutting rules are in [Sales tax and VAT](https://dev.freeagent.com/docs/sales_tax). On this cluster:

- Invoice, estimate, and credit note line items use `sales_tax_rate`; contact `charge_sales_tax` applies when a line rate is omitted.
- When `ec_status` is `EC VAT MOSS`, set `place_of_supply` to `EcMossPlaceOfSupply` on the parent document.
- Fetch MOSS rate bands with `client.EcMoss.ListSalesTaxRatesAsync(country, datedOn)`.

## Key URI fields

### Invoice

| Field | Target | Required on create? |
|-------|--------|---------------------|
| `contact` | Contact | Yes |
| `project` | Project | No |
| `bank_account` | Bank account | No |
| `property` | Property | No (landlord companies only) |
| `recurring_invoice` | Recurring invoice | Read-only when generated |

Invoice items (`item_type` includes Hours, Expenses, Bills, Stock, and others) may set `category`, `project`, and `stock_item`.

List filters: `?contact=`, `?project=`

### Estimate

| Field | Target | Required on create? |
|-------|--------|---------------------|
| `contact` | Contact | Yes |
| `project` | Project | No |
| `property` | Property | No (landlord companies only) |

List filters: `?contact=`, `?project=`, `?invoice=` (estimates linked to an invoice)

### Recurring invoice

Inherits invoice attributes plus scheduling fields. Requires `contact` on create.

List filter: `?contact=`

### Credit note

| Field | Target | Required on create? |
|-------|--------|---------------------|
| `contact` | Contact | Yes |
| `project` | Project | No |
| `bank_account` | Bank account | No |
| `property` | Property | No (landlord companies only) |

List filters: `?contact=`, `?project=`

### Credit note reconciliation

| Field | Target | Required on create? |
|-------|--------|---------------------|
| `invoice` | Invoice | Yes |
| `credit_note` | Credit note | Yes |

## Cross-cluster inbound links

Resources in other clusters that point **to** invoices:

| From cluster | Resource | Field |
|--------------|----------|-------|
| Projects and time | Timeslip | `billed_on_invoice` |
| Purchases | Bill | `rebilled_on_invoice_item` |
| Purchases | Expense | `rebilled_on_invoice` |
| Banking | Bank transaction explanation | `paid_invoice` (invoice receipt / credit note refund) |

## SDK sequencing note

**Invoices** need only [Contacts](foundations.md) on create. [Categories](foundations.md), [Projects](projects-and-time.md), credit notes, reconciliations, stock items, price list items, and [bank accounts](banking.md) are implemented for remittance links (`BankAccountId` on writes; `BankAccountReference` for inbound URI parsing).

## Sales cluster SDK patterns

Invoices establishes patterns used across the Sales cluster:

| Pattern | SDK surface | Sibling resources |
|---------|-------------|-------------------|
| Status transitions | `Mark*Async` methods calling `PUT .../transitions/{action}` | Estimates, credit notes |
| Nested line items | Add/update/delete via parent `Update*Async` payload (`id`, `_destroy`) | Estimates, credit notes |
| PDF download | `Get*PdfAsync` decoding `{ pdf: { content } }` base64 wrapper | Estimates |
| Email action | `Send*EmailAsync` with typed email request | Estimates |
| Company default text | `Get/Update/DeleteDefaultAdditionalTextAsync` under resource path | Estimates (if documented) |
| Linked references | `ContactReference`, `ProjectReference`, `PropertyReference`, `CategoryReference`, `BankAccountReference`; set optional links with `ContactId`, `ProjectId`, `PropertyId`, and line-item `CategoryNominalCode` | All Sales documents |
