using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.PriceListItems;

namespace FreeAgent.Client.Services.PriceListItems;

/// <summary>
/// Service for interacting with FreeAgent price list items.
/// </summary>
public sealed class PriceListItemsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the price list items service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal PriceListItemsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all price list items for the current company.
    /// </summary>
    /// <param name="sort">Sort field (<see cref="PriceListItemSortOptions.CreatedAt"/>, <see cref="PriceListItemSortOptions.Code"/>, or <see cref="PriceListItemSortOptions.UpdatedAt"/>); prefix with <c>-</c> for descending</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All price list items returned by FreeAgent</returns>
    public async Task<IReadOnlyList<PriceListItem>> ListAsync(
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            queryParameters.Add(new KeyValuePair<string, string>("sort", sort));
        }

        var endpoint = queryParameters.Count == 0
            ? "price_list_items"
            : FreeAgentQueryStringBuilder.BuildEndpoint("price_list_items", queryParameters);

        var response = await _requestClient.GetAsync<PriceListItemsResponse>(endpoint, cancellationToken);

        if (response.PriceListItems is null)
        {
            throw new FreeAgentApiException("Price list items data missing from API response");
        }

        return response.PriceListItems;
    }

    /// <summary>
    /// Gets a single price list item by identifier.
    /// </summary>
    /// <param name="priceListItemId">Price list item identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Price list item details</returns>
    public async Task<PriceListItem> GetPriceListItemAsync(long priceListItemId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(priceListItemId);

        var response = await _requestClient.GetAsync<PriceListItemResponse>(
            $"price_list_items/{priceListItemId}",
            cancellationToken);

        if (response.PriceListItem is null)
        {
            throw new FreeAgentApiException("Price list item data missing from API response");
        }

        return response.PriceListItem;
    }

    /// <summary>
    /// Creates a price list item.
    /// </summary>
    /// <param name="request">Price list item attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created price list item</returns>
    public async Task<PriceListItem> CreatePriceListItemAsync(
        CreatePriceListItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new PriceListItemRequest<CreatePriceListItemRequest> { PriceListItem = request });
        var response = await _requestClient.PostAsync<PriceListItemResponse>("price_list_items", content, cancellationToken);

        if (response.PriceListItem is null)
        {
            throw new FreeAgentApiException("Price list item data missing from API response");
        }

        return response.PriceListItem;
    }

    /// <summary>
    /// Updates a price list item.
    /// </summary>
    /// <param name="priceListItemId">Price list item identifier from the resource URL</param>
    /// <param name="request">Price list item attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated price list item</returns>
    public async Task<PriceListItem> UpdatePriceListItemAsync(
        long priceListItemId,
        UpdatePriceListItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(priceListItemId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new PriceListItemRequest<UpdatePriceListItemRequest> { PriceListItem = request });
        var response = await _requestClient.PutAsync<PriceListItemResponse>(
            $"price_list_items/{priceListItemId}",
            content,
            cancellationToken);

        if (response.PriceListItem is null)
        {
            throw new FreeAgentApiException("Price list item data missing from API response");
        }

        return response.PriceListItem;
    }

    /// <summary>
    /// Deletes a price list item.
    /// </summary>
    /// <param name="priceListItemId">Price list item identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeletePriceListItemAsync(long priceListItemId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(priceListItemId);

        return _requestClient.DeleteAsync($"price_list_items/{priceListItemId}", cancellationToken);
    }
}
