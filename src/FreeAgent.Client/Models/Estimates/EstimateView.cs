using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/estimates</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<EstimateView>))]
public enum EstimateView
{
    /// <summary>All estimates (default).</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Recent estimates only.</summary>
    [JsonStringEnumMemberName("recent")]
    Recent,

    /// <summary>Draft estimates only.</summary>
    [JsonStringEnumMemberName("draft")]
    Draft,

    /// <summary>Non-draft estimates only.</summary>
    [JsonStringEnumMemberName("non_draft")]
    NonDraft,

    /// <summary>Sent estimates only.</summary>
    [JsonStringEnumMemberName("sent")]
    Sent,

    /// <summary>Approved estimates only.</summary>
    [JsonStringEnumMemberName("approved")]
    Approved,

    /// <summary>Rejected estimates only.</summary>
    [JsonStringEnumMemberName("rejected")]
    Rejected,

    /// <summary>Invoiced estimates only.</summary>
    [JsonStringEnumMemberName("invoiced")]
    Invoiced,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
