using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Project budget unit values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectBudgetUnits>))]
public enum ProjectBudgetUnits
{
    /// <summary>Hours budget unit (wire value: "Hours").</summary>
    [JsonStringEnumMemberName("Hours")]
    Hours,

    /// <summary>Days budget unit (wire value: "Days").</summary>
    [JsonStringEnumMemberName("Days")]
    Days,

    /// <summary>Monetary budget unit (wire value: "Monetary").</summary>
    [JsonStringEnumMemberName("Monetary")]
    Monetary
}
