using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for batch timeslip create request payloads.
/// </summary>
internal sealed class TimeslipsBatchRequest
{
    /// <summary>
    /// Timeslip attributes to create.
    /// </summary>
    [JsonPropertyName("timeslips")]
    public IReadOnlyList<TimeslipWritePayload>? Timeslips { get; set; }
}
