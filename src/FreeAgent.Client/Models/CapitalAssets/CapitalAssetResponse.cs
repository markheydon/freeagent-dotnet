using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for a single capital asset API response.
/// </summary>
public sealed class CapitalAssetResponse
{
    /// <summary>
    /// Capital asset payload.
    /// </summary>
    [JsonPropertyName("capital_asset")]
    public CapitalAsset? CapitalAsset { get; set; }
}
