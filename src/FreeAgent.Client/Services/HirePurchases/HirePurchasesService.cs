using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.HirePurchases;

namespace FreeAgent.Client.Services.HirePurchases;

/// <summary>
/// Service for interacting with FreeAgent hire purchases.
/// </summary>
/// <remarks>
/// Hire purchases are read-only in the FreeAgent API.
/// </remarks>
public sealed class HirePurchasesService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the hire purchases service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal HirePurchasesService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all hire purchases for the current company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All hire purchases returned by FreeAgent</returns>
    public async Task<IReadOnlyList<HirePurchase>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<HirePurchasesResponse>("hire_purchases", cancellationToken);

        if (response.HirePurchases is null)
        {
            throw new FreeAgentApiException("Hire purchases data missing from API response");
        }

        return response.HirePurchases;
    }

    /// <summary>
    /// Gets a single hire purchase by identifier.
    /// </summary>
    /// <param name="hirePurchaseId">Hire purchase identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hire purchase details</returns>
    public Task<HirePurchase> GetHirePurchaseAsync(long hirePurchaseId, CancellationToken cancellationToken = default) =>
        GetHirePurchaseAsync(hirePurchaseId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single hire purchase by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="hirePurchaseId">Hire purchase identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hire purchase details</returns>
    public async Task<HirePurchase> GetHirePurchaseAsync(
        long hirePurchaseId,
        HirePurchaseGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hirePurchaseId);

        var response = await _requestClient.GetAsync<HirePurchaseResponse>(
            $"hire_purchases/{hirePurchaseId}",
            cancellationToken);

        if (response.HirePurchase is null)
        {
            throw new FreeAgentApiException("Hire purchase data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillAsync(
            response.HirePurchase,
            _requestClient,
            options?.IncludeBill == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateLiabilitiesOverOneYearCategoryAsync(
            response.HirePurchase,
            _requestClient,
            options?.IncludeLiabilitiesOverOneYearCategory == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateLiabilitiesUnderOneYearCategoryAsync(
            response.HirePurchase,
            _requestClient,
            options?.IncludeLiabilitiesUnderOneYearCategory == true,
            cancellationToken);

        return response.HirePurchase;
    }
}
