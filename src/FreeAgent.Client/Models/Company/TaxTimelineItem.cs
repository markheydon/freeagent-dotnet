using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents a single upcoming tax timeline event.
/// </summary>
public class TaxTimelineItem
{
    /// <summary>
    /// Timeline event description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Nature of the timeline event.
    /// </summary>
    [JsonPropertyName("nature")]
    public string Nature { get; set; } = string.Empty;

    /// <summary>
    /// Date associated with the timeline event.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly DatedOn { get; set; }

    /// <summary>
    /// Amount due when provided by the API.
    /// </summary>
    [JsonPropertyName("amount_due")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AmountDue { get; set; }

    /// <summary>
    /// Indicates if the event is personal.
    /// </summary>
    [JsonPropertyName("is_personal")]
    public bool IsPersonal { get; set; }
}
