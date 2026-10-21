using System.Globalization;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.CorporationTaxReturns;

namespace FreeAgent.Client.Services.CorporationTaxReturns;

/// <summary>
/// Service for corporation tax return endpoints.
/// </summary>
public sealed class CorporationTaxReturnsService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the corporation tax returns service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CorporationTaxReturnsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists corporation tax returns for the company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Corporation tax returns returned by FreeAgent</returns>
    public async Task<IReadOnlyList<CorporationTaxReturn>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<CorporationTaxReturnsListResponse>(
            "corporation_tax_returns",
            cancellationToken);

        if (response.CorporationTaxReturns is null)
        {
            throw new FreeAgentApiException("Corporation tax returns data missing from API response");
        }

        return response.CorporationTaxReturns;
    }

    /// <summary>
    /// Gets a corporation tax return for the period ending on the specified date.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Corporation tax return returned by FreeAgent</returns>
    public async Task<CorporationTaxReturn> GetAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<CorporationTaxReturnResponse>(
            BuildPeriodEndpoint(periodEndsOn),
            cancellationToken);

        if (response.CorporationTaxReturn is null)
        {
            throw new FreeAgentApiException("Corporation tax return data missing from API response");
        }

        return response.CorporationTaxReturn;
    }

    /// <summary>
    /// Marks a corporation tax return as filed.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated corporation tax return</returns>
    public Task<CorporationTaxReturn> MarkAsFiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionAsync(periodEndsOn, "mark_as_filed", cancellationToken);

    /// <summary>
    /// Marks a corporation tax return as unfiled.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated corporation tax return</returns>
    public Task<CorporationTaxReturn> MarkAsUnfiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionAsync(periodEndsOn, "mark_as_unfiled", cancellationToken);

    /// <summary>
    /// Marks a corporation tax return as paid.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated corporation tax return</returns>
    public Task<CorporationTaxReturn> MarkAsPaidAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionAsync(periodEndsOn, "mark_as_paid", cancellationToken);

    /// <summary>
    /// Marks a corporation tax return as unpaid.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated corporation tax return</returns>
    public Task<CorporationTaxReturn> MarkAsUnpaidAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionAsync(periodEndsOn, "mark_as_unpaid", cancellationToken);

    private async Task<CorporationTaxReturn> TransitionAsync(
        DateOnly periodEndsOn,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var endpoint = $"{BuildPeriodEndpoint(periodEndsOn)}/{action}";
        var response = await _requestClient.PutAsync<CorporationTaxReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.CorporationTaxReturn is null)
        {
            throw new FreeAgentApiException("Corporation tax return data missing from API response");
        }

        return response.CorporationTaxReturn;
    }

    private static string BuildPeriodEndpoint(DateOnly periodEndsOn) =>
        $"corporation_tax_returns/{periodEndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
}
