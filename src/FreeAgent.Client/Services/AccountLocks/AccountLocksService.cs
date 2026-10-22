using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Services.AccountLocks;

/// <summary>
/// Service for company account lock endpoints.
/// </summary>
public sealed class AccountLocksService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the account locks service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal AccountLocksService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists account locks for the company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account locks and optional user-lock date bounds</returns>
    public async Task<AccountLocksListResult> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<AccountLocksListResponse>("account_locks", cancellationToken);

        if (response.AccountLocks is null)
        {
            throw new FreeAgentApiException("Account locks data missing from API response");
        }

        return new AccountLocksListResult(
            response.AccountLocks,
            response.EarliestLockDate,
            response.LatestLockDate);
    }

    /// <summary>
    /// Creates or updates the user account lock.
    /// </summary>
    /// <param name="lockedToDate">Date through which the account should be locked.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SetUserAccountLockAsync(DateOnly lockedToDate, CancellationToken cancellationToken = default)
    {
        var content = FreeAgentJsonSerializer.CreateContent(new AccountLockRequest
        {
            AccountLock = new AccountLockWritePayload { LockedToDate = lockedToDate }
        });

        return _requestClient.SendPutAsync("account_locks", content, cancellationToken);
    }

    /// <summary>
    /// Deletes the user account lock.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteUserAccountLockAsync(CancellationToken cancellationToken = default) =>
        _requestClient.DeleteAsync("account_locks", cancellationToken);
}
