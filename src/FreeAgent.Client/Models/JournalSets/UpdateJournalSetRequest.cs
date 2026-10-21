namespace FreeAgent.Client;

/// <summary>
/// Attributes for updating a journal set.
/// </summary>
public sealed class UpdateJournalSetRequest
{
    /// <summary>
    /// Journal set description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Date on which journal entries are entered.
    /// </summary>
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Optional application tag.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Journal entry changes, including additions, updates, and deletions.
    /// </summary>
    public IReadOnlyList<JournalEntry>? JournalEntries { get; init; }
}
