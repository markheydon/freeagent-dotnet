using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Bank feed connection state returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankFeedState>))]
public enum BankFeedState
{
    /// <summary>Feed is enabled (wire value: <c>enabled</c>).</summary>
    [JsonStringEnumMemberName("enabled")]
    Enabled,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
