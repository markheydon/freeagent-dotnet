using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.VatReturns;

/// <summary>
/// VAT return payment status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<VatReturnPaymentStatus>))]
public enum VatReturnPaymentStatus
{
    /// <summary>Payment not yet recorded as paid.</summary>
    [JsonStringEnumMemberName("unpaid")]
    Unpaid,

    /// <summary>Payment marked as paid.</summary>
    [JsonStringEnumMemberName("marked_as_paid")]
    MarkedAsPaid,
}
