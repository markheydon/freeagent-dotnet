using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.IncomeTaxReturns;

/// <summary>
/// Income tax return payment status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<IncomeTaxReturnPaymentStatus>))]
public enum IncomeTaxReturnPaymentStatus
{
    /// <summary>Payment outstanding.</summary>
    [JsonStringEnumMemberName("unpaid")]
    Unpaid,

    /// <summary>Payment marked as paid manually.</summary>
    [JsonStringEnumMemberName("marked_as_paid")]
    MarkedAsPaid,

    /// <summary>Reserved for future use.</summary>
    [JsonStringEnumMemberName("paid")]
    Paid,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
