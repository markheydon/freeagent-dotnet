using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Wrapper for note list API responses.
/// </summary>
internal sealed class NotesResponse
{
    /// <summary>
    /// Note collection payload.
    /// </summary>
    [JsonPropertyName("notes")]
    public IReadOnlyList<Note>? Notes { get; set; }
}
