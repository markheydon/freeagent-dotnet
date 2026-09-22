using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Billing period values for <see cref="Task.BillingRate"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<TaskBillingPeriod>))]
public enum TaskBillingPeriod
{
    /// <summary>Hourly billing period (wire value: "hour").</summary>
    [JsonStringEnumMemberName("hour")]
    Hour,

    /// <summary>Daily billing period (wire value: "day").</summary>
    [JsonStringEnumMemberName("day")]
    Day
}
