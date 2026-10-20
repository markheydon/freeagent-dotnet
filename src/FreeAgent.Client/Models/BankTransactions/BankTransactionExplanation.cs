using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;

namespace FreeAgent.Client.Models.BankTransactions;

/// <summary>
/// Nested bank transaction explanation returned on a bank transaction.
/// </summary>
/// <remarks>
/// This is the nested shape documented on the bank transactions page. Full explanation
/// create, update, and delete operations are a separate resource.
/// </remarks>
public sealed class BankTransactionExplanation : IFreeAgentResource
{
    /// <summary>
    /// Explanation resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Wire representation of the parent bank transaction.
    /// </summary>
    [JsonPropertyName("bank_transaction")]
    [JsonInclude]
    internal ExpandableField<BankTransaction>? BankTransactionLink { get; set; }

    /// <summary>
    /// Parent bank transaction when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankTransaction? BankTransaction => BankTransactionLink?.Value;

    /// <summary>
    /// Parent bank transaction identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? BankTransactionId => BankTransactionLink?.Id;

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
    /// Date of the explanation.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Explanation entry type, as returned by FreeAgent (typically a category name).
    /// </summary>
    [JsonPropertyName("entry_type")]
    public string? EntryType { get; set; }

    /// <summary>
    /// Gross value of the explanation.
    /// </summary>
    [JsonPropertyName("gross_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? GrossValue { get; set; }

    /// <summary>
    /// Whether the explanation can be deleted.
    /// </summary>
    [JsonPropertyName("is_deletable")]
    public bool? IsDeletable { get; set; }
}
