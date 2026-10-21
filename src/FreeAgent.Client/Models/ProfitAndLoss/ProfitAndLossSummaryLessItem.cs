using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.ProfitAndLoss;

/// <summary>
/// A line subtracted from operating profit when calculating retained profit.
/// </summary>
public sealed class ProfitAndLossSummaryLessItem
{
    /// <summary>
    /// Description of the amount being subtracted (for example corporation tax or dividends).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Amount subtracted, in the company's native currency.
    /// </summary>
    [JsonPropertyName("total")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Total { get; set; }
}
