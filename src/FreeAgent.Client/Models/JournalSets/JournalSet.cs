using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.JournalSets;

/// <summary>
/// Represents a FreeAgent journal set.
/// </summary>
public sealed class JournalSet : IFreeAgentResource
{
    /// <summary>
    /// Journal set resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Date on which journal entries are entered.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Journal set description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Application tag used to identify API-created journal sets.
    /// </summary>
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Journal entries in the set.
    /// </summary>
    [JsonPropertyName("journal_entries")]
    public List<JournalEntry>? JournalEntries { get; set; }

    /// <summary>
    /// Bank account opening balances (opening balances journal set only).
    /// </summary>
    [JsonPropertyName("bank_accounts")]
    public List<JournalSetOpeningBalanceBankAccount>? BankAccounts { get; set; }

    /// <summary>
    /// Stock item opening balances (opening balances journal set only).
    /// </summary>
    [JsonPropertyName("stock_items")]
    public List<JournalSetOpeningBalanceStockItem>? StockItems { get; set; }
}
