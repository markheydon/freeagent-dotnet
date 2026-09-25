namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Sort field constants for credit note list requests.
/// </summary>
public static class CreditNoteSortOptions
{
    /// <summary>Sort by creation time.</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Sort by last update time.</summary>
    public const string UpdatedAt = "updated_at";
}
