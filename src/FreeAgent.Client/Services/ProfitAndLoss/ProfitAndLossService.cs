using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.ProfitAndLoss;

namespace FreeAgent.Client.Services.ProfitAndLoss;

/// <summary>
/// Service for profit and loss report endpoints.
/// </summary>
public sealed class ProfitAndLossService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the profit and loss service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal ProfitAndLossService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets the profit and loss summary for the current accounting year to date when no filters are supplied.
    /// </summary>
    /// <param name="fromDate">Optional start date (<c>from_date</c> query parameter).</param>
    /// <param name="toDate">Optional end date (<c>to_date</c> query parameter).</param>
    /// <param name="accountingPeriod">Optional accounting period (<c>accounting_period</c>, for example <c>2022/23</c>).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profit and loss summary returned by FreeAgent</returns>
    public async Task<ProfitAndLossSummary> GetSummaryAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        string? accountingPeriod = null,
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

        if (!string.IsNullOrWhiteSpace(accountingPeriod))
        {
            queryParameters.Add(new KeyValuePair<string, string>("accounting_period", accountingPeriod));
        }

        var endpoint = queryParameters.Count == 0
            ? "accounting/profit_and_loss/summary"
            : FreeAgentQueryStringBuilder.BuildEndpoint("accounting/profit_and_loss/summary", queryParameters);

        var response = await _requestClient.GetAsync<ProfitAndLossSummaryResponse>(endpoint, cancellationToken);

        if (response.ProfitAndLossSummary is null)
        {
            throw new FreeAgentApiException("Profit and loss summary data missing from API response");
        }

        return response.ProfitAndLossSummary;
    }
}
