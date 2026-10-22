namespace FreeAgent.Client.BlazorSample.Navigation;

/// <summary>
/// A single SDK Explorer drawer link.
/// </summary>
public sealed record SdkExplorerNavItem(
    string Title,
    string Href,
    string Icon,
    string RoutePath);

/// <summary>
/// A grouped set of drawer links.
/// </summary>
public sealed record SdkExplorerNavGroup(
    string Title,
    string Icon,
    IReadOnlyList<SdkExplorerNavItem> Items);
