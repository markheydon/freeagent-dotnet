using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Bill VAT status values for reporting purposes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillEcStatus>))]
public enum BillEcStatus
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
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
