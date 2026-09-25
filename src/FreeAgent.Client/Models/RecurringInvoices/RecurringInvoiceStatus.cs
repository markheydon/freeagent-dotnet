using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.RecurringInvoices;

/// <summary>
/// Recurring invoice status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<RecurringInvoiceStatus>))]
public enum RecurringInvoiceStatus
{
    /// <summary>Draft recurring invoice.</summary>
    [JsonStringEnumMemberName("Draft")]
    Draft,

    /// <summary>Active recurring invoice.</summary>
    [JsonStringEnumMemberName("Active")]
    Active
}
