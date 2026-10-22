# FreeAgent SDK Sample Apps

Samples for trying the `FreeAgent.Client` SDK against the FreeAgent API.

| Sample | Purpose | SDK source |
|--------|---------|------------|
| [FileBasedQuickStart/OAuthGetTokenPaste.cs](FileBasedQuickStart/OAuthGetTokenPaste.cs) | README Quick start step 1 — sandbox OAuth via pasted redirect URL (`FREEAGENT_CLIENT_ID`, `FREEAGENT_CLIENT_SECRET`). | Local project by default; `#:package` after NuGet install |
| [FileBasedQuickStart/CompanyHello.cs](FileBasedQuickStart/CompanyHello.cs) | README Quick start step 2 — `GetCompanyAsync` using `FREEAGENT_ACCESS_TOKEN`. | Local project by default; `#:package` after NuGet install |
| [FileBasedQuickStart/CompanyDeserializeSmoke.cs](FileBasedQuickStart/CompanyDeserializeSmoke.cs) | File-based app smoke for trim-safe JSON (fixture deserialise; optional live `GetCompanyAsync` when `FREEAGENT_ACCESS_TOKEN` is set). Sandbox smoke CI uses `#:project`. | Local project by default; `#:package` after NuGet install |
| [FreeAgent.Client.ConsoleSample](FreeAgent.Client.ConsoleSample/) | Real-world console reference and live integration harness - DI, interactive menu, `--seed-turpinverse`, `--run-all` sandbox smoke (mutates data) | Local project by default; `-p:UseLocalFreeAgentClient=false` for NuGet |
| [FreeAgent.Client.BlazorSample](FreeAgent.Client.BlazorSample/) | Blazor Server workbench for exercising SDK endpoints interactively | Local project by default; `-p:UseLocalFreeAgentClient=false` for NuGet |
| [FreeAgent.Client.Samples.Shared](FreeAgent.Client.Samples.Shared/) | Turpinverse canon loaders, seeders, and bulk seed orchestrator shared by both samples | Local project only (not published) |

---

## Console sample (real-world reference)

See [FreeAgent.Client.ConsoleSample/README.md](FreeAgent.Client.ConsoleSample/README.md) for the interactive example menu, read-only vs mutating examples, `--seed-turpinverse`, `--run-all` smoke testing, and CI token setup.

---

## Blazor sample (full workbench)

See [FreeAgent.Client.BlazorSample/README.md](FreeAgent.Client.BlazorSample/README.md) for SDK source toggle details. The sections below cover OAuth setup and running the workbench.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A FreeAgent developer account and registered OAuth app
  - Production apps: <https://dev.freeagent.com>
  - Sandbox apps: <https://dev.sandbox.freeagent.com>
- A FreeAgent **sandbox account** for OAuth approval (separate from the developer portal):
  - Sign up: <https://signup.sandbox.freeagent.com/signup>
  - Log in: <https://login.sandbox.freeagent.com/login>

---

## 1. Register a FreeAgent OAuth App

1. Sign in to the appropriate FreeAgent developer portal (sandbox recommended for development).
2. Create a new application.
3. Set the **OAuth Redirect URI** to exactly:

   ```
   https://localhost:5001/oauth/callback
   ```

   > **Important:** The redirect URI must match exactly - including the scheme (`https`), host (`localhost`), port (`:5001`), and path (`/oauth/callback`). A mismatch will cause the authorisation flow to fail.

4. Note your **OAuth identifier** and **OAuth secret**.

---

## 2. Configure Credentials via User Secrets

Never put credentials in `appsettings.json`. Use .NET user secrets instead:

```bash
cd samples/FreeAgent.Client.BlazorSample

dotnet user-secrets set "FreeAgent:ClientId"     "<your-client-id>"
dotnet user-secrets set "FreeAgent:ClientSecret" "<your-client-secret>"
```

The `FreeAgent:RedirectUri` value in `appsettings.json` defaults to `https://localhost:5001/oauth/callback` and should match what you registered in step 1. Override it via user secrets if you use a different port:

```bash
dotnet user-secrets set "FreeAgent:RedirectUri" "https://localhost:5001/oauth/callback"
```

---

## 3. Run the App

```bash
cd samples/FreeAgent.Client.BlazorSample
dotnet run
```

The app starts at `https://localhost:5001`. Your browser may show a certificate warning for the local development certificate; accept it, or run `dotnet dev-certs https --trust` first.

---

## 4. Authorize the App

1. Navigate to <https://localhost:5001>.
2. Click **Connect to FreeAgent**.
3. You are redirected to FreeAgent's authorisation page - approve the app.
4. FreeAgent redirects back to `https://localhost:5001/oauth/callback`.
5. The app exchanges the code for a token and shows **Connected** in the header.

---

## 5. Test SDK Calls

These pages exercise the SDK endpoints that exist today. Do not expect UI for resources that are not yet implemented.

| Page | SDK call tested | Probe features |
|------|-----------------|----------------|
| [/company](https://localhost:5001/company) | `CompanyService.GetCompanyAsync()` | Wire-to-model mapping, raw JSON |
| [/company/business-categories](https://localhost:5001/company/business-categories) | `CompanyService.ListBusinessCategoriesAsync()` | Per-row mapping inspection, list output |
| [/company/tax-timeline](https://localhost:5001/company/tax-timeline) | `CompanyService.ListTaxTimelineAsync()` | Per-row mapping inspection, timeline list |
| [/contacts](https://localhost:5001/contacts) | `ContactService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, list filters |
| [/contacts/detail](https://localhost:5001/contacts/detail) | `GetContactAsync()`, `CreateContactAsync()`, `UpdateContactAsync()`, `DeleteContactAsync()` | CRUD probes, Turpinverse seed data, full-detail fixture, progress bar |
| [/categories](https://localhost:5001/categories) | `CategoryService.ListAsync()` | Per-row mapping inspection, sub accounts toggle |
| [/categories/detail](https://localhost:5001/categories/detail) | `GetCategoryAsync()`, all `Create*CategoryAsync` / `Update*CategoryAsync`, `DeleteCategoryAsync` via write-variant selector | CRUD probes, all documented write variants, progress bar |
| [/projects](https://localhost:5001/projects) | `ProjectService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/sort/contact/nested filters |
| [/projects/detail](https://localhost:5001/projects/detail) | `GetProjectAsync()`, `CreateProjectAsync()`, `UpdateProjectAsync()`, `DeleteProjectAsync()` | CRUD probes, Turpinverse seed data, progress bar |
| [/invoices](https://localhost:5001/invoices) | `InvoiceService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/sort/contact/project filters |
| [/invoices/detail](https://localhost:5001/invoices/detail) | `GetInvoiceAsync()`, `CreateInvoiceAsync()`, `UpdateInvoiceAsync()`, `DeleteInvoiceAsync()`, transition and email methods | CRUD probes, Turpinverse seed data, progress bar |
| [/estimates](https://localhost:5001/estimates) | `EstimateService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/date/contact/project/invoice filters |
| [/estimates/detail](https://localhost:5001/estimates/detail) | `GetEstimateAsync()`, `CreateEstimateAsync()`, `UpdateEstimateAsync()`, `DeleteEstimateAsync()`, transition and email methods | CRUD probes, Turpinverse seed data, progress bar |
| [/recurring-invoices](https://localhost:5001/recurring-invoices) | `RecurringInvoiceService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/contact filters (read-only API) |
| [/recurring-invoices/detail](https://localhost:5001/recurring-invoices/detail) | `GetRecurringInvoiceAsync()` | GET probe, optional linked-resource hydration, progress bar |
| [/credit-notes](https://localhost:5001/credit-notes) | `CreditNotesService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/contact/project filters |
| [/credit-notes/detail](https://localhost:5001/credit-notes/detail) | `GetCreditNoteAsync()`, `CreateCreditNoteAsync()`, `UpdateCreditNoteAsync()`, `DeleteCreditNoteAsync()`, transition and email methods | CRUD probes, Turpinverse seed data, progress bar |
| [/credit-note-reconciliations](https://localhost:5001/credit-note-reconciliations) | `CreditNoteReconciliationsService.ListAsync()` | Per-row mapping inspection, date filters |
| [/credit-note-reconciliations/detail](https://localhost:5001/credit-note-reconciliations/detail) | `GetCreditNoteReconciliationAsync()`, `CreateCreditNoteReconciliationAsync()`, `UpdateCreditNoteReconciliationAsync()`, `DeleteCreditNoteReconciliationAsync()` | CRUD probes, progress bar |
| [/bills](https://localhost:5001/bills) | `BillService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/contact/project filters |
| [/bills/detail](https://localhost:5001/bills/detail) | `GetBillAsync()`, `CreateBillAsync()`, `UpdateBillAsync()`, `DeleteBillAsync()` | CRUD probes, hire purchase flag, Turpinverse seed data, progress bar |
| [/attachments/detail](https://localhost:5001/attachments/detail) | `AttachmentsService.GetAttachmentAsync()`, `DeleteAttachmentAsync()` | GET and DELETE probes, resolve ID from expense, `?id=` deep links, progress bar |
| [/hire-purchases](https://localhost:5001/hire-purchases) | `HirePurchasesService.ListAsync()` | Per-row mapping inspection, read-only list (UK hire purchase bills) |
| [/hire-purchases/detail](https://localhost:5001/hire-purchases/detail) | `HirePurchasesService.GetHirePurchaseAsync()` | GET probe, `?id=` deep links, progress bar |
| [/capital-assets](https://localhost:5001/capital-assets) | `CapitalAssetsService.ListAsync()` | Per-row mapping inspection, view and `include_history` filters |
| [/capital-assets/detail](https://localhost:5001/capital-assets/detail) | `CapitalAssetsService.GetCapitalAssetAsync()` | GET probe, optional history, `?id=` deep links, progress bar |
| [/capital-asset-types](https://localhost:5001/capital-asset-types) | `CapitalAssetTypesService.ListAsync()` | Per-row mapping inspection, list output |
| [/capital-asset-types/detail](https://localhost:5001/capital-asset-types/detail) | `GetCapitalAssetTypeAsync()`, `CreateCapitalAssetTypeAsync()`, `UpdateCapitalAssetTypeAsync()`, `DeleteCapitalAssetTypeAsync()` | CRUD probes, probe upsert by name, progress bar |
| [/journal-sets](https://localhost:5001/journal-sets) | `JournalSetsService.ListAsync()`, `GetOpeningBalancesAsync()` | Per-row mapping inspection, date/tag filters, opening balances |
| [/journal-sets/detail](https://localhost:5001/journal-sets/detail) | `GetJournalSetAsync()`, `CreateJournalSetAsync()`, `UpdateJournalSetAsync()`, `DeleteJournalSetAsync()` | CRUD probes, tagged probe upsert, balancing entry pair, progress bar |
| [/payroll](https://localhost:5001/payroll) | `ListAsync()`, `MarkPaymentAsPaidAsync()`, `MarkPaymentAsUnpaidAsync()` | Tax year end input, per-period row mapping, payment mark-as (mutates) |
| [/payroll/period](https://localhost:5001/payroll/period) | `ListPayslipsAsync()` | `?taxYearEnd=` and `?period=` deep links, per-payslip mapping |
| [/payroll-profiles](https://localhost:5001/payroll-profiles) | `ListAsync()`, `ListProfilesForUserAsync()` | Optional user id filter, per-row mapping |
| [/properties](https://localhost:5001/properties) | `PropertiesService.ListAsync()` | Per-row mapping inspection; landlord company-type warning |
| [/properties/detail](https://localhost:5001/properties/detail) | `GetPropertyAsync()`, `CreatePropertyAsync()`, `UpdatePropertyAsync()`, `DeletePropertyAsync()` | CRUD probes, stable address1 upsert, progress bar |
| [/expenses](https://localhost:5001/expenses) | `ExpenseService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view and project filters |
| [/expenses/detail](https://localhost:5001/expenses/detail) | `GetExpenseAsync()`, `CreateExpenseAsync()`, `CreateExpensesAsync()`, `UpdateExpenseAsync()`, `DeleteExpenseAsync()`, `GetMileageSettingsAsync()` | CRUD probes, category and mileage variants, batch create, progress bar |
| [/bank-accounts](https://localhost:5001/bank-accounts) | `BankAccountsService.ListAsync()` | Per-row mapping inspection, view filter |
| [/bank-accounts/detail](https://localhost:5001/bank-accounts/detail) | `GetBankAccountAsync()`, `Create*BankAccountAsync()`, `UpdateBankAccountAsync()`, `DeleteBankAccountAsync()` | CRUD probes, create variant selector, progress bar |
| [/bank-feeds](https://localhost:5001/bank-feeds) | `BankFeedsService.ListAsync()` | Per-row mapping inspection, read-only list |
| [/bank-feeds/detail](https://localhost:5001/bank-feeds/detail) | `BankFeedsService.GetBankFeedAsync()` | GET probe, `?id=` deep links, progress bar |
| [/bank-transactions](https://localhost:5001/bank-transactions) | `BankTransactionsService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, bank account and view filters |
| [/bank-transactions/detail](https://localhost:5001/bank-transactions/detail) | `GetBankTransactionAsync()`, `DeleteBankTransactionAsync()`, `UploadBankStatementAsync()`, `UploadBankStatementFileAsync()` | Get, delete, JSON and file statement uploads, progress bar |
| [/bank-transaction-explanations](https://localhost:5001/bank-transaction-explanations) | `BankTransactionExplanationsService.ListAsync()` | Per-row mapping inspection, required bank account ID, date filters |
| [/bank-transaction-explanations/detail](https://localhost:5001/bank-transaction-explanations/detail) | `GetBankTransactionExplanationAsync()`, all documented `Create*BankTransactionExplanationAsync()` variants, `UpdateBankTransactionExplanationAsync()`, `DeleteBankTransactionExplanationAsync()` | CRUD, write-variant selector, `?id=` deep links, progress bar |
| [/stock-items](https://localhost:5001/stock-items) | `StockItemsService.ListAsync()` | Per-row mapping inspection, sort filter |
| [/stock-items/detail](https://localhost:5001/stock-items/detail) | `StockItemsService.GetStockItemAsync()` | GET probe, `?id=` deep links, progress bar |
| [/price-list-items](https://localhost:5001/price-list-items) | `PriceListItemsService.ListAsync()` | Per-row mapping inspection, sort filter |
| [/price-list-items/detail](https://localhost:5001/price-list-items/detail) | `GetPriceListItemAsync()`, `CreatePriceListItemAsync()`, `UpdatePriceListItemAsync()`, `DeletePriceListItemAsync()` | CRUD probes, progress bar |
| [/tasks](https://localhost:5001/tasks) | `ProjectTaskService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/sort/project/updated_since filters |
| [/tasks/detail](https://localhost:5001/tasks/detail) | `GetProjectTaskAsync()`, `CreateProjectTaskAsync()`, `UpdateProjectTaskAsync()`, `DeleteProjectTaskAsync()` | CRUD probes, Turpinverse seed data, progress bar |
| [/timeslips](https://localhost:5001/timeslips) | `TimeslipService.ListAsync()` and `ListAutoPagingAsync()` | Per-row mapping inspection, view/date/user/task/project/nested filters |
| [/timeslips/detail](https://localhost:5001/timeslips/detail) | `GetTimeslipAsync()`, `CreateTimeslipAsync()`, `CreateTimeslipsAsync()`, `UpdateTimeslipAsync()`, `DeleteTimeslipAsync()`, `StartTimerAsync()`, `StopTimerAsync()` | CRUD + batch create variant + timer probes, Turpinverse seed data |
| [/notes](https://localhost:5001/notes) | `NoteService.ListContactNotesAsync()` and `ListProjectNotesAsync()` | Per-row mapping inspection, contact/project parent selector |
| [/notes/detail](https://localhost:5001/notes/detail) | `GetNoteAsync()`, `CreateContactNoteAsync()`, `CreateProjectNoteAsync()`, `UpdateNoteAsync()`, `DeleteNoteAsync()` | CRUD probes, create variant selector, Turpinverse seed data |
| [/users](https://localhost:5001/users) | `UserService.ListAsync()` | Per-row mapping inspection, view filters |
| [/users/detail](https://localhost:5001/users/detail) | `GetUserAsync()`, `GetCurrentUserAsync()`, `CreateUserAsync()`, `UpdateUserAsync()`, `UpdateCurrentUserAsync()`, `DeleteUserAsync()` | CRUD probes, personal profile endpoints, progress bar |
| [/email-addresses](https://localhost:5001/email-addresses) | `EmailAddressesService.ListAsync()` | Per-row mapping inspection, list output |
| [/profit-and-loss](https://localhost:5001/profit-and-loss) | `ProfitAndLossService.GetSummaryAsync()` | Date range query, wire-to-model mapping |
| [/balance-sheet](https://localhost:5001/balance-sheet) | `BalanceSheetService.GetBalanceSheetAsync()` / `GetOpeningBalancesAsync()` | As-of date query, wire-to-model mapping |
| [/trial-balance](https://localhost:5001/trial-balance) | `TrialBalanceService.ListSummaryAsync()` / `ListOpeningBalancesAsync()` | Date range query, wire-to-model mapping |
| [/cashflow](https://localhost:5001/cashflow) | `CashflowService.GetCashflowAsync()` | Date range query, wire-to-model mapping |
| [/transactions](https://localhost:5001/transactions) | `TransactionsService.ListAsync()` | Per-row mapping inspection, date and nominal-code filters |
| [/transactions/detail](https://localhost:5001/transactions/detail?id=1) | `TransactionsService.GetAccountingTransactionAsync()` | GET probe, `?id=` deep links, progress bar |
| [/vat-returns](https://localhost:5001/vat-returns) | `VatReturnsService.ListAsync()` / `GetVatReturnAsync(periodEndsOn)` | List + period detail fetch, wire-to-model mapping |
| [/corporation-tax-returns](https://localhost:5001/corporation-tax-returns) | `CorporationTaxReturnsService.ListAsync()` | Per-row mapping inspection; limited-company note when empty |
| [/corporation-tax-returns/detail](https://localhost:5001/corporation-tax-returns/detail?periodEndsOn=YYYY-MM-DD) | `GetCorporationTaxReturnAsync()`, `MarkAsFiledAsync()`, `MarkAsUnfiledAsync()`, `MarkAsPaidAsync()`, `MarkAsUnpaidAsync()` | `?periodEndsOn=` deep links, all mark-as transitions, progress bar |
| [/income-tax-returns](https://localhost:5001/income-tax-returns) | `IncomeTaxReturnsService.ListAsync(userId)` | Resolves `users/me` when `userId` omitted; per-row mapping |
| [/income-tax-returns/detail](https://localhost:5001/income-tax-returns/detail?userId=&amp;periodEndsOn=YYYY-MM-DD) | `GetIncomeTaxReturnAsync()`, `MarkAsFiledAsync()`, `MarkAsUnfiledAsync()`, `MarkPaymentAsPaidAsync()`, `MarkPaymentAsUnpaidAsync()` | User-scoped SA wire path; payment date for payment transitions |
| [/self-assessment-returns](https://localhost:5001/self-assessment-returns) | `SelfAssessmentReturnsService.ListAsync(userId)` | Same wire path as income tax; self assessment docs entry point |
| [/self-assessment-returns/detail](https://localhost:5001/self-assessment-returns/detail?userId=&amp;periodEndsOn=YYYY-MM-DD) | Self assessment return GET and mark-as transitions | Parallel probe surface for `self_assessment_returns` docs |
| [/final-accounts-reports](https://localhost:5001/final-accounts-reports) | `FinalAccountsReportsService.ListAsync()` | Limited-company note when empty |
| [/final-accounts-reports/detail](https://localhost:5001/final-accounts-reports/detail?periodEndsOn=YYYY-MM-DD) | `GetFinalAccountsReportAsync()`, `MarkAsFiledAsync()`, `MarkAsUnfiledAsync()` | Filing transitions only; `ExcludeFromRunAll` on mutating console examples |
| [/sales-tax-periods](https://localhost:5001/sales-tax-periods) | `SalesTaxPeriodsService.ListAsync()` | US/universal companies; per-row mapping |
| [/sales-tax-periods/detail](https://localhost:5001/sales-tax-periods/detail?id=123) | `GetSalesTaxPeriodAsync()`, `CreateSalesTaxPeriodAsync()`, `UpdateSalesTaxPeriodAsync()`, `DeleteSalesTaxPeriodAsync()` | Probe upsert by name `SDK-PROBE-STP`; mutating console examples `ExcludeFromRunAll` |

Contributors adding endpoints should follow [`docs/contributing/sample-probe-pages.md`](../docs/contributing/sample-probe-pages.md).


---

## Sandbox Rate-Limit Diagnostics

The sample app provides developer tooling pages under **Developer Tools** in the navigation drawer:

| Route | Purpose |
|-------|---------|
| `/developer/turpinverse-seed` | Bulk seed all Turpinverse canon data (contacts through timeslips) in dependency order. Sandbox-only. |
| `/sandbox/rate-limit-test` | Live rate-limit diagnostics against `GET /v2/company`. Sandbox-only. |

### Turpinverse bulk seed

- **Route:** `/developer/turpinverse-seed`
- **Purpose:** One-click sandbox population before probe pages or console `--run-all` smoke.
- **Sandbox-only:** Restricted to sandbox OAuth sessions, matching the console `--seed-turpinverse` guard.
- **Idempotent upsert:** Matches existing records by stable keys; does not delete stale sandbox data.
- **Payroll gap:** Turpinverse does not seed RTI payroll (periods, payslips, or liability payments). The payroll API is read-only except mark-as-paid/unpaid. For console smoke on `Mark previous liability payment paid then unpaid`, run at least two pay periods with payslips in the sandbox UI first. See [Smoke and test limitations](../docs/contributing/smoke-and-test-limitations.md).

Per-resource CRUD probe pages still expose individual Turpinverse seed buttons for targeted runs.

### Rate-limit test
- **X-RateLimit-Test header:** Optionally sends the `X-RateLimit-Test: true` header, which triggers test-mode rate-limiting in the FreeAgent sandbox API. This enables safe, repeatable limit testing without impacting real usage quotas.
- **Not an SDK endpoint:** This is a sample-app-only diagnostics tool. It does not expose new SDK surface or production API features.

Use this page to:

- Run multiple requests against the `/v2/company` endpoint
- Inspect returned `X-RateLimit-*` headers and `Retry-After` values
- Experiment with rate-limit handling and SDK retry logic

See the page in the running app for full details and usage instructions.


## Notes for Contributors

- **Probe pages:** When adding SDK endpoints, follow [`docs/contributing/sample-probe-pages.md`](../docs/contributing/sample-probe-pages.md). Copy patterns from Company, Contacts, and Categories.
- **Token lifetime:** The OAuth session is restored from a short-lived browser cookie (~1 hour). Reconnect after expiry if needed.
- **OAuth CSRF state:** Pending authorisation state is also stored in a short-lived cookie (~15 minutes) so callbacks still validate after an app restart during local development.
- **Session security:** Tokens and pending OAuth state are stored as plaintext JSON in `HttpOnly` cookies. This is intentional for local development only - do not deploy the sample's cookie persistence pattern to production.
- **Sandbox vs production:** Choose the target environment from the Connect panel before starting OAuth. After connecting, disconnect to switch environments.
- **Port:** The sample uses port `5001` (HTTPS). If you change it in `Properties/launchSettings.json`, update the redirect URI in both your FreeAgent app registration and your user secrets.
- **Do not commit credentials.** `appsettings.json` contains only empty placeholders. User secrets are stored outside the repository in your OS user profile.
