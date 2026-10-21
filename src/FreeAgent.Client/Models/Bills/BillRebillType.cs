using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Project rebill calculation type for a bill.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillRebillType>))]
public enum BillRebillType
{
    /// <summary>Rebill at cost.</summary>
    [JsonStringEnumMemberName("cost")]
    Cost,

    /// <summary>Rebill with markup.</summary>
    [JsonStringEnumMemberName("markup")]
    Markup,

    /// <summary>Rebill at a fixed price factor.</summary>
    [JsonStringEnumMemberName("price")]
    Price,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
