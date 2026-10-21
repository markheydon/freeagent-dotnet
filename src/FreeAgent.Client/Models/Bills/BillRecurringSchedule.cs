using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Recurrence frequency for a bill.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillRecurringSchedule>))]
public enum BillRecurringSchedule
{
    /// <summary>Weekly recurrence.</summary>
    [JsonStringEnumMemberName("Weekly")]
    Weekly,

    /// <summary>Two-weekly recurrence.</summary>
    [JsonStringEnumMemberName("Two Weekly")]
    TwoWeekly,

    /// <summary>Four-weekly recurrence.</summary>
    [JsonStringEnumMemberName("Four Weekly")]
    FourWeekly,

    /// <summary>Two-monthly recurrence.</summary>
    [JsonStringEnumMemberName("Two Monthly")]
    TwoMonthly,

    /// <summary>Quarterly recurrence.</summary>
    [JsonStringEnumMemberName("Quarterly")]
    Quarterly,

    /// <summary>Biannual recurrence.</summary>
    [JsonStringEnumMemberName("Biannually")]
    Biannually,

    /// <summary>Annual recurrence.</summary>
    [JsonStringEnumMemberName("Annually")]
    Annually,

    /// <summary>Two-yearly recurrence.</summary>
    [JsonStringEnumMemberName("2-Yearly")]
    TwoYearly,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
