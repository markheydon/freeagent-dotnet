using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>sort</c> field values for <c>GET /v2/projects</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectSortField>))]
public enum ProjectSortField
{
    /// <summary>Sort by project name (default).</summary>
    [JsonStringEnumMemberName("name")]
    Name,

    /// <summary>Sort by contact organisation and name (organisation, last, first).</summary>
    [JsonStringEnumMemberName("contact_name")]
    ContactName,

    /// <summary>Sort by contact display name (organisation, first, last).</summary>
    [JsonStringEnumMemberName("contact_display_name")]
    ContactDisplayName,

    /// <summary>Sort by creation time.</summary>
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,

    /// <summary>Sort by last update time.</summary>
    [JsonStringEnumMemberName("updated_at")]
    UpdatedAt,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
