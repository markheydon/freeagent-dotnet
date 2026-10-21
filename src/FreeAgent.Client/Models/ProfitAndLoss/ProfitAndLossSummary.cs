using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Profit and loss summary for a date range or accounting period.
/// </summary>
public sealed class ProfitAndLossSummary
{
    /// <summary>
    /// Start date of the report.
    /// </summary>
    [JsonPropertyName("from")]
    public DateOnly? From { get; set; }

    /// <summary>
    /// End date of the report.
    /// </summary>
    [JsonPropertyName("to")]
    public DateOnly? To { get; set; }

    /// <summary>
    /// Income for the period, in the company's native currency.
    /// </summary>
    [JsonPropertyName("income")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Income { get; set; }

    /// <summary>
    /// Expenses for the period, in the company's native currency.
    /// </summary>
    [JsonPropertyName("expenses")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Expenses { get; set; }

    /// <summary>
    /// Operating profit for the period, in the company's native currency.
    /// </summary>
    [JsonPropertyName("operating_profit")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OperatingProfit { get; set; }

    /// <summary>
    /// Amounts subtracted from operating profit when calculating retained profit.
    /// </summary>
    [JsonPropertyName("less")]
    public IReadOnlyList<ProfitAndLossSummaryLessItem>? Less { get; set; }

    /// <summary>
    /// Retained profit for the period.
    /// </summary>
    [JsonPropertyName("retained_profit")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RetainedProfit { get; set; }

    /// <summary>
    /// Retained profit brought forward from the previous year.
    /// </summary>
    [JsonPropertyName("retained_profit_brought_forward")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RetainedProfitBroughtForward { get; set; }

    /// <summary>
    /// Distributable profit at the end of the period.
    /// </summary>
    [JsonPropertyName("retained_profit_carried_forward")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RetainedProfitCarriedForward { get; set; }
}
