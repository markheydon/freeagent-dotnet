using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CapitalAssetTypes;

/// <summary>
/// Response envelope for listing capital asset types.
/// </summary>
internal sealed class CapitalAssetTypesListResponse
{
    /// <summary>
    /// Capital asset types returned by the API.
    /// </summary>
    [JsonPropertyName("capital_asset_types")]
    public List<CapitalAssetType>? CapitalAssetTypes { get; set; }
}
