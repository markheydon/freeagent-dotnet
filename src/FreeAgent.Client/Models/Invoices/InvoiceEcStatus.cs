using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Invoice VAT status values for reporting purposes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceEcStatus>))]
public enum InvoiceEcStatus
{
    /// <summary>UK or non-EC supply.</summary>
    [JsonStringEnumMemberName("UK/Non-EC")]
    UkNonEc,

    /// <summary>EC goods supply.</summary>
    [JsonStringEnumMemberName("EC Goods")]
    EcGoods,

    /// <summary>EC services supply.</summary>
    [JsonStringEnumMemberName("EC Services")]
    EcServices,

    /// <summary>Reverse charge supply.</summary>
    [JsonStringEnumMemberName("Reverse Charge")]
    ReverseCharge,

    /// <summary>EC VAT MOSS supply.</summary>
    [JsonStringEnumMemberName("EC VAT MOSS")]
    EcVatMoss
}
