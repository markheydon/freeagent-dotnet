using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BalanceSheet;

/// <summary>
/// Owner's equity section of the balance sheet.
/// </summary>
public sealed class BalanceSheetOwnersEquity
{
    /// <summary>
    /// Equity accounts.
    /// </summary>
    [JsonPropertyName("accounts")]
    public IReadOnlyList<BalanceSheetAccount>? Accounts { get; set; }

    /// <summary>
    /// Total retained profit, rounded to the nearest integer.
    /// </summary>
    [JsonPropertyName("retained_profit")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? RetainedProfit { get; set; }
}
