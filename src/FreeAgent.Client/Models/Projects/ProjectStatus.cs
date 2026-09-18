using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Project status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectStatus>))]
public enum ProjectStatus
{
    /// <summary>Active project (wire value: "Active").</summary>
    [JsonStringEnumMemberName("Active")]
    Active,

    /// <summary>Completed project (wire value: "Completed").</summary>
    [JsonStringEnumMemberName("Completed")]
    Completed,

    /// <summary>Cancelled project (wire value: "Cancelled").</summary>
    [JsonStringEnumMemberName("Cancelled")]
    Cancelled,

    /// <summary>Hidden project (wire value: "Hidden").</summary>
    [JsonStringEnumMemberName("Hidden")]
    Hidden
}
