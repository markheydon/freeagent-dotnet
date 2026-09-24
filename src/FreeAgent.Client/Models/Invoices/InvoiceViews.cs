namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// View filter constants for invoice list requests.
/// </summary>
public static class InvoiceViews
{
    /// <summary>All invoices.</summary>
    public const string All = "all";

    /// <summary>Recent, open, or overdue invoices.</summary>
    public const string RecentOpenOrOverdue = "recent_open_or_overdue";

    /// <summary>Open invoices only.</summary>
    public const string Open = "open";

    /// <summary>Overdue invoices only.</summary>
    public const string Overdue = "overdue";

    /// <summary>Open or overdue invoices.</summary>
    public const string OpenOrOverdue = "open_or_overdue";

    /// <summary>Draft invoices only.</summary>
    public const string Draft = "draft";

    /// <summary>Paid invoices only.</summary>
    public const string Paid = "paid";

    /// <summary>Invoices scheduled to email.</summary>
    public const string ScheduledToEmail = "scheduled_to_email";

    /// <summary>Invoices with active thank you emails.</summary>
    public const string ThankYouEmails = "thank_you_emails";

    /// <summary>Invoices with active reminder emails.</summary>
    public const string ReminderEmails = "reminder_emails";
}
