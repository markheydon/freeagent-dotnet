using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BankTransactions;

/// <summary>
/// Response envelope for a single bank transaction.
/// </summary>
public sealed class BankTransactionResponse
{
    /// <summary>
    /// Bank transaction payload.
    /// </summary>
    [JsonPropertyName("bank_transaction")]
    public BankTransaction? BankTransaction { get; set; }
}
