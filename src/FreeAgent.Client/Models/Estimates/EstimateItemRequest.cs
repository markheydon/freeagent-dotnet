using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Request envelope for estimate item update operations.
/// </summary>
internal sealed class EstimateItemRequest
{
    [JsonPropertyName("estimate_item")]
    public EstimateItemWritePayload? EstimateItem { get; set; }
}
