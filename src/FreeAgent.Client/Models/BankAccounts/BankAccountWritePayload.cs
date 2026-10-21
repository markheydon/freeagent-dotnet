using System.Text.Json.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Writable bank account attributes serialised in create and update request envelopes.
/// </summary>
internal sealed class BankAccountWritePayload
{
    [JsonPropertyName("type")]
    public BankAccountType? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("is_personal")]
    public bool? IsPersonal { get; set; }

    [JsonPropertyName("status")]
    public BankAccountStatus? Status { get; set; }

    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    [JsonPropertyName("opening_balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OpeningBalance { get; set; }

    [JsonPropertyName("bank_code")]
    public string? BankCode { get; set; }

    [JsonPropertyName("bank_guess_enabled")]
    public bool? BankGuessEnabled { get; set; }

    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("sort_code")]
    public string? SortCode { get; set; }

    [JsonPropertyName("secondary_sort_code")]
    public string? SecondarySortCode { get; set; }

    [JsonPropertyName("iban")]
    public string? Iban { get; set; }

    [JsonPropertyName("bic")]
    public string? Bic { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
