using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent bank account.
/// </summary>
public sealed class BankAccount : IFreeAgentResource
{
    /// <summary>
    /// Bank account resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Bank account type.
    /// </summary>
    [JsonPropertyName("type")]
    public BankAccountType? Type { get; set; }

    /// <summary>
    /// Account name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Accounting currency for the account.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Whether the account is personal (<see langword="true"/>) or business (<see langword="false"/>).
    /// </summary>
    [JsonPropertyName("is_personal")]
    public bool? IsPersonal { get; set; }

    /// <summary>
    /// Whether this is the company's primary bank account.
    /// </summary>
    [JsonPropertyName("is_primary")]
    public bool? IsPrimary { get; set; }

    /// <summary>
    /// Account visibility status.
    /// </summary>
    [JsonPropertyName("status")]
    public BankAccountStatus? Status { get; set; }

    /// <summary>
    /// Bank name.
    /// </summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>
    /// Account balance at the FreeAgent start date.
    /// </summary>
    [JsonPropertyName("opening_balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OpeningBalance { get; set; }

    /// <summary>
    /// Bank identifier constant derived from the sort code (for example <c>barclays</c>).
    /// </summary>
    [JsonPropertyName("bank_code")]
    public string? BankCode { get; set; }

    /// <summary>
    /// Latest account balance.
    /// </summary>
    [JsonPropertyName("current_balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CurrentBalance { get; set; }

    /// <summary>
    /// Date of the latest transaction or bank account entry.
    /// </summary>
    [JsonPropertyName("latest_activity_date")]
    public DateOnly? LatestActivityDate { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Whether bank guess is enabled for the account.
    /// </summary>
    [JsonPropertyName("bank_guess_enabled")]
    public bool? BankGuessEnabled { get; set; }

    /// <summary>
    /// Bank account number (standard and credit card accounts).
    /// </summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>
    /// Sort code or routing number (standard accounts).
    /// </summary>
    [JsonPropertyName("sort_code")]
    public string? SortCode { get; set; }

    /// <summary>
    /// Secondary sort code when the account has two bank/sort codes.
    /// </summary>
    [JsonPropertyName("secondary_sort_code")]
    public string? SecondarySortCode { get; set; }

    /// <summary>
    /// International Bank Account Number.
    /// </summary>
    [JsonPropertyName("iban")]
    public string? Iban { get; set; }

    /// <summary>
    /// Bank Identifier Code (Swift code).
    /// </summary>
    [JsonPropertyName("bic")]
    public string? Bic { get; set; }

    /// <summary>
    /// PayPal login email (PayPal accounts).
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
