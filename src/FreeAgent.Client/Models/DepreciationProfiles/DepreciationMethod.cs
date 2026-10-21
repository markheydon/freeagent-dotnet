using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.DepreciationProfiles;

/// <summary>
/// Depreciation calculation method for a capital asset purchase.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<DepreciationMethod>))]
public enum DepreciationMethod
{
    /// <summary>Straight line depreciation.</summary>
    [JsonStringEnumMemberName("straight_line")]
    StraightLine,

    /// <summary>Reducing balance depreciation.</summary>
    [JsonStringEnumMemberName("reducing_balance")]
    ReducingBalance,

    /// <summary>No depreciation ledgers are posted.</summary>
    [JsonStringEnumMemberName("no_depreciation")]
    NoDepreciation
}
