using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for a single contact API response.
/// </summary>
public class ContactResponse
{
    /// <summary>
    /// Contact payload.
    /// </summary>
    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }
}
