using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Writable note attributes for create and update requests.
/// </summary>
internal sealed class NoteWritePayload
{
    [JsonPropertyName("note")]
    public string? Content { get; set; }

    public static NoteWritePayload FromCreate(CreateContactNoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new NoteWritePayload { Content = request.Content };
    }

    public static NoteWritePayload FromCreate(CreateProjectNoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new NoteWritePayload { Content = request.Content };
    }

    public static NoteWritePayload FromUpdate(UpdateNoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new NoteWritePayload { Content = request.Content };
    }
}
