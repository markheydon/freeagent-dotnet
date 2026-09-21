namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Sort field values for listing projects.
/// </summary>
public static class ProjectSortOptions
{
    /// <summary>Sort by project name (default).</summary>
    public const string Name = "name";

    /// <summary>Sort by contact organisation and name (organisation, last, first).</summary>
    public const string ContactName = "contact_name";

    /// <summary>Sort by contact display name (organisation, first, last).</summary>
    public const string ContactDisplayName = "contact_display_name";

    /// <summary>Sort by creation timestamp.</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Sort by last update timestamp.</summary>
    public const string UpdatedAt = "updated_at";
}
