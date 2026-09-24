using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Wrapper for single timeslip API responses.
/// </summary>
public class TimeslipResponse
{
    /// <summary>
    /// Timeslip payload.
    /// </summary>
    [JsonPropertyName("timeslip")]
    public Timeslip? Timeslip { get; set; }
}
