namespace FreeAgent.Client;

/// <summary>
/// Represents one page of results from a paginated FreeAgent endpoint.
/// </summary>
/// <typeparam name="T">Type of items in the collection.</typeparam>
public sealed record PaginatedResponse<T>(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<T> Items)
{
    /// <summary>
    /// Gets a value indicating whether there are more pages available.
    /// </summary>
    public bool HasNextPage => Page * PerPage < Total;

    /// <summary>
    /// Gets the next page number if available.
    /// </summary>
    public int? NextPage => HasNextPage ? Page + 1 : null;
}
