using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Bank account opening balance row on the opening balances journal set.
/// </summary>
public sealed class JournalSetOpeningBalanceBankAccount
{
    /// <summary>
    /// Bank account resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Bank account identifier parsed from <see cref="Url"/>.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId => FreeAgentResourceId.TryParse(Url, out var id) ? id : null;

    /// <summary>
    /// Bank account description.
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
