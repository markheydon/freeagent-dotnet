using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CorporationTaxReturns;

/// <summary>
/// Corporation tax return filing status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CorporationTaxReturnFilingStatus>))]
public enum CorporationTaxReturnFilingStatus
{
    /// <summary>Draft return.</summary>
    [JsonStringEnumMemberName("draft")]
    Draft,

    /// <summary>Unfiled return.</summary>
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
}
