using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.StockItems;

namespace FreeAgent.Client.Services.StockItems;

/// <summary>
/// Service for interacting with FreeAgent stock items.
/// </summary>
/// <remarks>
/// Stock items are read-only in the FreeAgent API.
/// </remarks>
public sealed class StockItemsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the stock items service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal StockItemsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all stock items for the current company.
    /// </summary>
    /// <param name="sort">Sort field (<see cref="StockItemSortOptions.CreatedAt"/>, <see cref="StockItemSortOptions.Description"/>, or <see cref="StockItemSortOptions.UpdatedAt"/>); prefix with <c>-</c> for descending</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All stock items returned by FreeAgent</returns>
    public async Task<IReadOnlyList<StockItem>> ListAsync(
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            queryParameters.Add(new KeyValuePair<string, string>("sort", sort));
        }

        var endpoint = queryParameters.Count == 0
            ? "stock_items"
            : FreeAgentQueryStringBuilder.BuildEndpoint("stock_items", queryParameters);

        var response = await _requestClient.GetAsync<StockItemsResponse>(endpoint, cancellationToken);

        if (response.StockItems is null)
        {
            throw new FreeAgentApiException("Stock items data missing from API response");
        }

        return response.StockItems;
    }

    /// <summary>
    /// Gets a single stock item by identifier.
    /// </summary>
    /// <param name="stockItemId">Stock item identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stock item details</returns>
    public async Task<StockItem> GetStockItemAsync(long stockItemId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stockItemId);

        var response = await _requestClient.GetAsync<StockItemResponse>($"stock_items/{stockItemId}", cancellationToken);

        if (response.StockItem is null)
        {
            throw new FreeAgentApiException("Stock item data missing from API response");
        }

        return response.StockItem;
    }
}
