using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Response envelope for estimate list operations.
/// </summary>
public sealed class EstimatesResponse
{
    /// <summary>
    /// Estimates returned by the list endpoint.
    /// </summary>
    [JsonPropertyName("estimates")]
    public List<Estimate>? Estimates { get; set; }
}
