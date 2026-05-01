using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Pagination response wrapper for FreeAgent API.
/// </summary>
/// <typeparam name="T">Type of items in the collection</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Current page number.
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    /// <summary>
    /// Total number of items.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Items in the current page.
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Checks if there are more pages available.
    /// </summary>
    [JsonIgnore]
    public bool HasNextPage => Page * PerPage < Total;

    /// <summary>
    /// Gets the next page number if available.
    /// </summary>
    [JsonIgnore]
    public int? NextPage => HasNextPage ? Page + 1 : null;
}
