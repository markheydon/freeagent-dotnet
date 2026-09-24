namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Request attributes for updating a note.
/// </summary>
public sealed class UpdateNoteRequest
{
    internal string Content { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request to update note content.
    /// </summary>
    /// <param name="content">Updated note text.</param>
    /// <returns>An update-note request.</returns>
    public static UpdateNoteRequest Create(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new UpdateNoteRequest
        {
            Content = content
        };
    }
}
