using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Wrapper for timeslip list and batch create API responses.
/// </summary>
public class TimeslipsResponse
{
    /// <summary>
    /// Timeslip collection payload.
    /// </summary>
    [JsonPropertyName("timeslips")]
    public IReadOnlyList<Timeslip>? Timeslips { get; set; }
}
