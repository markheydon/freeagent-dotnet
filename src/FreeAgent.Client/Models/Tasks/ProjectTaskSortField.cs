using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>sort</c> field values for <c>GET /v2/tasks</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ProjectTaskSortField>))]
public enum ProjectTaskSortField
{
    /// <summary>Sort by task name (default).</summary>
    [JsonStringEnumMemberName("name")]
    Name,

    /// <summary>Sort by the project associated with the task.</summary>
    [JsonStringEnumMemberName("project")]
    Project,

    /// <summary>Sort by billing rate.</summary>
    [JsonStringEnumMemberName("billing_rate")]
    BillingRate,

    /// <summary>Sort by creation time.</summary>
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,

    /// <summary>Sort by last update time.</summary>
    [JsonStringEnumMemberName("updated_at")]
    UpdatedAt,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
