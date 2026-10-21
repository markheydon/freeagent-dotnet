using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a bank transaction list.
/// </summary>
public sealed class BankTransactionListResponse
{
    /// <summary>
    /// Bank transactions in the current page.
    /// </summary>
    [JsonPropertyName("bank_transactions")]
    public List<BankTransaction>? BankTransactions { get; set; }
}
