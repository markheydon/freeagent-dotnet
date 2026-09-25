using System.Diagnostics;
using FreeAgent.Client;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Seeds all supported Turpinverse canon resources in dependency order.
/// </summary>
public sealed class TurpinverseSeedOrchestrator
{
    private readonly TurpinverseCanonCoordinator _canonCoordinator;
    private readonly TurpinverseContactSeeder _contactSeeder;
    private readonly TurpinverseProjectSeeder _projectSeeder;
    private readonly TurpinverseTaskSeeder _taskSeeder;
    private readonly TurpinverseInvoiceSeeder _invoiceSeeder;
    private readonly TurpinverseQuoteSeeder _quoteSeeder;
    private readonly TurpinverseCreditNoteSeeder _creditNoteSeeder;
    private readonly TurpinverseNoteSeeder _noteSeeder;
    private readonly TurpinverseTimeslipSeeder _timeslipSeeder;

    public TurpinverseSeedOrchestrator(
        TurpinverseCanonCoordinator canonCoordinator,
        TurpinverseContactSeeder contactSeeder,
        TurpinverseProjectSeeder projectSeeder,
        TurpinverseTaskSeeder taskSeeder,
        TurpinverseInvoiceSeeder invoiceSeeder,
        TurpinverseQuoteSeeder quoteSeeder,
        TurpinverseCreditNoteSeeder creditNoteSeeder,
        TurpinverseNoteSeeder noteSeeder,
        TurpinverseTimeslipSeeder timeslipSeeder)
    {
        _canonCoordinator = canonCoordinator ?? throw new ArgumentNullException(nameof(canonCoordinator));
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

        await _canonCoordinator.PrepareForBulkSeedAsync(cancellationToken).ConfigureAwait(false);

        var stopwatch = Stopwatch.StartNew();
        var stages = new List<TurpinverseSeedStageResult>();

        foreach (var stage in TurpinverseSeedStageOrder.All)
        {
            stages.Add(await RunStageAsync(
                stage,
                () => ExecuteStageAsync(stage, client, cancellationToken),
                progress,
                cancellationToken));

            if (stage == TurpinverseSeedStage.Contacts && stages[^1].Failures.Count > 0)
            {
                break;
            }
        }

        stopwatch.Stop();
        return new TurpinverseSeedRunResult(stages, stopwatch.Elapsed);
    }

    private async Task<(int Created, int Updated, IReadOnlyList<string> Failures)> ExecuteStageAsync(
        TurpinverseSeedStage stage,
        FreeAgentClient client,
        CancellationToken cancellationToken) =>
        stage switch
        {
            TurpinverseSeedStage.Contacts => await ExecuteBulkStageAsync(
                () => _contactSeeder.CreateAllOrganisationContactsAsync(client, refreshCanon: false, cancellationToken),
                static result => (result.Created.Count, result.Updated.Count, result.Failures.Select(static failure => failure.Message))),
            TurpinverseSeedStage.Projects => await ExecuteBulkStageAsync(
                () => _projectSeeder.CreateAllProjectsAsync(client, refreshCanon: false, cancellationToken),
                static result => (result.Created.Count, result.Updated.Count, result.Failures.Select(static failure => failure.Message))),
            TurpinverseSeedStage.Tasks => await ExecuteBulkStageAsync(
                () => _taskSeeder.CreateAllTurpinverseTasksAsync(client, refreshCanon: false, cancellationToken),
                static result => (result.Created.Count, result.Updated.Count, result.Failures.Select(static failure => failure.Message))),
            TurpinverseSeedStage.Invoices => await ExecuteBulkStageAsync(
                () => _invoiceSeeder.CreateAllInvoicesAsync(client, refreshCanon: false, cancellationToken),
                static result => (result.Created.Count, result.Updated.Count, result.Failures.Select(static failure => failure.Message))),
            TurpinverseSeedStage.Estimates => await ExecuteBulkStageAsync(
                () => _quoteSeeder.CreateAllQuotesAsync(client, refreshCanon: false, cancellationToken),
                static result => (result.Created.Count, result.Updated.Count, result.Failures.Select(static failure => failure.Message))),
            TurpinverseSeedStage.CreditNotes => await ExecuteBulkStageAsync(
                () => _creditNoteSeeder.CreateAllCreditNotesAsync(client, refreshCanon: false, cancellationToken),
                static result => (result.Created.Count, result.Updated.Count, result.Failures.Select(static failure => failure.Message))),
            TurpinverseSeedStage.Notes => await ExecuteNotesStageAsync(client, cancellationToken),
            TurpinverseSeedStage.Timeslips => await ExecuteTimeslipsStageAsync(client, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported Turpinverse seed stage '{stage}'.")
        };

    private static async Task<(int Created, int Updated, IReadOnlyList<string> Failures)> ExecuteBulkStageAsync<TBulkResult>(
        Func<Task<TBulkResult>> runStage,
        Func<TBulkResult, (int Created, int Updated, IEnumerable<string> Failures)> mapResult)
    {
        var result = await runStage().ConfigureAwait(false);
        var (created, updated, failures) = mapResult(result);
        return (created, updated, failures.ToList());
    }

    private async Task<(int Created, int Updated, IReadOnlyList<string> Failures)> ExecuteNotesStageAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
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

        return (created, updated, failures);
    }

    private async Task<(int Created, int Updated, IReadOnlyList<string> Failures)> ExecuteTimeslipsStageAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
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

        return (created, updated, failures);
    }

    private static async Task<TurpinverseSeedStageResult> RunStageAsync(
        TurpinverseSeedStage stage,
        Func<Task<(int Created, int Updated, IReadOnlyList<string> Failures)>> runStage,
        IProgress<TurpinverseSeedStageProgress>? progress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var (created, updated, failures) = await runStage().ConfigureAwait(false);
        var result = new TurpinverseSeedStageResult(stage, created, updated, failures);
        progress?.Report(new TurpinverseSeedStageProgress(stage, result));
        return result;
    }
}
