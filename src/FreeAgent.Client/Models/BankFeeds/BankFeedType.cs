using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.BankFeeds;

/// <summary>
/// How a bank feed imports transactions into a bank account.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankFeedType>))]
public enum BankFeedType
{
    /// <summary>
    /// API bank feed (for example Open Banking aggregator via API).
    /// </summary>
    [JsonStringEnumMemberName("api")]
    Api,

    /// <summary>
    /// Open Banking bank feed.
    /// </summary>
    [JsonStringEnumMemberName("open_banking")]
    OpenBanking,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
