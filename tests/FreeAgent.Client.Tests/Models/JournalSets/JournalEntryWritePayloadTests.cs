using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client.Tests.Models.JournalSets;

public class JournalEntryWritePayloadTests
{
    [Fact]
    public void FromJournalEntry_UpdateWithEntryId_BuildsJournalEntryUrl()
    {
        var entry = new JournalEntry
        {
            EntryId = 9,
            DebitValue = 25m,
            CategoryNominalCode = "001"
        };

        var payload = JournalEntryWritePayload.FromJournalEntry(
            entry,
            FreeAgentEnvironment.Production,
            journalSetId: 4,
            isUpdate: true);

        Assert.Equal(
            "https://api.freeagent.com/v2/journal_sets/4/journal_entries/9",
            payload.Url);
    }
}
