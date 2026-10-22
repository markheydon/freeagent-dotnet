using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/invoices</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceView>))]
public enum InvoiceView
{
    /// <summary>All invoices (default).</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Recent, open, or overdue invoices.</summary>
    [JsonStringEnumMemberName("recent_open_or_overdue")]
    RecentOpenOrOverdue,

    /// <summary>Open invoices only.</summary>
    [JsonStringEnumMemberName("open")]
    Open,

    /// <summary>Overdue invoices only.</summary>
    [JsonStringEnumMemberName("overdue")]
    Overdue,

    /// <summary>Open or overdue invoices.</summary>
    [JsonStringEnumMemberName("open_or_overdue")]
    OpenOrOverdue,

    /// <summary>Draft invoices only.</summary>
    [JsonStringEnumMemberName("draft")]
    Draft,

    /// <summary>Paid invoices only.</summary>
    [JsonStringEnumMemberName("paid")]
    Paid,

    /// <summary>Invoices scheduled to email.</summary>
    [JsonStringEnumMemberName("scheduled_to_email")]
    ScheduledToEmail,

    /// <summary>Invoices with active thank-you emails.</summary>
    [JsonStringEnumMemberName("thank_you_emails")]
    ThankYouEmails,

    /// <summary>Invoices with active reminder emails.</summary>
    [JsonStringEnumMemberName("reminder_emails")]
    ReminderEmails,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
