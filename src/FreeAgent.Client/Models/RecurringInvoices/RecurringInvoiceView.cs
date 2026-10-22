using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/recurring_invoices</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<RecurringInvoiceView>))]
public enum RecurringInvoiceView
{
    /// <summary>Draft recurring invoices only.</summary>
    [JsonStringEnumMemberName("draft")]
    Draft,

    /// <summary>Active recurring invoices only.</summary>
    [JsonStringEnumMemberName("active")]
    Active,

    /// <summary>Inactive recurring invoices only.</summary>
    [JsonStringEnumMemberName("inactive")]
    Inactive,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
