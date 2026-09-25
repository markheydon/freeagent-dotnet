using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Wrapper for price list items list API responses.
/// </summary>
public sealed class PriceListItemsResponse
{
    /// <summary>
    /// Price list items list payload.
    /// </summary>
    [JsonPropertyName("price_list_items")]
    public List<PriceListItem>? PriceListItems { get; set; }
}
