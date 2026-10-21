using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Writable depreciation profile attributes nested under <c>capital_asset</c>.
/// </summary>
internal sealed class DepreciationProfileWritePayload
{
    [JsonPropertyName("method")]
    public DepreciationMethod Method { get; set; }

    [JsonPropertyName("asset_life_years")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? AssetLifeYears { get; set; }

    [JsonPropertyName("annual_depreciation_percentage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? AnnualDepreciationPercentage { get; set; }

    [JsonPropertyName("frequency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DepreciationFrequency? Frequency { get; set; }
}
