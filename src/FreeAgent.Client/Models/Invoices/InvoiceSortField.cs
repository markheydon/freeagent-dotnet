using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>sort</c> field values for <c>GET /v2/invoices</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceSortField>))]
public enum InvoiceSortField
{
    /// <summary>Sort by creation time (default).</summary>
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,

    /// <summary>Sort by last update time.</summary>
    [JsonStringEnumMemberName("updated_at")]
    UpdatedAt,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
