using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// VAT status values for bank transaction explanations.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankTransactionExplanationEcStatus>))]
public enum BankTransactionExplanationEcStatus
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
    EcVatMoss,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
