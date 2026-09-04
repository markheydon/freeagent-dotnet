using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// GoCardless direct debit mandate details for a contact.
/// </summary>
public class DirectDebitMandate
{
    /// <summary>
    /// Mandate currency code (ISO 4217). May be <see langword="null"/> when omitted by the API.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Maximum collection amount.
    /// </summary>
    [JsonPropertyName("max_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Remaining collection amount in the current interval.
    /// </summary>
    [JsonPropertyName("remaining_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? RemainingAmount { get; set; }

    /// <summary>
    /// Date the next collection interval starts.
    /// </summary>
    [JsonPropertyName("next_interval_starts_on")]
    public DateOnly? NextIntervalStartsOn { get; set; }
}
