using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Minimal landlord property resource shape returned on journal entry links.
/// </summary>
/// <remarks>
/// Full property REST operations are not yet implemented in the SDK.
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
}
