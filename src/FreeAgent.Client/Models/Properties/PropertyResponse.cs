using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Response envelope for a single property.
/// </summary>
internal sealed class PropertyResponse
{
    /// <summary>
    /// Property payload.
    /// </summary>
    [JsonPropertyName("property")]
    public LandlordProperty? Property { get; set; }
}
