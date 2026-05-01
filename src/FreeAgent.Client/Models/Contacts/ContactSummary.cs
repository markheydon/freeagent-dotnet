using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Represents a minimal contact projection used for list and pagination scenarios.
/// </summary>
public class ContactSummary
{
    /// <summary>
    /// Contact resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Contact display name.
    /// </summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>
    /// Contact first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Contact last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Contact organisation name.
    /// </summary>
    [JsonPropertyName("organisation_name")]
    public string? OrganisationName { get; set; }

    /// <summary>
    /// Human-friendly contact name derived from available FreeAgent fields.
    /// </summary>
    [JsonIgnore]
    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ContactName))
            {
                return ContactName;
            }

            if (!string.IsNullOrWhiteSpace(OrganisationName))
            {
                return OrganisationName;
            }

            var fullName = string.Join(" ", new[] { FirstName, LastName }.Where(static x => !string.IsNullOrWhiteSpace(x)));
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            return Url;
        }
    }

    /// <summary>
    /// Captures additional fields returned by FreeAgent for forward compatibility.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalData { get; set; } = new();
}
