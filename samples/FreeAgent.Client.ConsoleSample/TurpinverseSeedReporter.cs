using FreeAgent.Client.Samples.Shared.Turpinverse;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Writes Turpinverse bulk seed results to the console.
/// </summary>
internal static class TurpinverseSeedReporter
{
    /// <summary>
    /// Prints a stage summary table and overall totals.
    /// </summary>
    /// <param name="result">Aggregated seed run result.</param>
    public static void WriteSummary(TurpinverseSeedRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        Console.WriteLine();
        Console.WriteLine("Turpinverse seed summary");
        Console.WriteLine("------------------------");
        Console.WriteLine($"{"Stage",-14} {"Created",8} {"Updated",8} {"Failures",8} {"Status",6}");

        foreach (var stage in result.Stages)
        {
            Console.WriteLine(
                $"{FormatStage(stage.Stage),-14} {stage.Created,8} {stage.Updated,8} {stage.Failures.Count,8} {(stage.Succeeded ? "OK" : "FAIL"),6}");
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Total: created {result.TotalCreated}, updated {result.TotalUpdated}, failures {result.TotalFailures}, elapsed {result.Elapsed.TotalSeconds:0.0}s");

        foreach (var stage in result.Stages.Where(static stage => stage.Failures.Count > 0))
        {
            Console.WriteLine();
            Console.WriteLine($"{FormatStage(stage.Stage)} failures:");
            foreach (var failure in stage.Failures)
            {
                Console.WriteLine($"  - {failure}");
            }
        }

        Console.WriteLine();
    }

    private static string FormatStage(TurpinverseSeedStage stage) => stage switch
    {
        TurpinverseSeedStage.Contacts => "Contacts",
        TurpinverseSeedStage.Projects => "Projects",
        TurpinverseSeedStage.Tasks => "Tasks",
        TurpinverseSeedStage.Invoices => "Invoices",
        TurpinverseSeedStage.Estimates => "Estimates",
        TurpinverseSeedStage.CreditNotes => "CreditNotes",
        TurpinverseSeedStage.Notes => "Notes",
        TurpinverseSeedStage.Timeslips => "Timeslips",
        _ => stage.ToString(),
    };
}
