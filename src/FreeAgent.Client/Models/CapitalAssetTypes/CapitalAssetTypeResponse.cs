using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CapitalAssetTypes;

/// <summary>
/// Response envelope for a single capital asset type.
/// </summary>
internal sealed class CapitalAssetTypeResponse
{
    /// <summary>
    /// Capital asset type payload.
    /// </summary>
    [JsonPropertyName("capital_asset_type")]
    public CapitalAssetType? CapitalAssetType { get; set; }
}
