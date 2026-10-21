using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Represents a FreeAgent landlord property (rental unit address).
/// </summary>
/// <remarks>
/// Only applicable when the company type is <c>UkUnincorporatedLandlord</c>.
/// </remarks>
public sealed class LandlordProperty : IFreeAgentResource
{
    /// <summary>
    /// Property resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// First line of the property address.
    /// </summary>
    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    /// <summary>
    /// Second line of the property address.
    /// </summary>
    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    /// <summary>
    /// Third line of the property address.
    /// </summary>
    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    /// <summary>
    /// Town of the property address.
    /// </summary>
    [JsonPropertyName("town")]
    public string? Town { get; set; }

    /// <summary>
    /// Region of the property address.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// Postcode of the property address.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    /// <summary>
    /// Country of the property address. Defaults to United Kingdom on the API.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }
}
