using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Response envelope for single-estimate-item operations.
/// </summary>
public sealed class EstimateItemResponse
{
    /// <summary>
    /// Estimate item returned by the API.
    /// </summary>
    [JsonPropertyName("estimate_item")]
    public EstimateItem? EstimateItem { get; set; }
}
