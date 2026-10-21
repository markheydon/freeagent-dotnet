using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for bank accounts list API responses.
/// </summary>
public sealed class BankAccountListResponse
{
    /// <summary>
    /// Bank accounts list payload.
    /// </summary>
    [JsonPropertyName("bank_accounts")]
    public List<BankAccount>? BankAccounts { get; set; }
}
