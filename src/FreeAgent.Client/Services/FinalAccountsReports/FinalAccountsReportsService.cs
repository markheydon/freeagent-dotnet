using System.Globalization;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.FinalAccountsReports;

namespace FreeAgent.Client.Services.FinalAccountsReports;

/// <summary>
/// Service for final accounts report endpoints.
/// </summary>
public sealed class FinalAccountsReportsService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the final accounts reports service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal FinalAccountsReportsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists final accounts reports for the company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Final accounts reports returned by FreeAgent</returns>
    public async Task<IReadOnlyList<FinalAccountsReport>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<FinalAccountsReportsListResponse>(
            "final_accounts_reports",
            cancellationToken);

        if (response.FinalAccountsReports is null)
        {
            throw new FreeAgentApiException("Final accounts reports data missing from API response");
        }

        return response.FinalAccountsReports;
    }

    /// <summary>
    /// Gets a final accounts report for the period ending on the specified date.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Final accounts report returned by FreeAgent</returns>
    public async Task<FinalAccountsReport> GetAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<FinalAccountsReportResponse>(
            BuildPeriodEndpoint(periodEndsOn),
            cancellationToken);

        if (response.FinalAccountsReport is null)
        {
            throw new FreeAgentApiException("Final accounts report data missing from API response");
        }

        return response.FinalAccountsReport;
    }

    /// <summary>
    /// Marks a final accounts report as filed.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated final accounts report</returns>
    public Task<FinalAccountsReport> MarkAsFiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionAsync(periodEndsOn, "mark_as_filed", cancellationToken);

    /// <summary>
    /// Marks a final accounts report as unfiled.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated final accounts report</returns>
    public Task<FinalAccountsReport> MarkAsUnfiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionAsync(periodEndsOn, "mark_as_unfiled", cancellationToken);

    private async Task<FinalAccountsReport> TransitionAsync(
        DateOnly periodEndsOn,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var endpoint = $"{BuildPeriodEndpoint(periodEndsOn)}/{action}";
        var response = await _requestClient.PutAsync<FinalAccountsReportResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.FinalAccountsReport is null)
        {
            throw new FreeAgentApiException("Final accounts report data missing from API response");
        }

        return response.FinalAccountsReport;
    }

    private static string BuildPeriodEndpoint(DateOnly periodEndsOn) =>
        $"final_accounts_reports/{periodEndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
}
