using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client;

/// <summary>
/// Writable journal set attributes for create and update requests.
/// </summary>
internal sealed class JournalSetWritePayload
{
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    [JsonPropertyName("journal_entries")]
    public List<JournalEntryWritePayload>? JournalEntries { get; set; }

    public static JournalSetWritePayload FromCreate(CreateJournalSetRequest request, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new JournalSetWritePayload
        {
            DatedOn = request.DatedOn,
            Description = request.Description,
            Tag = request.Tag,
            JournalEntries = request.JournalEntries
                .Select(entry => JournalEntryWritePayload.FromJournalEntry(entry, environment, journalSetId: null, isUpdate: false))
                .ToList()
        };
    }

    public static JournalSetWritePayload FromUpdate(
        UpdateJournalSetRequest request,
        FreeAgentEnvironment environment,
        long journalSetId)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new JournalSetWritePayload
        {
            DatedOn = request.DatedOn,
            Description = request.Description,
            Tag = request.Tag,
            JournalEntries = request.JournalEntries?
                .Select(entry => JournalEntryWritePayload.FromJournalEntry(entry, environment, journalSetId, isUpdate: true))
                .ToList()
        };
    }
}
