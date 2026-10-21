using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Services.SalesTaxPeriods;

/// <summary>
/// Service for interacting with FreeAgent sales tax periods.
/// </summary>
public sealed class SalesTaxPeriodsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the sales tax periods service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal SalesTaxPeriodsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all sales tax periods for the current company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All sales tax periods returned by FreeAgent</returns>
    public async Task<IReadOnlyList<SalesTaxPeriod>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<SalesTaxPeriodsListResponse>("sales_tax_periods", cancellationToken);

        if (response.SalesTaxPeriods is null)
        {
            throw new FreeAgentApiException("Sales tax periods data missing from API response");
        }

        return response.SalesTaxPeriods;
    }

    /// <summary>
    /// Gets a single sales tax period by identifier.
    /// </summary>
    /// <param name="salesTaxPeriodId">Sales tax period identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sales tax period details</returns>
    public async Task<SalesTaxPeriod> GetSalesTaxPeriodAsync(long salesTaxPeriodId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(salesTaxPeriodId);

        var response = await _requestClient.GetAsync<SalesTaxPeriodResponse>(
            $"sales_tax_periods/{salesTaxPeriodId}",
            cancellationToken);

        if (response.SalesTaxPeriod is null)
        {
            throw new FreeAgentApiException("Sales tax period data missing from API response");
        }

        return response.SalesTaxPeriod;
    }

    /// <summary>
    /// Creates a sales tax period.
    /// </summary>
    /// <param name="request">Sales tax period attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created sales tax period</returns>
    public async Task<SalesTaxPeriod> CreateSalesTaxPeriodAsync(
        CreateSalesTaxPeriodRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new SalesTaxPeriodRequest<SalesTaxPeriodWritePayload>
        {
            SalesTaxPeriod = SalesTaxPeriodWritePayload.FromCreate(request)
        });
        var response = await _requestClient.PostAsync<SalesTaxPeriodResponse>("sales_tax_periods", content, cancellationToken);

        if (response.SalesTaxPeriod is null)
        {
            throw new FreeAgentApiException("Sales tax period data missing from API response");
        }

        return response.SalesTaxPeriod;
    }

    /// <summary>
    /// Updates a sales tax period.
    /// </summary>
    /// <param name="salesTaxPeriodId">Sales tax period identifier from the resource URL</param>
    /// <param name="request">Sales tax period attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated sales tax period</returns>
    public async Task<SalesTaxPeriod> UpdateSalesTaxPeriodAsync(
        long salesTaxPeriodId,
        UpdateSalesTaxPeriodRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(salesTaxPeriodId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new SalesTaxPeriodRequest<SalesTaxPeriodWritePayload>
        {
            SalesTaxPeriod = SalesTaxPeriodWritePayload.FromUpdate(request)
        });
        var response = await _requestClient.PutAsync<SalesTaxPeriodResponse>(
            $"sales_tax_periods/{salesTaxPeriodId}",
            content,
            cancellationToken);

        if (response.SalesTaxPeriod is null)
        {
            throw new FreeAgentApiException("Sales tax period data missing from API response");
        }

        return response.SalesTaxPeriod;
    }

    /// <summary>
    /// Deletes a sales tax period.
    /// </summary>
    /// <param name="salesTaxPeriodId">Sales tax period identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeleteSalesTaxPeriodAsync(long salesTaxPeriodId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(salesTaxPeriodId);

        return _requestClient.DeleteAsync($"sales_tax_periods/{salesTaxPeriodId}", cancellationToken);
    }
}
