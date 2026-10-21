using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for bank account create and update request payloads.
/// </summary>
internal sealed class BankAccountRequest
{
    /// <summary>
    /// Bank account attributes to create or update.
    /// </summary>
    [JsonPropertyName("bank_account")]
    public BankAccountWritePayload? BankAccount { get; set; }
}
