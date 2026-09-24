namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Sort field constants for invoice list requests.
/// </summary>
public static class InvoiceSortOptions
{
    /// <summary>Sort by creation time.</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Sort by last update time.</summary>
    public const string UpdatedAt = "updated_at";
}
