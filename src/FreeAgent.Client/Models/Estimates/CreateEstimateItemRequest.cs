using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Request envelope for creating an estimate item.
/// </summary>
internal sealed class CreateEstimateItemRequest
{
    [JsonPropertyName("estimate")]
    public EstimateReference? Estimate { get; set; }

    [JsonPropertyName("estimate_item")]
    public EstimateItemWritePayload? EstimateItem { get; set; }
}
