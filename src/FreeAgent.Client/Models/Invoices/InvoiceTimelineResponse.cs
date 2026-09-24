using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// A single entry on the company invoice timeline.
/// </summary>
public sealed class InvoiceTimelineItem
{
    /// <summary>
    /// Invoice reference associated with the timeline entry.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Timeline entry summary text.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>
    /// Timeline entry description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Date of the timeline entry.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Amount associated with the timeline entry.
    /// </summary>
    [JsonPropertyName("amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Amount { get; set; }
}

/// <summary>
/// Response envelope for the invoice timeline list.
/// </summary>
public sealed class InvoiceTimelineResponse
{
    /// <summary>
    /// Invoice timeline items.
    /// </summary>
    [JsonPropertyName("invoice_timeline_items")]
    public List<InvoiceTimelineItem>? InvoiceTimelineItems { get; set; }
}
