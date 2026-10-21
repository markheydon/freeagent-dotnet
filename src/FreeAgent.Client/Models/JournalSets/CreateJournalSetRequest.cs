namespace FreeAgent.Client.Models.JournalSets;

/// <summary>
/// Attributes for creating a journal set.
/// </summary>
public sealed class CreateJournalSetRequest
{
    /// <summary>
    /// Journal set description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Date on which journal entries are entered.
    /// </summary>
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Optional application tag.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Journal entries that must balance.
    /// </summary>
    public required IReadOnlyList<JournalEntry> JournalEntries { get; init; }
}
