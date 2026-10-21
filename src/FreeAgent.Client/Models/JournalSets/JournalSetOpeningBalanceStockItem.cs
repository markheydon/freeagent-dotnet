using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.JournalSets;

/// <summary>
/// Stock item opening balance row on the opening balances journal set.
/// </summary>
public sealed class JournalSetOpeningBalanceStockItem
{
    /// <summary>
    /// Stock item resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Stock item identifier parsed from <see cref="Url"/>.
    /// </summary>
    [JsonIgnore]
    public long? StockItemId => FreeAgentResourceId.TryParse(Url, out var id) ? id : null;

    /// <summary>
    /// Stock item description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Opening debit value.
    /// </summary>
    [JsonPropertyName("debit_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DebitValue { get; set; }
}
