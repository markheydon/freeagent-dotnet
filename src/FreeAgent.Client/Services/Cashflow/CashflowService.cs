using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Cashflow;
using CashflowModel = FreeAgent.Client.Models.Cashflow.Cashflow;

namespace FreeAgent.Client.Services.Cashflow;

/// <summary>
/// Service for cashflow report endpoints.
/// </summary>
public sealed class CashflowService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the cashflow service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CashflowService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets historic cashflow for the specified date range.
    /// </summary>
    /// <param name="fromDate">Start date (<c>from_date</c> query parameter).</param>
    /// <param name="toDate">End date (<c>to_date</c> query parameter).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cashflow summary returned by FreeAgent</returns>
    public async Task<CashflowModel> GetAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            "cashflow",
            [
                new KeyValuePair<string, string>("from_date", fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("to_date", toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            ]);

        var response = await _requestClient.GetAsync<CashflowResponse>(endpoint, cancellationToken);

        if (response.Cashflow is null)
        {
            throw new FreeAgentApiException("Cashflow data missing from API response");
        }

        return response.Cashflow;
    }
}
