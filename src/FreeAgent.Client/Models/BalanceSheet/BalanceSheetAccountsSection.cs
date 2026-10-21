using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// A balance sheet section that lists nominal accounts (for example current assets or current liabilities).
/// </summary>
public sealed class BalanceSheetAccountsSection
{
    /// <summary>
    /// Accounts in this section.
    /// </summary>
    [JsonPropertyName("accounts")]
    public IReadOnlyList<BalanceSheetAccount>? Accounts { get; set; }
}
