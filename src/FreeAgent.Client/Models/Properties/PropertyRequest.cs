using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Request envelope for property create and update operations.
/// </summary>
internal sealed class PropertyRequest
{
    /// <summary>
    /// Property write payload.
    /// </summary>
    [JsonPropertyName("property")]
    public PropertyWritePayload? Property { get; set; }
}
