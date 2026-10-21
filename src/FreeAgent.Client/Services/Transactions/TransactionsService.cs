using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Transactions;

namespace FreeAgent.Client.Services.Transactions;

/// <summary>
/// Service for the accounting transactions report.
/// </summary>
/// <remarks>
/// Minimum FreeAgent access level: Tax, Accounting &amp; Users.
/// Lists ledger transactions posted to account categories.
/// </remarks>
public sealed class TransactionsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initialises a new instance of the transactions service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal TransactionsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists accounting transactions.
    /// </summary>
    /// <param name="fromDate">Optional start date (<c>from_date</c> query parameter).</param>
    /// <param name="toDate">Optional end date (<c>to_date</c> query parameter).</param>
    /// <param name="nominalCode">Optional category nominal code (<c>nominal_code</c> query parameter).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions returned by FreeAgent</returns>
    /// <remarks>
    /// When no dates are supplied, FreeAgent returns transactions from the start of the current accounting period to the current date.
    /// Requested periods must be 12 months or shorter, or fall inside a single accounting year.
    /// </remarks>
    public async Task<IReadOnlyList<AccountingTransaction>> ListAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        string? nominalCode = null,
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

        if (nominalCode is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
            queryParameters.Add(new KeyValuePair<string, string>("nominal_code", nominalCode));
        }

        var endpoint = queryParameters.Count == 0
            ? "accounting/transactions"
            : FreeAgentQueryStringBuilder.BuildEndpoint("accounting/transactions", queryParameters);

        var response = await _requestClient.GetAsync<AccountingTransactionsListResponse>(endpoint, cancellationToken);

        if (response.Transactions is null)
        {
            throw new FreeAgentApiException("Transactions data missing from API response");
        }

        return response.Transactions;
    }

    /// <summary>
    /// Gets a single accounting transaction by identifier.
    /// </summary>
    /// <param name="transactionId">Transaction identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction returned by FreeAgent</returns>
    public async Task<AccountingTransaction> GetAsync(long transactionId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transactionId);

        var response = await _requestClient.GetAsync<AccountingTransactionResponse>(
            $"accounting/transactions/{transactionId.ToString(CultureInfo.InvariantCulture)}",
            cancellationToken);

        if (response.Transaction is null)
        {
            throw new FreeAgentApiException("Transaction data missing from API response");
        }

        return response.Transaction;
    }
}
