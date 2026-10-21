using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Invoice line item sales tax status values.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceSalesTaxStatus>))]
public enum InvoiceSalesTaxStatus
{
    /// <summary>Item is taxable.</summary>
    [JsonStringEnumMemberName("TAXABLE")]
    Taxable,

    /// <summary>Item is exempt from sales tax.</summary>
    [JsonStringEnumMemberName("EXEMPT")]
    Exempt,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
