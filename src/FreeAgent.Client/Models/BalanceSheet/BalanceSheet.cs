using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Balance sheet figures for an accounting period or opening balances.
/// </summary>
public sealed class BalanceSheet
{
    /// <summary>
    /// Start date of the accounting period for which values are shown. Not present on opening balances.
    /// </summary>
    [JsonPropertyName("accounting_period_start_date")]
    public DateOnly? AccountingPeriodStartDate { get; set; }

    /// <summary>
    /// Values are as at this date from <see cref="AccountingPeriodStartDate"/>. Not present on opening balances.
    /// </summary>
    [JsonPropertyName("as_at_date")]
    public DateOnly? AsAtDate { get; set; }

    /// <summary>
    /// ISO code for the company's native currency.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Capital assets of the business.
    /// </summary>
    [JsonPropertyName("capital_assets")]
    public BalanceSheetCapitalAssets? CapitalAssets { get; set; }

    /// <summary>
    /// Current assets of the business.
    /// </summary>
    [JsonPropertyName("current_assets")]
    public BalanceSheetAccountsSection? CurrentAssets { get; set; }

    /// <summary>
    /// Current liabilities of the business.
    /// </summary>
    [JsonPropertyName("current_liabilities")]
    public BalanceSheetAccountsSection? CurrentLiabilities { get; set; }

    /// <summary>
    /// Total value of current asset and current liability debit values, rounded to the nearest integer.
    /// </summary>
    [JsonPropertyName("net_current_assets")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? NetCurrentAssets { get; set; }

    /// <summary>
    /// Total assets, rounded to the nearest integer.
    /// </summary>
    [JsonPropertyName("total_assets")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? TotalAssets { get; set; }

    /// <summary>
    /// Owner's equity.
    /// </summary>
    [JsonPropertyName("owners_equity")]
    public BalanceSheetOwnersEquity? OwnersEquity { get; set; }

    /// <summary>
    /// Total owner's equity, rounded to the nearest integer.
    /// </summary>
    [JsonPropertyName("total_owners_equity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? TotalOwnersEquity { get; set; }
}
