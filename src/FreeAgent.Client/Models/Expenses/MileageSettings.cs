using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Mileage configuration returned by the expenses API.
/// </summary>
public sealed class MileageSettings
{
    /// <summary>
    /// Engine type and engine size options grouped by effective date ranges.
    /// </summary>
    [JsonPropertyName("engine_type_and_size_options")]
    public List<MileageEngineTypeAndSizeOption>? EngineTypeAndSizeOptions { get; set; }
}

/// <summary>
/// Engine type and size options valid between two dates.
/// </summary>
public sealed class MileageEngineTypeAndSizeOption
{
    /// <summary>
    /// Start date for this option set.
    /// </summary>
    [JsonPropertyName("from")]
    public DateOnly? From { get; set; }

    /// <summary>
    /// End date for this option set.
    /// </summary>
    [JsonPropertyName("to")]
    public DateOnly? To { get; set; }

    /// <summary>
    /// Engine types mapped to allowed engine sizes.
    /// </summary>
    [JsonPropertyName("value")]
    public Dictionary<string, List<string>>? Value { get; set; }
}

/// <summary>
/// Response envelope for mileage settings.
/// </summary>
internal sealed class MileageSettingsResponse
{
    [JsonPropertyName("mileage_settings")]
    public MileageSettings? MileageSettings { get; set; }
}
