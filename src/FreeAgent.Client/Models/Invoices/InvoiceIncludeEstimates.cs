using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Estimate grouping options when creating or updating an invoice.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceIncludeEstimates>))]
public enum InvoiceIncludeEstimates
{
    /// <summary>One line grouped by a single estimate.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_single_estimate")]
    BilledGroupedBySingleEstimate,

    /// <summary>Separate lines grouped by estimate.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_estimate")]
    BilledGroupedByEstimate
}
