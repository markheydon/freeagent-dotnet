using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Timeslip grouping options when creating or updating an invoice.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceIncludeTimeslips>))]
public enum InvoiceIncludeTimeslips
{
    /// <summary>One line grouped by a single timeslip.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_single_timeslip")]
    BilledGroupedBySingleTimeslip,

    /// <summary>Separate lines grouped by timeslip.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_timeslip")]
    BilledGroupedByTimeslip,

    /// <summary>Separate lines grouped by timeslip task.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_timeslip_task")]
    BilledGroupedByTimeslipTask,

    /// <summary>Separate lines grouped by timeslip date.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_timeslip_date")]
    BilledGroupedByTimeslipDate
}
