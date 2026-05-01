using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Wrapper for company tax timeline API responses.
/// </summary>
public class TaxTimelineResponse
{
    /// <summary>
    /// Timeline items returned by the API.
    /// </summary>
    [JsonPropertyName("timeline_items")]
    public List<TaxTimelineItem>? TimelineItems { get; set; }
}
