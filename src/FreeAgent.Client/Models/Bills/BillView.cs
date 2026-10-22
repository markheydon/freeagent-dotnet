using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/bills</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillView>))]
public enum BillView
{
    /// <summary>All bills (default).</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Open bills only.</summary>
    [JsonStringEnumMemberName("open")]
    Open,

    /// <summary>Overdue bills only.</summary>
    [JsonStringEnumMemberName("overdue")]
    Overdue,

    /// <summary>Open or overdue bills.</summary>
    [JsonStringEnumMemberName("open_or_overdue")]
    OpenOrOverdue,

    /// <summary>Open or overdue bill payments (positive total value).</summary>
    [JsonStringEnumMemberName("open_or_overdue_payments")]
    OpenOrOverduePayments,

    /// <summary>Open or overdue bill refunds (negative total value).</summary>
    [JsonStringEnumMemberName("open_or_overdue_refunds")]
    OpenOrOverdueRefunds,

    /// <summary>Paid bills only.</summary>
    [JsonStringEnumMemberName("paid")]
    Paid,

    /// <summary>Recurring bills only.</summary>
    [JsonStringEnumMemberName("recurring")]
    Recurring,

    /// <summary>Hire purchase bills only.</summary>
    [JsonStringEnumMemberName("hire_purchase")]
    HirePurchase,

    /// <summary>Bills with CIS bill items only.</summary>
    [JsonStringEnumMemberName("cis")]
    Cis,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
