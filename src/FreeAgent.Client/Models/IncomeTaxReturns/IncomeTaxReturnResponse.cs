using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a single income tax return.
/// </summary>
internal sealed class IncomeTaxReturnResponse
{
    /// <summary>
    /// Income tax return payload.
    /// </summary>
    [JsonPropertyName("self_assessment_return")]
    public IncomeTaxReturn? IncomeTaxReturn { get; set; }
}
