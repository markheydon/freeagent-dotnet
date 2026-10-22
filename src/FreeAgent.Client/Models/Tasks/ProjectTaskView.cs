using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/tasks</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectTaskView>))]
public enum ProjectTaskView
{
    /// <summary>All tasks (default).</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Active tasks only.</summary>
    [JsonStringEnumMemberName("active")]
    Active,

    /// <summary>Completed tasks only.</summary>
    [JsonStringEnumMemberName("completed")]
    Completed,

    /// <summary>Hidden tasks only.</summary>
    [JsonStringEnumMemberName("hidden")]
    Hidden,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
