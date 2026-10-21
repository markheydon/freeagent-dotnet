using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a single bank feed.
/// </summary>
public sealed class BankFeedResponse
{
    /// <summary>
    /// Bank feed returned by the API.
    /// </summary>
    [JsonPropertyName("bank_feed")]
    public BankFeed? BankFeed { get; set; }
}
