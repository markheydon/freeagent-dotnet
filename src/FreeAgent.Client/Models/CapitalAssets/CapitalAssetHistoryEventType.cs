using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CapitalAssets;

/// <summary>
/// Documented <c>type</c> values on capital asset history entries.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CapitalAssetHistoryEventType>))]
public enum CapitalAssetHistoryEventType
{
    /// <summary>
    /// Asset purchase event.
    /// </summary>
    [JsonStringEnumMemberName("purchase")]
    Purchase,

    /// <summary>
    /// Depreciation event.
    /// </summary>
    [JsonStringEnumMemberName("depreciation")]
    Depreciation,

    /// <summary>
    /// Annual investment allowance event.
    /// </summary>
    [JsonStringEnumMemberName("annual_investment_allowance")]
    AnnualInvestmentAllowance,

    /// <summary>
    /// Asset disposal event.
    /// </summary>
    [JsonStringEnumMemberName("disposal")]
    Disposal
}
