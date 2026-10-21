using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.JournalSets;

namespace FreeAgent.Client.Tests.Models.JournalSets;

public class JournalSetModelSerializationTests
{
    [Fact]
    public void DeserializeJournalSet_MapsDatesTimestampsAndLinkedIds()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/journal_sets/1",
              "dated_on": "2011-07-28",
              "description": "An example journal set",
              "updated_at": "2020-02-06T11:08:28.000Z",
              "journal_entries": [
                {
                  "url": "https://api.freeagent.com/v2/journal_entries/1",
                  "category": "https://api.freeagent.com/v2/categories/001",
                  "description": "A Sales Correction",
                  "debit_value": "-123.45"
                },
                {
                  "url": "https://api.freeagent.com/v2/journal_sets/1/journal_entries/2",
                  "category": "https://api.freeagent.com/v2/categories/901",
                  "user": "https://api.freeagent.com/v2/users/1",
                  "debit_value": "123.45"
                }
              ]
            }
            """;

        var journalSet = JsonSerializer.Deserialize<JournalSet>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(journalSet);
        Assert.Equal(new DateOnly(2011, 7, 28), journalSet!.DatedOn);
        Assert.Equal(new DateTimeOffset(2020, 2, 6, 11, 8, 28, TimeSpan.Zero), journalSet.UpdatedAt);
        Assert.NotNull(journalSet.JournalEntries);
        Assert.Equal(2, journalSet.JournalEntries!.Count);
        Assert.Equal("001", journalSet.JournalEntries[0].CategoryNominalCode);
        Assert.Equal(-123.45m, journalSet.JournalEntries[0].DebitValue);
        Assert.Equal(2, journalSet.JournalEntries[1].EntryId);
        Assert.Equal(1L, journalSet.JournalEntries[1].UserId);
    }

    [Fact]
    public void SerializeUpdatePayload_IncludesDestroyFlag()
    {
        var payload = JournalSetWritePayload.FromUpdate(
            new UpdateJournalSetRequest
            {
                JournalEntries =
                [
                    new JournalEntry
                    {
                        Url = "https://api.freeagent.com/v2/journal_sets/37/journal_entries/17",
                        Destroy = true
                    }
                ]
            },
            FreeAgentEnvironment.Production,
            37);

        var json = JsonSerializer.Serialize(
            new JournalSetRequest<JournalSetWritePayload> { JournalSet = payload },
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"_destroy\":true", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeCreatePayload_OmitsHistoricalBankAccountOnJournalEntry()
    {
        var entry = JsonSerializer.Deserialize<JournalEntry>("""
            {
              "category": "https://api.freeagent.com/v2/categories/001",
              "description": "Historical bank row",
              "debit_value": "10.0",
              "bank_account": "https://api.freeagent.com/v2/bank_accounts/3"
            }
            """, FreeAgentJsonSerializer.Options)!;

        var payload = JournalSetWritePayload.FromCreate(
            new CreateJournalSetRequest
            {
                Description = "Correction",
                DatedOn = new DateOnly(2026, 1, 15),
                JournalEntries = [entry]
            },
            FreeAgentEnvironment.Production);

        var json = JsonSerializer.Serialize(
            new JournalSetRequest<JournalSetWritePayload> { JournalSet = payload },
            FreeAgentJsonSerializer.Options);

        Assert.DoesNotContain("bank_account", json, StringComparison.Ordinal);
    }
}
