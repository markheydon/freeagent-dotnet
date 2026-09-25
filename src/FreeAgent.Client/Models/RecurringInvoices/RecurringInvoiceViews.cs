namespace FreeAgent.Client.Models.RecurringInvoices;

/// <summary>
/// View filter constants for recurring invoice list requests.
/// </summary>
public static class RecurringInvoiceViews
{
    /// <summary>Draft recurring invoices only.</summary>
    public const string Draft = "draft";

    /// <summary>Active recurring invoices only.</summary>
    public const string Active = "active";

    /// <summary>Inactive recurring invoices only.</summary>
    public const string Inactive = "inactive";
}
