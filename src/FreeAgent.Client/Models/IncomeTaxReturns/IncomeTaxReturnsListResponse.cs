using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.IncomeTaxReturns;

/// <summary>
/// Response envelope for listing income tax returns.
/// </summary>
internal sealed class IncomeTaxReturnsListResponse
{
    /// <summary>
    /// Income tax returns for the user.
    /// </summary>
    [JsonPropertyName("self_assessment_returns")]
    public List<IncomeTaxReturn>? IncomeTaxReturns { get; set; }
}
