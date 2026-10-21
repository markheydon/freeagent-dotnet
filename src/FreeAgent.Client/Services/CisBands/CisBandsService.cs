using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.CisBands;

/// <summary>
/// Service for Construction Industry Scheme band reference data.
/// </summary>
public sealed class CisBandsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the CIS bands service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CisBandsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all CIS bands selected for the company in CIS for Subcontractors settings.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Available CIS bands returned by FreeAgent</returns>
    public async Task<IReadOnlyList<CisBand>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<CisBandsListResponse>("cis_bands", cancellationToken);

        if (response.AvailableBands is null)
        {
            throw new FreeAgentApiException("CIS bands data missing from API response");
        }

        return response.AvailableBands;
    }
}
