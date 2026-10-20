using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BankTransactionExplanations;

/// <summary>
/// Response envelope for a bank transaction explanations list.
/// </summary>
public sealed class BankTransactionExplanationsListResponse
{
    /// <summary>
    /// Bank transaction explanations returned by FreeAgent.
    /// </summary>
    [JsonPropertyName("bank_transaction_explanations")]
    public List<BankTransactionExplanation>? BankTransactionExplanations { get; set; }
}
