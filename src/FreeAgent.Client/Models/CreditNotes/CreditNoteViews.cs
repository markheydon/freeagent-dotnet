namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// View filter constants for credit note list requests.
/// </summary>
public static class CreditNoteViews
{
    /// <summary>All credit notes.</summary>
    public const string All = "all";

    /// <summary>Recent, open, or overdue credit notes.</summary>
    public const string RecentOpenOrOverdue = "recent_open_or_overdue";

    /// <summary>Open credit notes only.</summary>
    public const string Open = "open";

    /// <summary>Overdue credit notes only.</summary>
    public const string Overdue = "overdue";

    /// <summary>Open or overdue credit notes.</summary>
    public const string OpenOrOverdue = "open_or_overdue";

    /// <summary>Draft credit notes only.</summary>
    public const string Draft = "draft";

    /// <summary>Refunded credit notes only.</summary>
    public const string Refunded = "refunded";

    /// <summary>
    /// Credit notes from the last <c>N</c> months. Replace <c>N</c> with the desired month count
    /// (for example <c>last_3_months</c>).
    /// </summary>
    public const string LastNMonthsPrefix = "last_";
}
