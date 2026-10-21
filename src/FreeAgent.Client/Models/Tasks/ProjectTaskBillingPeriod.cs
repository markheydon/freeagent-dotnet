using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Billing period values for <see cref="ProjectTask.BillingRate"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectTaskBillingPeriod>))]
public enum ProjectTaskBillingPeriod
{
    /// <summary>Hourly billing period (wire value: "hour").</summary>
    [JsonStringEnumMemberName("hour")]
    Hour,

    /// <summary>Daily billing period (wire value: "day").</summary>
    [JsonStringEnumMemberName("day")]
    Day,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
