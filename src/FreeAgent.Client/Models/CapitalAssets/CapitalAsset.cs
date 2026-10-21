using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent capital asset.
/// </summary>
/// <remarks>
/// Capital assets are read-only in the FreeAgent API and are typically created via expenses or bank explanations.
/// </remarks>
public sealed class CapitalAsset : IFreeAgentResource
{
    /// <summary>
    /// Capital asset resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Deprecated asset life in years for straight line depreciation; otherwise <c>0</c>. Prefer <see cref="DepreciationProfile"/>.
    /// </summary>
    [JsonPropertyName("asset_life_years")]
    public int? AssetLifeYears { get; set; }

    /// <summary>
    /// Depreciation calculation profile for the asset.
    /// </summary>
    [JsonPropertyName("depreciation_profile")]
    public DepreciationProfile? DepreciationProfile { get; set; }

    /// <summary>
    /// User-created custom asset type name or a system value from the API docs.
    /// </summary>
    [JsonPropertyName("asset_type")]
    public string? AssetType { get; set; }

    /// <summary>
    /// When the asset was purchased.
    /// </summary>
    [JsonPropertyName("purchased_on")]
    public DateOnly? PurchasedOn { get; set; }

    /// <summary>
    /// When the asset was disposed of.
    /// </summary>
    [JsonPropertyName("disposed_on")]
    public DateOnly? DisposedOn { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Lifecycle events when <c>include_history=true</c> was requested.
    /// </summary>
    [JsonPropertyName("capital_asset_history")]
    public List<CapitalAssetHistoryEntry>? CapitalAssetHistory { get; set; }
}
