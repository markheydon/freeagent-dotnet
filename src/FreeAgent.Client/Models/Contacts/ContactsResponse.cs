using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Wrapper for contacts list API responses.
/// </summary>
public class ContactsResponse
{
    /// <summary>
    /// Contact list payload.
    /// </summary>
    [JsonPropertyName("contacts")]
    public List<ContactSummary>? Contacts { get; set; }
}
