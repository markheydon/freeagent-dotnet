namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Notes.NoteService.GetNoteAsync(long, NoteGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class NoteGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the parent contact when the note response contains only a contact URI in <c>parent_url</c>.
    /// </summary>
    public bool IncludeParentContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the parent project when the note response contains only a project URI in <c>parent_url</c>.
    /// </summary>
    public bool IncludeParentProject { get; init; }
}
