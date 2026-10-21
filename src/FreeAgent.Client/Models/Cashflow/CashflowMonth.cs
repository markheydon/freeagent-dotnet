using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Cashflow;

/// <summary>
/// Cashflow total for a calendar month within a cashflow summary.
/// </summary>
public sealed class CashflowMonth
{
    /// <summary>
    /// Calendar month (1–12).
    /// </summary>
    [JsonPropertyName("month")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Month { get; set; }

    /// <summary>
    /// Calendar year.
    /// </summary>
    [JsonPropertyName("year")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Year { get; set; }

    /// <summary>
    /// Cashflow total for the month.
    /// </summary>
    [JsonPropertyName("total")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Total { get; set; }
}
