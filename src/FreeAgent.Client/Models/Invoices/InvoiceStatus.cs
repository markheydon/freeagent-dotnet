using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Invoice status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceStatus>))]
public enum InvoiceStatus
{
    /// <summary>Draft invoice.</summary>
    [JsonStringEnumMemberName("Draft")]
    Draft,

    /// <summary>Scheduled to email.</summary>
    [JsonStringEnumMemberName("Scheduled To Email")]
    ScheduledToEmail,

    /// <summary>Open invoice.</summary>
    [JsonStringEnumMemberName("Open")]
    Open,

    /// <summary>Zero value invoice.</summary>
    [JsonStringEnumMemberName("Zero Value")]
    ZeroValue,

    /// <summary>Overdue invoice.</summary>
    [JsonStringEnumMemberName("Overdue")]
    Overdue,

    /// <summary>Paid invoice.</summary>
    [JsonStringEnumMemberName("Paid")]
    Paid,

    /// <summary>Overpaid invoice.</summary>
    [JsonStringEnumMemberName("Overpaid")]
    Overpaid,

    /// <summary>Refunded invoice.</summary>
    [JsonStringEnumMemberName("Refunded")]
    Refunded,

    /// <summary>Written-off invoice.</summary>
    [JsonStringEnumMemberName("Written-off")]
    WrittenOff,

    /// <summary>Part written-off invoice.</summary>
    [JsonStringEnumMemberName("Part written-off")]
    PartWrittenOff
}
