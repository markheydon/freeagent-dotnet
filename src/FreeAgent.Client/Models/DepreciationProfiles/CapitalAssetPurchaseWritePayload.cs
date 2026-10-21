using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Writable <c>capital_asset</c> namespace for purchase-related entities.
/// </summary>
internal sealed class CapitalAssetPurchaseWritePayload
{
    [JsonPropertyName("depreciation_profile")]
    public DepreciationProfileWritePayload DepreciationProfile { get; set; } = null!;

    public static CapitalAssetPurchaseWritePayload? FromWrite(CapitalAssetPurchaseWrite? write)
    {
        if (write is null)
        {
            return null;
        }

        return new CapitalAssetPurchaseWritePayload
        {
            DepreciationProfile = write.DepreciationProfile
        };
    }
}
