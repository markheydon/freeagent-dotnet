namespace FreeAgent.Client;

/// <summary>
/// Capital asset purchase attributes for create and update requests on expenses, bill items, and bank transaction explanations.
/// </summary>
public sealed class CapitalAssetPurchaseWrite
{
    internal DepreciationProfileWritePayload DepreciationProfile { get; private init; } = null!;

    /// <summary>
    /// Straight line depreciation with the given asset life in years (2–25).
    /// </summary>
    /// <param name="assetLifeYears">Years before the asset value reaches zero.</param>
    /// <param name="frequency">Ledger posting frequency. Monthly is used when omitted.</param>
    public static CapitalAssetPurchaseWrite StraightLine(int assetLifeYears, DepreciationFrequency? frequency = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(assetLifeYears, 2);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(assetLifeYears, 25);

        return new CapitalAssetPurchaseWrite
        {
            DepreciationProfile = new DepreciationProfileWritePayload
            {
                Method = DepreciationMethod.StraightLine,
                AssetLifeYears = assetLifeYears,
                Frequency = frequency
            }
        };
    }

    /// <summary>
    /// Reducing balance depreciation with the given annual percentage (1–99).
    /// </summary>
    /// <param name="annualDepreciationPercentage">Percentage reduction applied each year.</param>
    /// <param name="frequency">Ledger posting frequency. Monthly is used when omitted.</param>
    public static CapitalAssetPurchaseWrite ReducingBalance(int annualDepreciationPercentage, DepreciationFrequency? frequency = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(annualDepreciationPercentage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(annualDepreciationPercentage, 99);

        return new CapitalAssetPurchaseWrite
        {
            DepreciationProfile = new DepreciationProfileWritePayload
            {
                Method = DepreciationMethod.ReducingBalance,
                AnnualDepreciationPercentage = annualDepreciationPercentage,
                Frequency = frequency
            }
        };
    }

    /// <summary>
    /// Capital asset purchase with no depreciation ledgers.
    /// </summary>
    public static CapitalAssetPurchaseWrite NoDepreciation() =>
        new()
        {
            DepreciationProfile = new DepreciationProfileWritePayload
            {
                Method = DepreciationMethod.NoDepreciation
            }
        };
}
