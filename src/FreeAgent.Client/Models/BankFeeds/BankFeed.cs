using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;

namespace FreeAgent.Client.Models.BankFeeds;

/// <summary>
/// Represents a FreeAgent bank feed connected to a bank account.
/// </summary>
public sealed class BankFeed : IFreeAgentResource
{
    /// <summary>
    /// Bank feed resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Wire representation of the bank account that receives imported transactions.
    /// </summary>
    [JsonPropertyName("bank_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? BankAccountLink { get; set; }

    /// <summary>
    /// Bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? BankAccount => BankAccountLink?.Value;

    /// <summary>
    /// Bank account identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId => BankAccountLink?.Id;

    /// <summary>
    /// Current status of the feed (for example <c>enabled</c>).
    /// </summary>
    /// <remarks>
    /// FreeAgent documents this as a string; only <c>enabled</c> appears in the official examples.
    /// </remarks>
    [JsonPropertyName("state")]
    public BankFeedState? State { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Feed connection type.
    /// </summary>
    [JsonPropertyName("feed_type")]
    public BankFeedType? FeedType { get; set; }

    /// <summary>
    /// Display name of the bank service for this feed.
    /// </summary>
    [JsonPropertyName("bank_service_name")]
    public string? BankServiceName { get; set; }

    /// <summary>
    /// When Strong Customer Authentication expires for an API feed (UTC). Omitted for non-API feeds.
    /// </summary>
    [JsonPropertyName("sca_expires_at")]
    public DateTimeOffset? ScaExpiresAt { get; set; }

    /// <summary>
    /// Attaches a hydrated bank account to this bank feed.
    /// </summary>
    /// <param name="bankAccount">Bank account details.</param>
    internal void AttachBankAccount(BankAccount bankAccount)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        BankAccountLink = new ExpandableField<BankAccount>(bankAccount.Url, bankAccount);
    }
}
