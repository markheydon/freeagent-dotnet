using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
using BalanceSheetModel = global::FreeAgent.Client.BalanceSheet;

namespace FreeAgent.Client.Services.BalanceSheet;

/// <summary>
/// Service for balance sheet report endpoints.
/// </summary>
public sealed class BalanceSheetService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the balance sheet service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal BalanceSheetService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets the balance sheet as at the current date for the ongoing annual accounting period when no filter is supplied.
    /// </summary>
    /// <param name="asAtDate">Optional as-at date (<c>as_at_date</c> query parameter).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Balance sheet returned by FreeAgent</returns>
    public async Task<BalanceSheetModel> GetBalanceSheetAsync(
        DateOnly? asAtDate = null,
        CancellationToken cancellationToken = default)
    {
        var endpoint = asAtDate is null
            ? "accounting/balance_sheet"
            : FreeAgentQueryStringBuilder.BuildEndpoint(
                "accounting/balance_sheet",
                [new KeyValuePair<string, string>(
                    "as_at_date",
                    asAtDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))]);

        var response = await _requestClient.GetAsync<BalanceSheetResponse>(endpoint, cancellationToken);

        if (response.BalanceSheet is null)
        {
            throw new FreeAgentApiException("Balance sheet data missing from API response");
        }

        return response.BalanceSheet;
    }

    /// <summary>
    /// Gets opening balance sheet figures for the opening balances period.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Opening balances balance sheet returned by FreeAgent</returns>
    public async Task<BalanceSheetModel> GetOpeningBalancesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<BalanceSheetResponse>(
            "accounting/balance_sheet/opening_balances",
            cancellationToken);

        if (response.BalanceSheet is null)
        {
            throw new FreeAgentApiException("Balance sheet opening balances data missing from API response");
        }

        return response.BalanceSheet;
    }
}
