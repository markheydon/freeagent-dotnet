using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/projects</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectView>))]
public enum ProjectView
{
    /// <summary>Active projects only.</summary>
    [JsonStringEnumMemberName("active")]
    Active,

    /// <summary>Completed projects only.</summary>
    [JsonStringEnumMemberName("completed")]
    Completed,

    /// <summary>Cancelled projects only.</summary>
    [JsonStringEnumMemberName("cancelled")]
    Cancelled,

    /// <summary>Hidden projects only.</summary>
    [JsonStringEnumMemberName("hidden")]
    Hidden,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
