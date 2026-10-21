using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.TrialBalance;

/// <summary>
/// Service for trial balance report endpoints.
/// </summary>
public sealed class TrialBalanceService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the trial balance service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal TrialBalanceService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets the trial balance summary for the current date when no filters are supplied.
    /// </summary>
    /// <param name="fromDate">Optional start date (<c>from_date</c> query parameter).</param>
    /// <param name="toDate">Optional end date (<c>to_date</c> query parameter).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trial balance summary lines returned by FreeAgent</returns>
    public async Task<IReadOnlyList<TrialBalanceLine>> ListSummaryAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new List<KeyValuePair<string, string>>();

        if (fromDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "from_date",
                fromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (toDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "to_date",
                toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        var endpoint = queryParameters.Count == 0
            ? "accounting/trial_balance/summary"
            : FreeAgentQueryStringBuilder.BuildEndpoint("accounting/trial_balance/summary", queryParameters);

        var response = await _requestClient.GetAsync<TrialBalanceSummaryResponse>(endpoint, cancellationToken);

        if (response.TrialBalanceSummary is null)
        {
            throw new FreeAgentApiException("Trial balance summary data missing from API response");
        }

        return response.TrialBalanceSummary;
    }

    /// <summary>
    /// Gets trial balance opening balances.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Opening balance trial balance lines returned by FreeAgent</returns>
    public async Task<IReadOnlyList<TrialBalanceLine>> ListOpeningBalancesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<TrialBalanceSummaryResponse>(
            "accounting/trial_balance/summary/opening_balances",
            cancellationToken);

        if (response.TrialBalanceSummary is null)
        {
            throw new FreeAgentApiException("Trial balance opening balances data missing from API response");
        }

        return response.TrialBalanceSummary;
    }
}
