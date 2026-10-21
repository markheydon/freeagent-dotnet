using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for capital assets list API responses.
/// </summary>
public sealed class CapitalAssetsListResponse
{
    /// <summary>
    /// Capital assets list payload.
    /// </summary>
    [JsonPropertyName("capital_assets")]
    public List<CapitalAsset>? CapitalAssets { get; set; }
}
