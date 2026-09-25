using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Response envelope for single-estimate operations.
/// </summary>
public sealed class EstimateResponse
{
    /// <summary>
    /// Estimate returned by the API.
    /// </summary>
    [JsonPropertyName("estimate")]
    public Estimate? Estimate { get; set; }
}
