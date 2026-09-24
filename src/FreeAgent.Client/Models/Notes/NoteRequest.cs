using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Wrapper for note create and update request payloads.
/// </summary>
internal sealed class NoteRequest
{
    /// <summary>
    /// Note attributes to create or update.
    /// </summary>
    [JsonPropertyName("note")]
    public NoteWritePayload? Note { get; set; }
}
