using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CorporationTaxReturns;

/// <summary>
/// Corporation tax return payment status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CorporationTaxReturnPaymentStatus>))]
public enum CorporationTaxReturnPaymentStatus
{
    /// <summary>Payment not yet recorded as paid.</summary>
    [JsonStringEnumMemberName("unpaid")]
    Unpaid,

    /// <summary>Payment marked as paid.</summary>
    [JsonStringEnumMemberName("marked_as_paid")]
    MarkedAsPaid,

    /// <summary>Reserved for future use by FreeAgent.</summary>
    [JsonStringEnumMemberName("paid")]
    Paid,
}
