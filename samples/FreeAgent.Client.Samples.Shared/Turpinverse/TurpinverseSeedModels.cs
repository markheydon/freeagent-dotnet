namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Identifies a Turpinverse bulk seed stage in dependency order.
/// </summary>
public enum TurpinverseSeedStage
{
    Contacts,
    Projects,
    Tasks,
    Invoices,
    Estimates,
    CreditNotes,
    Notes,
    Timeslips,
}

/// <summary>
/// Dependency order for Turpinverse bulk seeding.
/// </summary>
public static class TurpinverseSeedStageOrder
{
    /// <summary>Stages executed by <see cref="TurpinverseSeedOrchestrator"/>.</summary>
    public static IReadOnlyList<TurpinverseSeedStage> All { get; } =
    [
        TurpinverseSeedStage.Contacts,
        TurpinverseSeedStage.Projects,
        TurpinverseSeedStage.Tasks,
        TurpinverseSeedStage.Invoices,
        TurpinverseSeedStage.Estimates,
        TurpinverseSeedStage.CreditNotes,
        TurpinverseSeedStage.Notes,
        TurpinverseSeedStage.Timeslips,
    ];
}

/// <summary>
/// Progress update for a single Turpinverse seed stage.
/// </summary>
/// <param name="Stage">The stage that completed.</param>
/// <param name="Result">Aggregated counts for the stage.</param>
public sealed record TurpinverseSeedStageProgress(
    TurpinverseSeedStage Stage,
    TurpinverseSeedStageResult Result);

/// <summary>
/// Normalised created, updated, and failure counts for one seed stage.
/// </summary>
/// <param name="Stage">The stage these counts belong to.</param>
/// <param name="Created">Records created in FreeAgent.</param>
/// <param name="Updated">Records updated in FreeAgent.</param>
/// <param name="Failures">Failure messages when individual items could not be seeded.</param>
public sealed record TurpinverseSeedStageResult(
    TurpinverseSeedStage Stage,
    int Created,
    int Updated,
    IReadOnlyList<string> Failures)
{
    /// <summary>Gets a value indicating whether this stage completed without failures.</summary>
    public bool Succeeded => Failures.Count == 0;
}

/// <summary>
/// Aggregated outcome of seeding all Turpinverse canon data in dependency order.
/// </summary>
/// <param name="Stages">Per-stage results in execution order.</param>
/// <param name="Elapsed">Total wall-clock time for the run.</param>
public sealed record TurpinverseSeedRunResult(
    IReadOnlyList<TurpinverseSeedStageResult> Stages,
    TimeSpan Elapsed)
{
    /// <summary>Gets a value indicating whether every stage completed without failures.</summary>
    public bool Succeeded => Stages.All(stage => stage.Succeeded);

    /// <summary>Gets the total number of records created across all stages.</summary>
    public int TotalCreated => Stages.Sum(stage => stage.Created);

    /// <summary>Gets the total number of records updated across all stages.</summary>
    public int TotalUpdated => Stages.Sum(stage => stage.Updated);

    /// <summary>Gets the total number of individual item failures across all stages.</summary>
    public int TotalFailures => Stages.Sum(stage => stage.Failures.Count);
}
