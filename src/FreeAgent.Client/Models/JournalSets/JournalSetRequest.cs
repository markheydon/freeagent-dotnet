using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for journal set write API requests.
/// </summary>
/// <typeparam name="T">Create or update payload type.</typeparam>
internal sealed class JournalSetRequest<T>
{
    /// <summary>
    /// Journal set payload.
    /// </summary>
    [JsonPropertyName("journal_set")]
    public T JournalSet { get; set; } = default!;
}
