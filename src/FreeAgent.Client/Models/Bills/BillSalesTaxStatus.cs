using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Bill line item sales tax status values.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillSalesTaxStatus>))]
public enum BillSalesTaxStatus
{
    /// <summary>Item is taxable.</summary>
    [JsonStringEnumMemberName("TAXABLE")]
    Taxable,

    /// <summary>Item is exempt from sales tax.</summary>
    [JsonStringEnumMemberName("EXEMPT")]
    Exempt,

    /// <summary>Item is out of scope for sales tax.</summary>
    [JsonStringEnumMemberName("OUT_OF_SCOPE")]
    OutOfScope,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
