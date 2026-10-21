using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Services.CapitalAssetTypes;

/// <summary>
/// Service for interacting with FreeAgent capital asset types.
/// </summary>
public sealed class CapitalAssetTypesService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the capital asset types service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CapitalAssetTypesService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all capital asset types for the current company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All capital asset types returned by FreeAgent</returns>
    public async Task<IReadOnlyList<CapitalAssetType>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<CapitalAssetTypesListResponse>("capital_asset_types", cancellationToken);

        if (response.CapitalAssetTypes is null)
        {
            throw new FreeAgentApiException("Capital asset types data missing from API response");
        }

        return response.CapitalAssetTypes;
    }

    /// <summary>
    /// Gets a single capital asset type by identifier.
    /// </summary>
    /// <param name="capitalAssetTypeId">Capital asset type identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Capital asset type details</returns>
    public async Task<CapitalAssetType> GetCapitalAssetTypeAsync(long capitalAssetTypeId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capitalAssetTypeId);

        var response = await _requestClient.GetAsync<CapitalAssetTypeResponse>(
            $"capital_asset_types/{capitalAssetTypeId}",
            cancellationToken);

        if (response.CapitalAssetType is null)
        {
            throw new FreeAgentApiException("Capital asset type data missing from API response");
        }

        return response.CapitalAssetType;
    }

    /// <summary>
    /// Creates a capital asset type.
    /// </summary>
    /// <param name="request">Capital asset type attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created capital asset type</returns>
    public async Task<CapitalAssetType> CreateCapitalAssetTypeAsync(
        CreateCapitalAssetTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new CapitalAssetTypeRequest<CapitalAssetTypeWritePayload>
        {
            CapitalAssetType = CapitalAssetTypeWritePayload.FromCreate(request)
        });
        var response = await _requestClient.PostAsync<CapitalAssetTypeResponse>("capital_asset_types", content, cancellationToken);

        if (response.CapitalAssetType is null)
        {
            throw new FreeAgentApiException("Capital asset type data missing from API response");
        }

        return response.CapitalAssetType;
    }

    /// <summary>
    /// Updates a user-created capital asset type that does not contain any items.
    /// </summary>
    /// <param name="capitalAssetTypeId">Capital asset type identifier from the resource URL</param>
    /// <param name="request">Capital asset type attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated capital asset type</returns>
    public async Task<CapitalAssetType> UpdateCapitalAssetTypeAsync(
        long capitalAssetTypeId,
        UpdateCapitalAssetTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capitalAssetTypeId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new CapitalAssetTypeRequest<CapitalAssetTypeWritePayload>
        {
            CapitalAssetType = CapitalAssetTypeWritePayload.FromUpdate(request)
        });
        var response = await _requestClient.PutAsync<CapitalAssetTypeResponse>(
            $"capital_asset_types/{capitalAssetTypeId}",
            content,
            cancellationToken);

        if (response.CapitalAssetType is null)
        {
            throw new FreeAgentApiException("Capital asset type data missing from API response");
        }

        return response.CapitalAssetType;
    }

    /// <summary>
    /// Deletes a user-created capital asset type that does not contain any items.
    /// </summary>
    /// <param name="capitalAssetTypeId">Capital asset type identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeleteCapitalAssetTypeAsync(long capitalAssetTypeId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capitalAssetTypeId);

        return _requestClient.DeleteAsync($"capital_asset_types/{capitalAssetTypeId}", cancellationToken);
    }
}
