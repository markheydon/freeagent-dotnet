namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.CreditNotes.CreditNotesService.GetCreditNoteAsync(long, CreditNoteGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class CreditNoteGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the contact when the credit note response contains only a contact URI.
    /// </summary>
    public bool IncludeContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the credit note response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
