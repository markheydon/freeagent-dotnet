using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Request envelope for creating or updating a contact.
/// </summary>
internal class ContactRequest
{
    /// <summary>
    /// Contact payload.
    /// </summary>
    [JsonPropertyName("contact")]
    public ContactWritePayload Contact { get; set; } = new();
}
