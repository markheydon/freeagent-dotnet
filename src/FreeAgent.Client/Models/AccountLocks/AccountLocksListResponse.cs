using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wire envelope for listing account locks.
/// </summary>
internal sealed class AccountLocksListResponse
{
    /// <summary>
    /// Account locks returned by FreeAgent.
    /// </summary>
    [JsonPropertyName("account_locks")]
    public List<AccountLock>? AccountLocks { get; set; }

    /// <summary>
    /// Earliest date a user lock may be set.
    /// </summary>
    [JsonPropertyName("earliest_lock_date")]
    public DateOnly? EarliestLockDate { get; set; }

    /// <summary>
    /// Latest date a user lock may be set.
    /// </summary>
    [JsonPropertyName("latest_lock_date")]
    public DateOnly? LatestLockDate { get; set; }
}
