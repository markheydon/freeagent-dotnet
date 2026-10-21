using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for listing accounting transactions.
/// </summary>
internal sealed class AccountingTransactionsListResponse
{
    /// <summary>
    /// Transactions for the requested period.
    /// </summary>
    [JsonPropertyName("transactions")]
    public List<AccountingTransaction>? Transactions { get; set; }
}
