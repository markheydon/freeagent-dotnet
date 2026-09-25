using System.Diagnostics;
using FreeAgent.Client;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Seeds all supported Turpinverse canon resources in dependency order.
/// </summary>
public sealed class TurpinverseSeedOrchestrator
{
    private readonly TurpinverseContactSeeder _contactSeeder;
    private readonly TurpinverseProjectSeeder _projectSeeder;
    private readonly TurpinverseTaskSeeder _taskSeeder;
    private readonly TurpinverseInvoiceSeeder _invoiceSeeder;
    private readonly TurpinverseQuoteSeeder _quoteSeeder;
    private readonly TurpinverseCreditNoteSeeder _creditNoteSeeder;
    private readonly TurpinverseNoteSeeder _noteSeeder;
    private readonly TurpinverseTimeslipSeeder _timeslipSeeder;

    public TurpinverseSeedOrchestrator(
        TurpinverseContactSeeder contactSeeder,
        TurpinverseProjectSeeder projectSeeder,
        TurpinverseTaskSeeder taskSeeder,
        TurpinverseInvoiceSeeder invoiceSeeder,
        TurpinverseQuoteSeeder quoteSeeder,
        TurpinverseCreditNoteSeeder creditNoteSeeder,
        TurpinverseNoteSeeder noteSeeder,
        TurpinverseTimeslipSeeder timeslipSeeder)
    {
        _contactSeeder = contactSeeder ?? throw new ArgumentNullException(nameof(contactSeeder));
        _projectSeeder = projectSeeder ?? throw new ArgumentNullException(nameof(projectSeeder));
        _taskSeeder = taskSeeder ?? throw new ArgumentNullException(nameof(taskSeeder));
        _invoiceSeeder = invoiceSeeder ?? throw new ArgumentNullException(nameof(invoiceSeeder));
        _quoteSeeder = quoteSeeder ?? throw new ArgumentNullException(nameof(quoteSeeder));
        _creditNoteSeeder = creditNoteSeeder ?? throw new ArgumentNullException(nameof(creditNoteSeeder));
        _noteSeeder = noteSeeder ?? throw new ArgumentNullException(nameof(noteSeeder));
        _timeslipSeeder = timeslipSeeder ?? throw new ArgumentNullException(nameof(timeslipSeeder));
    }

    /// <summary>
    /// Seeds contacts, projects, tasks, sales documents, notes, and timeslips from Turpinverse canon.
    /// </summary>
    /// <param name="client">Authenticated FreeAgent client.</param>
    /// <param name="progress">Optional callback invoked after each stage completes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Aggregated per-stage results and elapsed time.</returns>
    public async Task<TurpinverseSeedRunResult> SeedAllAsync(
        FreeAgentClient client,
        IProgress<TurpinverseSeedStageProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var stopwatch = Stopwatch.StartNew();
        var stages = new List<TurpinverseSeedStageResult>();

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Contacts,
            async () =>
            {
                var result = await _contactSeeder.CreateAllOrganisationContactsAsync(client, cancellationToken);
                return FromBulk(result.Created.Count, result.Updated.Count, result.Failures.Select(f => f.Message));
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Projects,
            async () =>
            {
                var result = await _projectSeeder.CreateAllProjectsAsync(client, cancellationToken);
                return FromBulk(result.Created.Count, result.Updated.Count, result.Failures.Select(f => f.Message));
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Tasks,
            async () =>
            {
                var result = await _taskSeeder.CreateAllTurpinverseTasksAsync(client, cancellationToken);
                return FromBulk(result.Created.Count, result.Updated.Count, result.Failures.Select(f => f.Message));
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Invoices,
            async () =>
            {
                var result = await _invoiceSeeder.CreateAllInvoicesAsync(client, cancellationToken);
                return FromBulk(result.Created.Count, result.Updated.Count, result.Failures.Select(f => f.Message));
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Estimates,
            async () =>
            {
                var result = await _quoteSeeder.CreateAllQuotesAsync(client, cancellationToken);
                return FromBulk(result.Created.Count, result.Updated.Count, result.Failures.Select(f => f.Message));
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.CreditNotes,
            async () =>
            {
                var result = await _creditNoteSeeder.CreateAllCreditNotesAsync(client, cancellationToken);
                return FromBulk(result.Created.Count, result.Updated.Count, result.Failures.Select(f => f.Message));
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Notes,
            async () =>
            {
                var failures = new List<string>();
                var created = 0;
                var updated = 0;

                try
                {
                    var contactNote = await _noteSeeder.UpsertTurpinEnterprisesContactNoteAsync(client, cancellationToken);
                    if (contactNote.Action == NoteSeedAction.Created)
                    {
                        created++;
                    }
                    else
                    {
                        updated++;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add($"Turpin Enterprises contact note: {ex.Message}");
                }

                try
                {
                    var projectNote = await _noteSeeder.UpsertBlackBessProjectNoteAsync(client, cancellationToken);
                    if (projectNote.Action == NoteSeedAction.Created)
                    {
                        created++;
                    }
                    else
                    {
                        updated++;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add($"Black Bess project note: {ex.Message}");
                }

                return FromBulk(created, updated, failures);
            },
            progress,
            cancellationToken));

        stages.Add(await RunStageAsync(
            TurpinverseSeedStage.Timeslips,
            async () =>
            {
                var failures = new List<string>();
                var created = 0;
                var updated = 0;

                try
                {
                    var result = await _timeslipSeeder.CreateBlackBessProbeTimeslipAsync(client, cancellationToken);
                    if (result.Action == TimeslipSeedAction.Created)
                    {
                        created++;
                    }
                    else
                    {
                        updated++;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add($"Black Bess probe timeslip: {ex.Message}");
                }

                return FromBulk(created, updated, failures);
            },
            progress,
            cancellationToken));

        stopwatch.Stop();
        return new TurpinverseSeedRunResult(stages, stopwatch.Elapsed);
    }

    private static async Task<TurpinverseSeedStageResult> RunStageAsync(
        TurpinverseSeedStage stage,
        Func<Task<(int Created, int Updated, IReadOnlyList<string> Failures)>> runStage,
        IProgress<TurpinverseSeedStageProgress>? progress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var (created, updated, failures) = await runStage().ConfigureAwait(false);
        var result = FromBulk(stage, created, updated, failures);
        progress?.Report(new TurpinverseSeedStageProgress(stage, result));
        return result;
    }

    private static (int Created, int Updated, IReadOnlyList<string> Failures) FromBulk(
        int created,
        int updated,
        IEnumerable<string> failures) =>
        (created, updated, failures.ToList());

    private static TurpinverseSeedStageResult FromBulk(
        TurpinverseSeedStage stage,
        int created,
        int updated,
        IEnumerable<string> failures) =>
        new(stage, created, updated, failures.ToList());
}
