namespace FreeAgent.Client.Models.StockItems;

/// <summary>
/// Sort field values for listing stock items.
/// </summary>
public static class StockItemSortOptions
{
    /// <summary>Sort by creation timestamp (default).</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Sort by description or code.</summary>
    public const string Description = "description";

    /// <summary>Sort by last update timestamp.</summary>
    public const string UpdatedAt = "updated_at";
}
