using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CapitalAssets;

namespace FreeAgent.Client.Services.CapitalAssets;

/// <summary>
/// Service for interacting with FreeAgent capital assets.
/// </summary>
/// <remarks>
/// Capital assets are read-only in the FreeAgent API. Minimum access level: Banking.
/// </remarks>
public sealed class CapitalAssetsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the capital assets service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CapitalAssetsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists capital assets for the current company.
    /// </summary>
    /// <param name="view">Optional view filter.</param>
    /// <param name="includeHistory">When <see langword="true"/>, requests lifecycle history on each capital asset.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Capital assets returned by FreeAgent</returns>
    public async Task<IReadOnlyList<CapitalAsset>> ListAsync(
        CapitalAssetView? view = null,
        bool includeHistory = false,
        CancellationToken cancellationToken = default)
    {
        var endpoint = BuildListEndpoint(view, includeHistory);
        var response = await _requestClient.GetAsync<CapitalAssetsListResponse>(endpoint, cancellationToken);

        if (response.CapitalAssets is null)
        {
            throw new FreeAgentApiException("Capital assets data missing from API response");
        }

        return response.CapitalAssets;
    }

    /// <summary>
    /// Gets a single capital asset by identifier.
    /// </summary>
    /// <param name="capitalAssetId">Capital asset identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Capital asset details</returns>
    public Task<CapitalAsset> GetCapitalAssetAsync(long capitalAssetId, CancellationToken cancellationToken = default) =>
        GetCapitalAssetAsync(capitalAssetId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single capital asset by identifier with optional history.
    /// </summary>
    /// <param name="capitalAssetId">Capital asset identifier from the resource URL</param>
    /// <param name="options">Optional request settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Capital asset details</returns>
    public async Task<CapitalAsset> GetCapitalAssetAsync(
        long capitalAssetId,
        CapitalAssetGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capitalAssetId);

        var endpoint = BuildGetEndpoint(capitalAssetId, options?.IncludeHistory == true);
        var response = await _requestClient.GetAsync<CapitalAssetResponse>(endpoint, cancellationToken);

        if (response.CapitalAsset is null)
        {
            throw new FreeAgentApiException("Capital asset data missing from API response");
        }

        return response.CapitalAsset;
    }

    private static string BuildListEndpoint(CapitalAssetView? view, bool includeHistory)
    {
        var queryParameters = new List<KeyValuePair<string, string>>();

        if (view is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("view", EnumWireValue.Get(view.Value)));
        }

        if (includeHistory)
        {
            queryParameters.Add(new KeyValuePair<string, string>("include_history", "true"));
        }

        return queryParameters.Count == 0
            ? "capital_assets"
            : FreeAgentQueryStringBuilder.BuildEndpoint("capital_assets", queryParameters);
    }

    private static string BuildGetEndpoint(long capitalAssetId, bool includeHistory)
    {
        var endpoint = $"capital_assets/{capitalAssetId}";

        if (!includeHistory)
        {
            return endpoint;
        }

        return FreeAgentQueryStringBuilder.BuildEndpoint(
            endpoint,
            [new KeyValuePair<string, string>("include_history", "true")]);
    }
}
