using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.JournalSets;

/// <summary>
/// Response wrapper for journal set list requests.
/// </summary>
internal sealed class JournalSetsListResponse
{
    /// <summary>
    /// Journal sets returned by the API.
    /// </summary>
    [JsonPropertyName("journal_sets")]
    public List<JournalSet>? JournalSets { get; set; }
}
