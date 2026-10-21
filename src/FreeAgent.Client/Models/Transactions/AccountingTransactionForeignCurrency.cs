using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Transactions;

/// <summary>
/// Foreign-currency amounts on an accounting transaction when the currency differs from the company currency.
/// </summary>
public sealed class AccountingTransactionForeignCurrency
{
    /// <summary>
    /// ISO currency code for the foreign amount.
    /// </summary>
    [JsonPropertyName("currency_code")]
    public CurrencyCode? CurrencyCode { get; set; }

    /// <summary>
    /// Debit value in the foreign currency. Credit values are negative.
    /// </summary>
    [JsonPropertyName("debit_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DebitValue { get; set; }
}
