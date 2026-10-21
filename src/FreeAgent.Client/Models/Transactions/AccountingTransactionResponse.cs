using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a single accounting transaction.
/// </summary>
internal sealed class AccountingTransactionResponse
{
    /// <summary>
    /// Transaction payload.
    /// </summary>
    [JsonPropertyName("transaction")]
    public AccountingTransaction? Transaction { get; set; }
}
