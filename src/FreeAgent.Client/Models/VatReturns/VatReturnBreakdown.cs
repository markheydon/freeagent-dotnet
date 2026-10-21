using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.VatReturns;

/// <summary>
/// VAT return breakdown with titled rows.
/// </summary>
public sealed class VatReturnBreakdown
{
    /// <summary>
    /// Breakdown title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Breakdown rows.
    /// </summary>
    [JsonPropertyName("rows")]
    public IReadOnlyList<VatReturnBreakdownRow>? Rows { get; set; }
}
