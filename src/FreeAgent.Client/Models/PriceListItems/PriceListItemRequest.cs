using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Wrapper for price list item write API requests.
/// </summary>
/// <typeparam name="T">Create or update payload type.</typeparam>
internal sealed class PriceListItemRequest<T>
{
    /// <summary>
    /// Price list item payload.
    /// </summary>
    [JsonPropertyName("price_list_item")]
    public T PriceListItem { get; set; } = default!;
}
