using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Depreciation profile nested under a capital asset on API responses.
/// </summary>
public sealed class DepreciationProfile
{
    /// <summary>
    /// Depreciation calculation method.
    /// </summary>
    [JsonPropertyName("method")]
    public DepreciationMethod? Method { get; set; }

    /// <summary>
    /// Asset life in years for straight line depreciation (2–25).
    /// </summary>
    [JsonPropertyName("asset_life_years")]
    public int? AssetLifeYears { get; set; }

    /// <summary>
    /// Annual depreciation percentage for reducing balance depreciation (1–99).
    /// </summary>
    [JsonPropertyName("annual_depreciation_percentage")]
    public int? AnnualDepreciationPercentage { get; set; }

    /// <summary>
    /// Ledger posting frequency. Defaults to monthly when omitted on write.
    /// </summary>
    [JsonPropertyName("frequency")]
    public DepreciationFrequency? Frequency { get; set; }
}
