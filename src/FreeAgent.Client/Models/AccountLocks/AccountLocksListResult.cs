namespace FreeAgent.Client;

/// <summary>
/// Account locks for a company, including optional bounds for setting a user account lock.
/// </summary>
public sealed class AccountLocksListResult
{
    /// <summary>
    /// Initializes a new instance of <see cref="AccountLocksListResult"/>.
    /// </summary>
    /// <param name="accountLocks">Locks returned by FreeAgent.</param>
    /// <param name="earliestLockDate">Earliest date a user lock may be set, when returned.</param>
    /// <param name="latestLockDate">Latest date a user lock may be set, when returned.</param>
    public AccountLocksListResult(
        IReadOnlyList<AccountLock> accountLocks,
        DateOnly? earliestLockDate,
        DateOnly? latestLockDate)
    {
        AccountLocks = accountLocks ?? throw new ArgumentNullException(nameof(accountLocks));
        EarliestLockDate = earliestLockDate;
        LatestLockDate = latestLockDate;
    }

    /// <summary>
    /// Account locks for the company.
    /// </summary>
    public IReadOnlyList<AccountLock> AccountLocks { get; }

    /// <summary>
    /// Earliest date at which a user account lock may be set. Absent when a user lock cannot be set.
    /// </summary>
    public DateOnly? EarliestLockDate { get; }

    /// <summary>
    /// Latest date at which a user account lock may be set.
    /// </summary>
    public DateOnly? LatestLockDate { get; }
}
