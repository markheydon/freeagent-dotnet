using FreeAgent.Client.Infrastructure.Serialization;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents VAT accounting basis values used by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InitialVatBasis>))]
public enum InitialVatBasis
{
    /// <summary>Cash basis (wire value: "Cash").</summary>
    [JsonStringEnumMemberName("Cash")]
    Cash,

    /// <summary>Invoice basis (wire value: "Invoice").</summary>
    [JsonStringEnumMemberName("Invoice")]
    Invoice
}
