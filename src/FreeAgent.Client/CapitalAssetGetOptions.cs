namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.CapitalAssets.CapitalAssetsService.GetCapitalAssetAsync(long, CapitalAssetGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class CapitalAssetGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, requests <c>include_history=true</c> so lifecycle events are returned on the capital asset.
    /// </summary>
    public bool IncludeHistory { get; init; }
}
