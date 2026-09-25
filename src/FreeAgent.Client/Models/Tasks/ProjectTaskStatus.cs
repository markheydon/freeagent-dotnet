using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Task status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectTaskStatus>))]
public enum ProjectTaskStatus
{
    /// <summary>Active task (wire value: "Active").</summary>
    [JsonStringEnumMemberName("Active")]
    Active,

    /// <summary>Completed task (wire value: "Completed").</summary>
    [JsonStringEnumMemberName("Completed")]
    Completed,

    /// <summary>Hidden task (wire value: "Hidden").</summary>
    [JsonStringEnumMemberName("Hidden")]
    Hidden
}
