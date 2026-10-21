using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CisSettings;

/// <summary>
/// Response envelope for CIS settings.
/// </summary>
internal sealed class CisSettingsResponse
{
    /// <summary>
    /// CIS settings payload.
    /// </summary>
    [JsonPropertyName("cis_settings")]
    public CisSettings? CisSettings { get; set; }
}
