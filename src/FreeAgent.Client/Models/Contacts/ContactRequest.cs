using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Request envelope for creating or updating a contact.
/// </summary>
public class ContactRequest
{
    /// <summary>
    /// Contact payload.
    /// </summary>
    [JsonPropertyName("contact")]
    public Contact Contact { get; set; } = new();
}
