using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Payroll liability payment status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<PayrollPaymentStatus>))]
public enum PayrollPaymentStatus
{
    /// <summary>Payment not yet recorded as paid.</summary>
    [JsonStringEnumMemberName("unpaid")]
    Unpaid,

    /// <summary>Payment marked as paid.</summary>
    [JsonStringEnumMemberName("marked_as_paid")]
    MarkedAsPaid,

    /// <summary>Payment recorded as paid (reserved for future API use).</summary>
    [JsonStringEnumMemberName("paid")]
    Paid,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
