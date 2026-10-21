using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Response envelope for a list of properties.
/// </summary>
internal sealed class PropertyListResponse
{
    /// <summary>
    /// Property collection payload.
    /// </summary>
    [JsonPropertyName("properties")]
    public List<LandlordProperty>? Properties { get; set; }
}
