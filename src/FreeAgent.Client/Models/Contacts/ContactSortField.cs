using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>sort</c> field values for <c>GET /v2/contacts</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ContactSortField>))]
public enum ContactSortField
{
    /// <summary>Sort by organisation and name (default).</summary>
    [JsonStringEnumMemberName("name")]
    Name,

    /// <summary>Sort by creation time.</summary>
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,

    /// <summary>Sort by last update time.</summary>
    [JsonStringEnumMemberName("updated_at")]
    UpdatedAt,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
