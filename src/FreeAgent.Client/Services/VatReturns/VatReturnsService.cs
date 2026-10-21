using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.VatReturns;

/// <summary>
/// Service for VAT return read endpoints.
/// </summary>
public sealed class VatReturnsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the VAT returns service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal VatReturnsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists VAT returns for the company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>VAT returns returned by FreeAgent</returns>
    public async Task<IReadOnlyList<VatReturn>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<VatReturnsListResponse>("vat_returns", cancellationToken);

        if (response.VatReturns is null)
        {
            throw new FreeAgentApiException("VAT returns data missing from API response");
        }

        return response.VatReturns;
    }

    /// <summary>
    /// Gets a VAT return for the period ending on the specified date.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>VAT return returned by FreeAgent</returns>
    public async Task<VatReturn> GetVatReturnAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
    {
        var endpoint = $"vat_returns/{periodEndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
        var response = await _requestClient.GetAsync<VatReturnResponse>(endpoint, cancellationToken);

        if (response.VatReturn is null)
        {
            throw new FreeAgentApiException("VAT return data missing from API response");
        }

        return response.VatReturn;
    }
}
