using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.JournalSets;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Journal sets endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Journal sets")]
internal sealed class JournalSetSamples(SampleContext context) : IConsoleSampleProvider
{
    private const string ProbeTag = "SDK-CONSOLE-JOURNAL-SET";
    private const string ProbeDescription = "SDK console probe journal set";

    [ConsoleSample(Name = "List journal sets")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListJournalSetsAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.JournalSets.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Journal sets ({items.Count})");
        SampleOutput.WriteRows(items, item => $"{item.ResourceId,8}  {item.DatedOn:yyyy-MM-dd}  {item.Description}  tag={item.Tag}");
    }

    [ConsoleSample(Name = "Get opening balances journal set")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetOpeningBalancesAsync(CancellationToken cancellationToken)
    {
        var opening = await context.Client.JournalSets.GetOpeningBalancesAsync(cancellationToken);

        SampleOutput.WriteHeader("Opening balances journal set");
        SampleOutput.WriteField("Id", opening.ResourceId);
        SampleOutput.WriteField("Description", opening.Description);
        SampleOutput.WriteField("Bank account rows", opening.BankAccounts?.Count ?? 0);
        SampleOutput.WriteField("Stock item rows", opening.StockItems?.Count ?? 0);
    }

    [ConsoleSample(Name = "Get journal set detail with probe create and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetJournalSetDetailAsync(CancellationToken cancellationToken)
    {
        var (probeId, createdForProbe) = await ResolveProbeJournalSetIdAsync(cancellationToken);
        try
        {
            var detail = await context.Client.JournalSets.GetJournalSetAsync(probeId, cancellationToken);

            SampleOutput.WriteHeader("Journal set detail");
            SampleOutput.WriteField("Id", detail.ResourceId);
            SampleOutput.WriteField("Description", detail.Description);
            SampleOutput.WriteField("Entries", detail.JournalEntries?.Count ?? 0);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.JournalSets.DeleteJournalSetAsync(probeId, cancellationToken);
            }
        }
    }

    [ConsoleSample(Name = "Create probe journal set and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeJournalSetAsync(CancellationToken cancellationToken)
    {
        var created = await context.Client.JournalSets.CreateJournalSetAsync(
            new CreateJournalSetRequest
            {
                Description = ProbeDescription,
                DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                Tag = ProbeTag,
                JournalEntries =
                [
                    new JournalEntry { CategoryNominalCode = "001", DebitValue = -1.00m },
                    new JournalEntry { CategoryNominalCode = "901", DebitValue = 1.00m }
                ]
            },
            cancellationToken);

        SampleOutput.WriteHeader("Created probe journal set");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Tag", created.Tag);

        await context.Client.JournalSets.DeleteJournalSetAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteHeader("Deleted probe journal set");
    }

    private async Task<(long ProbeId, bool CreatedForProbe)> ResolveProbeJournalSetIdAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.JournalSets.ListAsync(tag: ProbeTag, cancellationToken: cancellationToken);
        if (items.Count > 0)
        {
            return (items[0].ResourceId, false);
        }

        var created = await context.Client.JournalSets.CreateJournalSetAsync(
            new CreateJournalSetRequest
            {
                Description = ProbeDescription,
                DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                Tag = ProbeTag,
                JournalEntries =
                [
                    new JournalEntry { CategoryNominalCode = "001", DebitValue = -1.00m },
                    new JournalEntry { CategoryNominalCode = "901", DebitValue = 1.00m }
                ]
            },
            cancellationToken);

        return (created.ResourceId, true);
    }
}
