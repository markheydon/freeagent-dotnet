namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Request attributes for creating a note on a project.
/// </summary>
public sealed class CreateProjectNoteRequest
{
    internal string Content { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request for a new project note.
    /// </summary>
    /// <param name="content">Note text.</param>
    /// <returns>A create-project-note request.</returns>
    public static CreateProjectNoteRequest Create(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new CreateProjectNoteRequest
        {
            Content = content
        };
    }
}
