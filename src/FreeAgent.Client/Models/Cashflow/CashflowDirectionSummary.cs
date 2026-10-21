using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Cashflow;

/// <summary>
/// Incoming or outgoing cashflow totals with per-month breakdown.
/// </summary>
public sealed class CashflowDirectionSummary
{
    /// <summary>
    /// Total for the direction across the date range.
    /// </summary>
    [JsonPropertyName("total")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Total { get; set; }

    /// <summary>
    /// Per-month totals.
    /// </summary>
    [JsonPropertyName("months")]
    public IReadOnlyList<CashflowMonth>? Months { get; set; }
}
