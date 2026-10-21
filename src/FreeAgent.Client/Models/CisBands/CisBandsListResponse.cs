using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CisBands;

/// <summary>
/// Response envelope for listing CIS bands.
/// </summary>
internal sealed class CisBandsListResponse
{
    /// <summary>
    /// CIS bands configured for the company.
    /// </summary>
    [JsonPropertyName("available_bands")]
    public List<CisBand>? AvailableBands { get; set; }
}
