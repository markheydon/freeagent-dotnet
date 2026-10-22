using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>sort</c> field values for <c>GET /v2/price_list_items</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<PriceListItemSortField>))]
public enum PriceListItemSortField
{
    /// <summary>Sort by creation time (default).</summary>
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,

    /// <summary>Sort by price list item code.</summary>
    [JsonStringEnumMemberName("code")]
    Code,

    /// <summary>Sort by last update time.</summary>
    [JsonStringEnumMemberName("updated_at")]
    UpdatedAt,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
