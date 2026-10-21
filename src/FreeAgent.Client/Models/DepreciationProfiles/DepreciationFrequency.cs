using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.DepreciationProfiles;

/// <summary>
/// How often depreciation ledgers are posted for a capital asset.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<DepreciationFrequency>))]
public enum DepreciationFrequency
{
    /// <summary>Depreciation ledgers are posted monthly.</summary>
    [JsonStringEnumMemberName("monthly")]
    Monthly,

    /// <summary>Depreciation ledgers are posted annually.</summary>
    [JsonStringEnumMemberName("annually")]
    Annually
}
