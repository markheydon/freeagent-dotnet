using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Bill payment status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillStatus>))]
public enum BillStatus
{
    /// <summary>Zero-value bill.</summary>
    [JsonStringEnumMemberName("Zero Value")]
    ZeroValue,

    /// <summary>Open bill.</summary>
    [JsonStringEnumMemberName("Open")]
    Open,

    /// <summary>Paid bill.</summary>
    [JsonStringEnumMemberName("Paid")]
    Paid,

    /// <summary>Overdue bill.</summary>
    [JsonStringEnumMemberName("Overdue")]
    Overdue,

    /// <summary>Refunded bill (negative total value and fully paid).</summary>
    [JsonStringEnumMemberName("Refunded")]
    Refunded,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
