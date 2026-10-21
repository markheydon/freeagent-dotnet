using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for listing bank feeds.
/// </summary>
public sealed class BankFeedsListResponse
{
    /// <summary>
    /// Bank feeds returned by the API.
    /// </summary>
    [JsonPropertyName("bank_feeds")]
    public List<BankFeed>? BankFeeds { get; set; }
}
