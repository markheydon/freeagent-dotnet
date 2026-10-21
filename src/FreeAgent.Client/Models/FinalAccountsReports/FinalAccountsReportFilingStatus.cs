using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Final accounts report filing status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<FinalAccountsReportFilingStatus>))]
public enum FinalAccountsReportFilingStatus
{
    /// <summary>Draft report.</summary>
    [JsonStringEnumMemberName("draft")]
    Draft,

    /// <summary>Unfiled report.</summary>
    [JsonStringEnumMemberName("unfiled")]
    Unfiled,

    /// <summary>Pending submission.</summary>
    [JsonStringEnumMemberName("pending")]
    Pending,

    /// <summary>Rejected submission.</summary>
    [JsonStringEnumMemberName("rejected")]
    Rejected,

    /// <summary>Filed online.</summary>
    [JsonStringEnumMemberName("filed")]
    Filed,

    /// <summary>Marked as filed manually.</summary>
    [JsonStringEnumMemberName("marked_as_filed")]
    MarkedAsFiled,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
