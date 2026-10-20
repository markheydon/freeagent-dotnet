using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BankTransactions;

/// <summary>
/// One transaction line in a JSON bank statement upload.
/// </summary>
public sealed class BankStatementTransaction
{
    /// <summary>
    /// Date of the transaction.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public required DateOnly DatedOn { get; init; }

    /// <summary>
    /// Free-text description. The API defaults this to an empty string when omitted.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Amount in the company's native currency. The API defaults this to 0 when omitted.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Unique transaction identifier (<c>fitid</c>). The API defaults this to null when omitted.
    /// </summary>
    [JsonPropertyName("fitid")]
    public string? FitId { get; init; }

    /// <summary>
    /// Statement transaction type. The API defaults this to <c>OTHER</c> when omitted.
    /// </summary>
    [JsonPropertyName("transaction_type")]
    public BankStatementTransactionType? TransactionType { get; init; }
}
