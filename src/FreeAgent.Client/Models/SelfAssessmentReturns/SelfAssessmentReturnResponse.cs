using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.SelfAssessmentReturns;

/// <summary>
/// Response envelope for a single self assessment return.
/// </summary>
internal sealed class SelfAssessmentReturnResponse
{
    /// <summary>
    /// Self assessment return payload.
    /// </summary>
    [JsonPropertyName("self_assessment_return")]
    public SelfAssessmentReturn? SelfAssessmentReturn { get; set; }
}
