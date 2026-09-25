using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// UK VAT status values for price list items.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<PriceListItemVatStatus>))]
public enum PriceListItemVatStatus
{
    /// <summary>Item is out of scope for VAT (default).</summary>
    [JsonStringEnumMemberName("out_of_scope")]
    OutOfScope,

    /// <summary>Reduced rate VAT.</summary>
    [JsonStringEnumMemberName("reduced")]
    Reduced,

    /// <summary>Standard rate VAT.</summary>
    [JsonStringEnumMemberName("standard")]
    Standard,

    /// <summary>Zero rate VAT.</summary>
    [JsonStringEnumMemberName("zero")]
    Zero
}
