using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.BankFeeds;

/// <summary>
/// Service for interacting with FreeAgent bank feeds.
/// </summary>
/// <remarks>
/// Minimum FreeAgent access level: Banking.
/// Bank feeds are read-only in the FreeAgent API.
/// </remarks>
public sealed class BankFeedsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the bank feeds service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal BankFeedsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all bank feeds for the current company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All bank feeds returned by FreeAgent</returns>
    public async Task<IReadOnlyList<BankFeed>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<BankFeedsListResponse>("bank_feeds", cancellationToken);

        if (response.BankFeeds is null)
        {
            throw new FreeAgentApiException("Bank feeds data missing from API response");
        }

        return response.BankFeeds;
    }

    /// <summary>
    /// Gets a single bank feed by identifier.
    /// </summary>
    /// <param name="bankFeedId">Bank feed identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bank feed details</returns>
    public Task<BankFeed> GetBankFeedAsync(long bankFeedId, CancellationToken cancellationToken = default) =>
        GetBankFeedAsync(bankFeedId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single bank feed by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="bankFeedId">Bank feed identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bank feed details</returns>
    public async Task<BankFeed> GetBankFeedAsync(
        long bankFeedId,
        BankFeedGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankFeedId);

        var response = await _requestClient.GetAsync<BankFeedResponse>(
            $"bank_feeds/{bankFeedId}",
            cancellationToken);

        if (response.BankFeed is null)
        {
            throw new FreeAgentApiException("Bank feed data missing from API response");
        }

        await LinkedResourceHydration.HydrateBankAccountAsync(
            response.BankFeed,
            _requestClient,
            options?.IncludeBankAccount == true,
            cancellationToken);

        return response.BankFeed;
    }
}
