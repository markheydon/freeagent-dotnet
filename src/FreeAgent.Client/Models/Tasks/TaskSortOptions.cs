namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Sort field values for listing tasks.
/// </summary>
public static class TaskSortOptions
{
    /// <summary>Sort by task name (default).</summary>
    public const string Name = "name";

    /// <summary>Sort by the project associated with the task.</summary>
    public const string Project = "project";

    /// <summary>Sort by billing rate.</summary>
    public const string BillingRate = "billing_rate";

    /// <summary>Sort by creation timestamp.</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Sort by last update timestamp.</summary>
    public const string UpdatedAt = "updated_at";
}
