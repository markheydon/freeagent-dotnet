using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Wrapper for price list item write API requests.
/// </summary>
internal sealed class PriceListItemRequest
{
    /// <summary>
    /// Price list item payload.
    /// </summary>
    [JsonPropertyName("price_list_item")]
    public object? PriceListItem { get; set; }
}
