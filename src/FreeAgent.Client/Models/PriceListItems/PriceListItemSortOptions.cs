namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Sort field values for listing price list items.
/// </summary>
public static class PriceListItemSortOptions
{
    /// <summary>Sort by creation timestamp (default).</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Sort by item code.</summary>
    public const string Code = "code";

    /// <summary>Sort by last update timestamp.</summary>
    public const string UpdatedAt = "updated_at";
}
