namespace FreeAgent.Client.BlazorSample.Navigation;

/// <summary>
/// Route matching and menu filter helpers for the SDK Explorer drawer.
/// </summary>
public static class SdkExplorerNavMatching
{
    /// <summary>
    /// Whether <paramref name="currentPath"/> is the Overview route.
    /// </summary>
    public static bool IsOverviewRoute(string currentPath) =>
        string.IsNullOrEmpty(currentPath);

    /// <summary>
    /// Whether <paramref name="currentPath"/> matches the nav item route rules.
    /// </summary>
    public static bool RouteMatches(string currentPath, SdkExplorerNavItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrEmpty(item.RoutePath))
        {
            return IsOverviewRoute(currentPath);
        }

        return currentPath.Equals(item.RoutePath, StringComparison.OrdinalIgnoreCase)
            || currentPath.StartsWith(item.RoutePath + "/", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Whether any item in the group matches the current path.
    /// </summary>
    public static bool GroupIsActive(string currentPath, SdkExplorerNavGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return group.Items.Any(item => RouteMatches(currentPath, item));
    }

    /// <summary>
    /// Whether the group should be visible for the filter query.
    /// </summary>
    public static bool ShouldShowGroup(SdkExplorerNavGroup group, string? query)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        return GetVisibleItems(group, query).Count > 0;
    }

    /// <summary>
    /// Items in the group that match the filter query.
    /// </summary>
    public static IReadOnlyList<SdkExplorerNavItem> GetVisibleItems(SdkExplorerNavGroup group, string? query)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (string.IsNullOrWhiteSpace(query))
        {
            return group.Items;
        }

        var normalized = query.Trim();
        return group.Items
            .Where(item => ItemMatchesFilter(group, item, normalized))
            .ToList();
    }

    /// <summary>
    /// Whether the Overview link matches the filter.
    /// </summary>
    public static bool OverviewMatchesFilter(string? query) =>
        string.IsNullOrWhiteSpace(query)
        || "Overview".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Total nav items across all groups (excluding Overview).
    /// </summary>
    public static int TotalItemCount(IReadOnlyList<SdkExplorerNavGroup> groups) =>
        groups.Sum(group => group.Items.Count);

    private static bool ItemMatchesFilter(SdkExplorerNavGroup group, SdkExplorerNavItem item, string query) =>
        item.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
        || group.Title.Contains(query, StringComparison.OrdinalIgnoreCase);
}
