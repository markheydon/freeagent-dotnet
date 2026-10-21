using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// A nominal account line on the balance sheet.
/// </summary>
public sealed class BalanceSheetAccount
{
    /// <summary>
    /// Display name of the account.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Nominal code of the account.
    /// </summary>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; set; }

    /// <summary>
    /// Total debit value for the account, rounded to the nearest integer.
    /// </summary>
    [JsonPropertyName("total_debit_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? TotalDebitValue { get; set; }
}
