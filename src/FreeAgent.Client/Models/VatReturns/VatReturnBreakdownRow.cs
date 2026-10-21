using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.VatReturns;

/// <summary>
/// A row in a VAT return breakdown.
/// </summary>
public sealed class VatReturnBreakdownRow
{
    /// <summary>
    /// Row title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Row value.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Value { get; set; }

    /// <summary>
    /// Row identifier key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// HMRC VAT box number when applicable.
    /// </summary>
    [JsonPropertyName("box_number")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? BoxNumber { get; set; }
}
