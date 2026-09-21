using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Billing period values for <see cref="Project.NormalBillingRate"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectBillingPeriod>))]
public enum ProjectBillingPeriod
{
    /// <summary>Hourly billing period (wire value: "hour").</summary>
    [JsonStringEnumMemberName("hour")]
    Hour,

    /// <summary>Daily billing period (wire value: "day").</summary>
    [JsonStringEnumMemberName("day")]
    Day
}
