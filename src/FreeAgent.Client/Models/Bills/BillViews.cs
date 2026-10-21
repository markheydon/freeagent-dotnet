namespace FreeAgent.Client;

/// <summary>
/// View filter constants for bill list requests.
/// </summary>
public static class BillViews
{
    /// <summary>All bills.</summary>
    public const string All = "all";

    /// <summary>Open bills only.</summary>
    public const string Open = "open";

    /// <summary>Overdue bills only.</summary>
    public const string Overdue = "overdue";

    /// <summary>Open or overdue bills.</summary>
    public const string OpenOrOverdue = "open_or_overdue";

    /// <summary>Open or overdue bill payments (positive total value).</summary>
    public const string OpenOrOverduePayments = "open_or_overdue_payments";

    /// <summary>Open or overdue bill refunds (negative total value).</summary>
    public const string OpenOrOverdueRefunds = "open_or_overdue_refunds";

    /// <summary>Paid bills only.</summary>
    public const string Paid = "paid";

    /// <summary>Recurring bills only.</summary>
    public const string Recurring = "recurring";

    /// <summary>Hire purchase bills only.</summary>
    public const string HirePurchase = "hire_purchase";

    /// <summary>Bills with CIS bill items only.</summary>
    public const string Cis = "cis";
}
