using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CapitalAssets;

/// <summary>
/// A lifecycle event on a capital asset, returned when <c>include_history=true</c>.
/// </summary>
public sealed class CapitalAssetHistoryEntry
{
    /// <summary>
    /// Event type (for example purchase, depreciation, or disposal).
    /// </summary>
    [JsonPropertyName("type")]
    public CapitalAssetHistoryEventType? Type { get; set; }

    /// <summary>
    /// Description of the event.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// When the event occurred.
    /// </summary>
    [JsonPropertyName("date")]
    public DateOnly? Date { get; set; }

    /// <summary>
    /// Value of the underlying transaction.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Value { get; set; }

    /// <summary>
    /// Tax value included in the underlying transaction.
    /// </summary>
    [JsonPropertyName("tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TaxValue { get; set; }

    /// <summary>
    /// Wire representation of a related bill, expense, or bank transaction explanation link.
    /// </summary>
    [JsonPropertyName("link")]
    [JsonInclude]
    internal string? Link { get; set; }

    /// <summary>
    /// Related expense identifier when <see cref="Link"/> refers to an expense.
    /// </summary>
    [JsonIgnore]
    public long? ExpenseId => TryParseExpenseId(out var id) ? id : null;

    /// <summary>
    /// Related bill identifier when <see cref="Link"/> refers to a bill.
    /// </summary>
    [JsonIgnore]
    public long? BillId => TryParseBillId(out var id) ? id : null;

    /// <summary>
    /// Related bank transaction explanation identifier when <see cref="Link"/> refers to an explanation.
    /// </summary>
    [JsonIgnore]
    public long? BankTransactionExplanationId => TryParseBankTransactionExplanationId(out var id) ? id : null;

    internal bool TryParseExpenseId(out long id)
    {
        id = 0;
        return !string.IsNullOrWhiteSpace(Link)
            && Link.Contains("/expenses/", StringComparison.OrdinalIgnoreCase)
            && FreeAgentResourceId.TryParse(Link, out id);
    }

    internal bool TryParseBillId(out long id)
    {
        id = 0;
        return !string.IsNullOrWhiteSpace(Link)
            && Link.Contains("/bills/", StringComparison.OrdinalIgnoreCase)
            && FreeAgentResourceId.TryParse(Link, out id);
    }

    internal bool TryParseBankTransactionExplanationId(out long id)
    {
        id = 0;
        return !string.IsNullOrWhiteSpace(Link)
            && Link.Contains("/bank_transaction_explanations/", StringComparison.OrdinalIgnoreCase)
            && FreeAgentResourceId.TryParse(Link, out id);
    }
}
