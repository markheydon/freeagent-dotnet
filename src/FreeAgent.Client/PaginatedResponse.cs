namespace FreeAgent.Client;

/// <summary>
/// Represents one page of results from a paginated FreeAgent endpoint.
/// </summary>
/// <typeparam name="T">Type of items in the collection.</typeparam>
public sealed record PaginatedResponse<T>
{
    /// <summary>
    /// Initialises a new paginated response instance.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="perPage">Number of items per page.</param>
    /// <param name="total">Total number of items across all pages.</param>
    /// <param name="items">Items in the current page.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is null.</exception>
    public PaginatedResponse(int page, int perPage, int total, IReadOnlyList<T> items)
    {
        Page = page;
        PerPage = perPage;
        Total = total;
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    /// <summary>
    /// Gets the current 1-based page number.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PerPage { get; init; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Gets a value indicating whether there are more pages available.
    /// </summary>
    public bool HasNextPage => Page * PerPage < Total;

    /// <summary>
    /// Gets the next page number if available.
    /// </summary>
    public int? NextPage => HasNextPage ? Page + 1 : null;
}
