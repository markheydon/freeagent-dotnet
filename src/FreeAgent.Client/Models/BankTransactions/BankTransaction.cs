using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;
using FreeAgent.Client.Models.BankTransactionExplanations;

namespace FreeAgent.Client.Models.BankTransactions;

/// <summary>
/// Represents a FreeAgent bank transaction.
/// </summary>
public sealed class BankTransaction : IFreeAgentResource
{
    /// <summary>
    /// Bank transaction resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Total amount in the company's native currency.
    /// </summary>
    [JsonPropertyName("amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Wire representation of the bank account.
    /// </summary>
    [JsonPropertyName("bank_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? BankAccountLink { get; set; }

    /// <summary>
    /// Bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? BankAccount => BankAccountLink?.Value;

    /// <summary>
    /// Bank account identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId => BankAccountLink?.Id;

    /// <summary>
    /// Date of the transaction.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Complete description, including the amount and type.
    /// </summary>
    [JsonPropertyName("full_description")]
    public string? FullDescription { get; set; }

    /// <summary>
    /// When the transaction was uploaded to FreeAgent.
    /// </summary>
    [JsonPropertyName("uploaded_at")]
    public DateTimeOffset? UploadedAt { get; set; }

    /// <summary>
    /// Amount yet to be explained.
    /// </summary>
    [JsonPropertyName("unexplained_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? UnexplainedAmount { get; set; }

    /// <summary>
    /// <see langword="true"/> when the transaction was added manually.
    /// </summary>
    [JsonPropertyName("is_manual")]
    public bool? IsManual { get; set; }

    /// <summary>
    /// Unique transaction identifier within the banking institution (<c>fit_id</c>).
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

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
    /// Number of other transactions in the bank account with a matching description.
    /// </summary>
    [JsonPropertyName("matching_transactions_count")]
    public int? MatchingTransactionsCount { get; set; }

    /// <summary>
    /// Explanations that fully or partially explain this transaction.
    /// </summary>
    [JsonPropertyName("bank_transaction_explanations")]
    public List<BankTransactionExplanation>? BankTransactionExplanations { get; set; }
}
