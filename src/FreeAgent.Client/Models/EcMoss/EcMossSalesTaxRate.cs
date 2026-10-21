using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.EcMoss;

/// <summary>
/// EC VAT MOSS sales tax rate band returned by the MOSS rates lookup.
/// </summary>
public sealed class EcMossSalesTaxRate
{
    /// <summary>
    /// Tax percentage for the band.
    /// </summary>
    [JsonPropertyName("percentage")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Percentage { get; set; }

    /// <summary>
    /// Rate band name (for example Standard or Reduced).
    /// </summary>
    [JsonPropertyName("band")]
    public string? Band { get; set; }
}
