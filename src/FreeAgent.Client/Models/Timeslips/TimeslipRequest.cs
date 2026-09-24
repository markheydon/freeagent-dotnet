using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Wrapper for single timeslip create and update request payloads.
/// </summary>
internal sealed class TimeslipRequest
{
    /// <summary>
    /// Timeslip attributes to create or update.
    /// </summary>
    [JsonPropertyName("timeslip")]
    public TimeslipWritePayload? Timeslip { get; set; }
}
