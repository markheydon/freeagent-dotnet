namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// View filter values for listing projects.
/// </summary>
public static class ProjectViews
{
    /// <summary>Active projects only.</summary>
    public const string Active = "active";

    /// <summary>Completed projects only.</summary>
    public const string Completed = "completed";

    /// <summary>Cancelled projects only.</summary>
    public const string Cancelled = "cancelled";

    /// <summary>Hidden projects only.</summary>
    public const string Hidden = "hidden";
}
