using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a single bank transaction explanation.
/// </summary>
public sealed class BankTransactionExplanationResponse
{
    /// <summary>
    /// Bank transaction explanation payload.
    /// </summary>
    [JsonPropertyName("bank_transaction_explanation")]
    public BankTransactionExplanation? BankTransactionExplanation { get; set; }

    /// <summary>
    /// Alternate envelope key used on some list responses.
    /// </summary>
    [JsonPropertyName("bank_transaction_explanations")]
    [JsonInclude]
    internal BankTransactionExplanation? BankTransactionExplanationAlternateEnvelope { get; set; }

    /// <summary>
    /// Returns the explanation payload from whichever envelope key FreeAgent returned.
    /// </summary>
    internal BankTransactionExplanation? ResolvePayload() =>
        BankTransactionExplanation ?? BankTransactionExplanationAlternateEnvelope;
}
