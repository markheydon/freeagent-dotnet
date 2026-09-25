using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Estimate document type values.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<EstimateType>))]
public enum EstimateType
{
    /// <summary>Standard estimate.</summary>
    [JsonStringEnumMemberName("Estimate")]
    Estimate,

    /// <summary>Quote document.</summary>
    [JsonStringEnumMemberName("Quote")]
    Quote,

    /// <summary>Proposal document.</summary>
    [JsonStringEnumMemberName("Proposal")]
    Proposal
}
