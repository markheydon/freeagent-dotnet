using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for capital asset type write API requests.
/// </summary>
/// <typeparam name="T">Create or update payload type.</typeparam>
internal sealed class CapitalAssetTypeRequest<T>
{
    /// <summary>
    /// Capital asset type payload.
    /// </summary>
    [JsonPropertyName("capital_asset_type")]
    public T CapitalAssetType { get; set; } = default!;
}
