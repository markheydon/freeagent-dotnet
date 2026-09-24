using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Wrapper for single note API responses.
/// </summary>
internal sealed class NoteResponse
{
    /// <summary>
    /// Note payload.
    /// </summary>
    [JsonPropertyName("note")]
    public Note? Note { get; set; }
}
