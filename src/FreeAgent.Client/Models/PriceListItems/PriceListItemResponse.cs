using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Wrapper for a single price list item API response.
/// </summary>
public sealed class PriceListItemResponse
{
    /// <summary>
    /// Price list item payload.
    /// </summary>
    [JsonPropertyName("price_list_item")]
    public PriceListItem? PriceListItem { get; set; }
}
