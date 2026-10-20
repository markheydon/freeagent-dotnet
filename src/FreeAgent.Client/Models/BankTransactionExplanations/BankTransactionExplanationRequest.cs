using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BankTransactionExplanations;

/// <summary>
/// Request envelope for a single bank transaction explanation create or update.
/// </summary>
internal sealed class BankTransactionExplanationRequest
{
    [JsonPropertyName("bank_transaction_explanation")]
    public BankTransactionExplanationWritePayload? BankTransactionExplanation { get; set; }
}
