using MudBlazor;

namespace FreeAgent.Client.BlazorSample.Navigation;

/// <summary>
/// SDK Explorer drawer menu catalogue (single source of truth for probe page navigation).
/// </summary>
public static class SdkExplorerNavCatalog
{
    /// <summary>
    /// All navigation groups in drawer order.
    /// </summary>
    public static IReadOnlyList<SdkExplorerNavGroup> Groups { get; } = BuildGroups();

    private static IReadOnlyList<SdkExplorerNavGroup> BuildGroups() =>
    [
        Group(
            "Company",
            Icons.Material.Filled.Business,
            Item("Company Overview", "/company", Icons.Material.Filled.FactCheck),
            Item("Business Categories", "/company/business-categories", Icons.Material.Filled.Category),
            Item("Tax Timeline", "/company/tax-timeline", Icons.Material.Filled.Event),
            Item("Email addresses", "/email-addresses", Icons.Material.Filled.Email)),
        Group(
            "Contacts",
            Icons.Material.Filled.Contacts,
            Item("Contacts list", "/contacts", Icons.Material.Filled.AllInclusive),
            Item("Contact CRUD", "/contacts/detail", Icons.Material.Filled.Person)),
        Group(
            "Categories",
            Icons.Material.Filled.AccountTree,
            Item("Categories list", "/categories", Icons.Material.Filled.AllInclusive),
            Item("Category CRUD", "/categories/detail", Icons.Material.Filled.Edit)),
        Group(
            "Invoices",
            Icons.Material.Filled.ReceiptLong,
            Item("Invoices list", "/invoices", Icons.Material.Filled.AllInclusive),
            Item("Invoice CRUD", "/invoices/detail", Icons.Material.Filled.RequestQuote)),
        Group(
            "Estimates",
            Icons.Material.Filled.Description,
            Item("Estimates list", "/estimates", Icons.Material.Filled.AllInclusive),
            Item("Estimate CRUD", "/estimates/detail", Icons.Material.Filled.EditNote)),
        Group(
            "Recurring invoices",
            Icons.Material.Filled.Autorenew,
            Item("Recurring invoices list", "/recurring-invoices", Icons.Material.Filled.AllInclusive),
            Item("Recurring invoice detail", "/recurring-invoices/detail", Icons.Material.Filled.Visibility)),
        Group(
            "Credit notes",
            Icons.Material.Filled.Reply,
            Item("Credit notes list", "/credit-notes", Icons.Material.Filled.AllInclusive),
            Item("Credit note CRUD", "/credit-notes/detail", Icons.Material.Filled.EditNote),
            Item("Reconciliations list", "/credit-note-reconciliations", Icons.Material.Filled.AllInclusive),
            Item("Reconciliation CRUD", "/credit-note-reconciliations/detail", Icons.Material.Filled.CompareArrows)),
        Group(
            "Bills",
            Icons.Material.Filled.RequestQuote,
            Item("Bills list", "/bills", Icons.Material.Filled.AllInclusive),
            Item("Bill CRUD", "/bills/detail", Icons.Material.Filled.EditNote),
            Item("Hire purchases list", "/hire-purchases", Icons.Material.Filled.AllInclusive),
            Item("Hire purchase detail", "/hire-purchases/detail", Icons.Material.Filled.Visibility)),
        Group(
            "Expenses",
            Icons.Material.Filled.Receipt,
            Item("Expenses list", "/expenses", Icons.Material.Filled.AllInclusive),
            Item("Expense CRUD", "/expenses/detail", Icons.Material.Filled.EditNote),
            Item("Attachment detail", "/attachments/detail", Icons.Material.Filled.AttachFile)),
        Group(
            "Banking",
            Icons.Material.Filled.AccountBalance,
            Item("Bank accounts list", "/bank-accounts", Icons.Material.Filled.AllInclusive),
            Item("Bank account CRUD", "/bank-accounts/detail", Icons.Material.Filled.Edit),
            Item("Bank feeds list", "/bank-feeds", Icons.Material.Filled.AllInclusive),
            Item("Bank feed detail", "/bank-feeds/detail", Icons.Material.Filled.Visibility),
            Item("Bank transactions list", "/bank-transactions", Icons.Material.Filled.AllInclusive),
            Item("Bank transaction operations", "/bank-transactions/detail", Icons.Material.Filled.Edit),
            Item("Bank transaction explanations list", "/bank-transaction-explanations", Icons.Material.Filled.AllInclusive),
            Item("Bank transaction explanation CRUD", "/bank-transaction-explanations/detail", Icons.Material.Filled.Edit)),
        Group(
            "Assets and journals",
            Icons.Material.Filled.AccountBalanceWallet,
            Item("Capital assets list", "/capital-assets", Icons.Material.Filled.AllInclusive),
            Item("Capital asset detail", "/capital-assets/detail", Icons.Material.Filled.Visibility),
            Item("Capital asset types list", "/capital-asset-types", Icons.Material.Filled.AllInclusive),
            Item("Capital asset type CRUD", "/capital-asset-types/detail", Icons.Material.Filled.Edit),
            Item("Journal sets list", "/journal-sets", Icons.Material.Filled.AllInclusive),
            Item("Journal set CRUD", "/journal-sets/detail", Icons.Material.Filled.Edit),
            Item("Properties list", "/properties", Icons.Material.Filled.HomeWork),
            Item("Property CRUD", "/properties/detail", Icons.Material.Filled.Edit),
            Item("Depreciation profiles", "/depreciation-profiles", Icons.Material.Filled.TrendingDown)),
        Group(
            "Payroll",
            Icons.Material.Filled.Payments,
            Item("Payroll tax year", "/payroll", Icons.Material.Filled.Payments),
            Item("Payroll period payslips", "/payroll/period", Icons.Material.Filled.Receipt),
            Item("Payroll profiles", "/payroll-profiles", Icons.Material.Filled.Person)),
        Group(
            "CIS",
            Icons.Material.Filled.Handshake,
            Item("CIS bands list", "/cis-bands", Icons.Material.Filled.AllInclusive),
            Item("CIS settings", "/cis-settings", Icons.Material.Filled.Settings),
            Item("CIS settings CRUD", "/cis-settings/detail", Icons.Material.Filled.Edit)),
        Group(
            "Reports",
            Icons.Material.Filled.Assessment,
            Item("Profit and loss", "/profit-and-loss", Icons.Material.Filled.TrendingUp),
            Item("Balance sheet", "/balance-sheet", Icons.Material.Filled.AccountBalance),
            Item("Trial balance", "/trial-balance", Icons.Material.Filled.TableChart),
            Item("Cashflow", "/cashflow", Icons.Material.Filled.SwapVert),
            Item("Transactions", "/transactions", Icons.Material.Filled.Receipt),
            Item("Transaction detail", "/transactions/detail", Icons.Material.Filled.Visibility),
            Item("Final accounts reports", "/final-accounts-reports", Icons.Material.Filled.Description),
            Item("Final accounts report detail", "/final-accounts-reports/detail", Icons.Material.Filled.Edit)),
        Group(
            "Taxes",
            Icons.Material.Filled.ReceiptLong,
            Item("VAT returns", "/vat-returns", Icons.Material.Filled.ReceiptLong),
            Item("Corporation tax returns", "/corporation-tax-returns", Icons.Material.Filled.Gavel),
            Item("Corporation tax return detail", "/corporation-tax-returns/detail", Icons.Material.Filled.Edit),
            Item("Income tax returns", "/income-tax-returns", Icons.Material.Filled.Assignment),
            Item("Income tax return detail", "/income-tax-returns/detail", Icons.Material.Filled.Edit),
            Item("Self assessment returns", "/self-assessment-returns", Icons.Material.Filled.AssignmentInd),
            Item("Self assessment return detail", "/self-assessment-returns/detail", Icons.Material.Filled.Edit),
            Item("Sales tax periods list", "/sales-tax-periods", Icons.Material.Filled.Receipt),
            Item("Sales tax period CRUD", "/sales-tax-periods/detail", Icons.Material.Filled.Edit),
            Item("EC VAT MOSS rates", "/ec-moss/sales-tax-rates", Icons.Material.Filled.Public)),
        Group(
            "Sales catalogue",
            Icons.Material.Filled.Inventory2,
            Item("Stock items list", "/stock-items", Icons.Material.Filled.AllInclusive),
            Item("Stock item detail", "/stock-items/detail", Icons.Material.Filled.Visibility),
            Item("Price list items list", "/price-list-items", Icons.Material.Filled.AllInclusive),
            Item("Price list item CRUD", "/price-list-items/detail", Icons.Material.Filled.Edit)),
        Group(
            "Projects and time",
            Icons.Material.Filled.Work,
            Item("Projects list", "/projects", Icons.Material.Filled.AllInclusive),
            Item("Project CRUD", "/projects/detail", Icons.Material.Filled.Assignment),
            Item("Tasks list", "/tasks", Icons.Material.Filled.AllInclusive),
            Item("Task CRUD", "/tasks/detail", Icons.Material.Filled.Task),
            Item("Timeslips list", "/timeslips", Icons.Material.Filled.AllInclusive),
            Item("Timeslip CRUD", "/timeslips/detail", Icons.Material.Filled.Timer),
            Item("Notes list", "/notes", Icons.Material.Filled.AllInclusive),
            Item("Note CRUD", "/notes/detail", Icons.Material.Filled.StickyNote2)),
        Group(
            "Users",
            Icons.Material.Filled.People,
            Item("Users list", "/users", Icons.Material.Filled.AllInclusive),
            Item("User CRUD", "/users/detail", Icons.Material.Filled.Person)),
        Group(
            "Developer Tools",
            Icons.Material.Filled.Handyman,
            Item("Turpinverse seed", "/developer/turpinverse-seed", Icons.Material.Filled.Agriculture),
            Item("Sandbox Rate-Limit Test", "/sandbox/rate-limit-test", Icons.Material.Filled.Speed)),
    ];

    private static SdkExplorerNavGroup Group(string title, string icon, params SdkExplorerNavItem[] items) =>
        new(title, icon, items);

    private static SdkExplorerNavItem Item(string title, string href, string icon)
    {
        var routePath = href.TrimStart('/');
        return new SdkExplorerNavItem(title, href, icon, routePath);
    }
}
