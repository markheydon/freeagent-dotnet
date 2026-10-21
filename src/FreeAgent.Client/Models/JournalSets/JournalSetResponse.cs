using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.JournalSets;

/// <summary>
/// Response wrapper for single journal set requests.
/// </summary>
internal sealed class JournalSetResponse
{
    /// <summary>
    /// Journal set returned by the API.
    /// </summary>
    [JsonPropertyName("journal_set")]
    public JournalSet? JournalSet { get; set; }
}
