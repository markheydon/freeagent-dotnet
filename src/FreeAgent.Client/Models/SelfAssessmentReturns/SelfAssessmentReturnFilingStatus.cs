using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.SelfAssessmentReturns;

/// <summary>
/// Self assessment return filing status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<SelfAssessmentReturnFilingStatus>))]
public enum SelfAssessmentReturnFilingStatus
{
    /// <summary>Unfiled return.</summary>
    [JsonStringEnumMemberName("unfiled")]
    Unfiled,

    /// <summary>Pending submission.</summary>
    [JsonStringEnumMemberName("pending")]
    Pending,

    /// <summary>Rejected submission.</summary>
    [JsonStringEnumMemberName("rejected")]
    Rejected,

    /// <summary>Provisionally filed online.</summary>
    [JsonStringEnumMemberName("provisionally_filed")]
    ProvisionallyFiled,

    /// <summary>Filed online.</summary>
    [JsonStringEnumMemberName("filed")]
    Filed,

    /// <summary>Marked as filed manually.</summary>
    [JsonStringEnumMemberName("marked_as_filed")]
    MarkedAsFiled,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
