namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// View filter values for listing tasks.
/// </summary>
public static class TaskViews
{
    /// <summary>All tasks (default).</summary>
    public const string All = "all";

    /// <summary>Active tasks only.</summary>
    public const string Active = "active";

    /// <summary>Completed tasks only.</summary>
    public const string Completed = "completed";

    /// <summary>Hidden tasks only.</summary>
    public const string Hidden = "hidden";
}
