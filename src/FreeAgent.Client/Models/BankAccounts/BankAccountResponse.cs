using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for single bank account API responses.
/// </summary>
public sealed class BankAccountResponse
{
    /// <summary>
    /// Bank account payload.
    /// </summary>
    [JsonPropertyName("bank_account")]
    public BankAccount? BankAccount { get; set; }
}
