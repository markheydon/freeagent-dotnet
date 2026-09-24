namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Request attributes for creating a note on a contact.
/// </summary>
public sealed class CreateContactNoteRequest
{
    internal string Content { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request for a new contact note.
    /// </summary>
    /// <param name="content">Note text.</param>
    /// <returns>A create-contact-note request.</returns>
    public static CreateContactNoteRequest Create(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new CreateContactNoteRequest
        {
            Content = content
        };
    }
}
