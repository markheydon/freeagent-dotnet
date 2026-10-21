using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for listing self assessment returns.
/// </summary>
internal sealed class SelfAssessmentReturnsListResponse
{
    /// <summary>
    /// Self assessment returns for the user.
    /// </summary>
    [JsonPropertyName("self_assessment_returns")]
    public List<SelfAssessmentReturn>? SelfAssessmentReturns { get; set; }
}
