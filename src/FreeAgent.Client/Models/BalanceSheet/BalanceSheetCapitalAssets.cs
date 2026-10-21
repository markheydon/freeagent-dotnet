using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Capital assets section of the balance sheet.
/// </summary>
public sealed class BalanceSheetCapitalAssets
{
    /// <summary>
    /// Capital asset accounts.
    /// </summary>
    [JsonPropertyName("accounts")]
    public IReadOnlyList<BalanceSheetAccount>? Accounts { get; set; }

    /// <summary>
    /// Net book value of all capital asset accounts, rounded to the nearest integer.
    /// </summary>
    [JsonPropertyName("net_book_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? NetBookValue { get; set; }
}
