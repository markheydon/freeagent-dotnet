using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.RecurringInvoices;

/// <summary>
/// Recurring invoice schedule frequency values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<RecurringInvoiceFrequency>))]
public enum RecurringInvoiceFrequency
{
    /// <summary>Weekly recurrence.</summary>
    [JsonStringEnumMemberName("Weekly")]
    Weekly,

    /// <summary>Two weekly recurrence.</summary>
    [JsonStringEnumMemberName("Two Weekly")]
    TwoWeekly,

    /// <summary>Four weekly recurrence.</summary>
    [JsonStringEnumMemberName("Four Weekly")]
    FourWeekly,

    /// <summary>Monthly recurrence.</summary>
    [JsonStringEnumMemberName("Monthly")]
    Monthly,

    /// <summary>Two monthly recurrence.</summary>
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

    /// <summary>Two yearly recurrence.</summary>
    [JsonStringEnumMemberName("2-Yearly")]
    TwoYearly
}
