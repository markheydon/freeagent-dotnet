using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wire envelope for account lock write requests.
/// </summary>
internal sealed class AccountLockRequest
{
    /// <summary>
    /// Account lock body.
    /// </summary>
    [JsonPropertyName("account_lock")]
    public AccountLockWritePayload AccountLock { get; set; } = null!;
}
