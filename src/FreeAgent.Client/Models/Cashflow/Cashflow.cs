using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Historic cashflow summary for a date range.
/// </summary>
public sealed class Cashflow
{
    /// <summary>
    /// Start date of the cashflow range.
    /// </summary>
    [JsonPropertyName("from")]
    public DateOnly? From { get; set; }

    /// <summary>
    /// End date of the cashflow range.
    /// </summary>
    [JsonPropertyName("to")]
    public DateOnly? To { get; set; }

    /// <summary>
    /// Incoming cashflow.
    /// </summary>
    [JsonPropertyName("incoming")]
    public CashflowDirectionSummary? Incoming { get; set; }

    /// <summary>
    /// Outgoing cashflow.
    /// </summary>
    [JsonPropertyName("outgoing")]
    public CashflowDirectionSummary? Outgoing { get; set; }

    /// <summary>
    /// Net balance for the range.
    /// </summary>
    [JsonPropertyName("balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Balance { get; set; }
}
