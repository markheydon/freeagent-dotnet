using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Running timer state on a timeslip.
/// </summary>
public sealed class TimeslipTimer
{
    /// <summary>
    /// Whether the timer is running. Always <see langword="true"/> when returned on the wire.
    /// </summary>
    [JsonPropertyName("running")]
    public bool? Running { get; set; }

    /// <summary>
    /// Effective start date of the timer in UTC, including any time already recorded.
    /// </summary>
    [JsonPropertyName("start_from")]
    public DateTimeOffset? StartFrom { get; set; }
}
