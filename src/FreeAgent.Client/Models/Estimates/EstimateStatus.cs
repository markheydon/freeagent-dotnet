using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Estimate status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<EstimateStatus>))]
public enum EstimateStatus
{
    /// <summary>Draft estimate.</summary>
    [JsonStringEnumMemberName("Draft")]
    Draft,

    /// <summary>Sent estimate.</summary>
    [JsonStringEnumMemberName("Sent")]
    Sent,

    /// <summary>Open estimate.</summary>
    [JsonStringEnumMemberName("Open")]
    Open,

    /// <summary>Approved estimate.</summary>
    [JsonStringEnumMemberName("Approved")]
    Approved,

    /// <summary>Rejected estimate.</summary>
    [JsonStringEnumMemberName("Rejected")]
    Rejected,

    /// <summary>Estimate converted to an invoice.</summary>
    [JsonStringEnumMemberName("Invoiced")]
    Invoiced
}
