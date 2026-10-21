using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Payroll;

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
}
