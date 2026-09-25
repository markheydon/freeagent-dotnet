using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Request envelope for estimate create and update operations.
/// </summary>
internal sealed class EstimateRequest
{
    [JsonPropertyName("estimate")]
    public EstimateWritePayload? Estimate { get; set; }
}
