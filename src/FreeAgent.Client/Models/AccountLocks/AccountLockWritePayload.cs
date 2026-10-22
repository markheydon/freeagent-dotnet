using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Payload for creating or updating the user account lock.
/// </summary>
public sealed class AccountLockWritePayload
{
    /// <summary>
    /// Date through which the account should be locked.
    /// </summary>
    [JsonPropertyName("locked_to_date")]
    public DateOnly LockedToDate { get; set; }
}
