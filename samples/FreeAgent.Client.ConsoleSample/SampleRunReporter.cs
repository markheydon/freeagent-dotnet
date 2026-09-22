using System.Diagnostics;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Outcome for a single console sample run.
/// </summary>
internal enum SampleRunOutcome
{
    /// <summary>Example completed successfully.</summary>
    Passed,

    /// <summary>Example threw an unexpected exception.</summary>
    Failed,

    /// <summary>Example skipped because prerequisites were missing.</summary>
    Skipped,
}

/// <summary>
/// Result for one executed example.
/// </summary>
/// <param name="Category">Example category.</param>
/// <param name="Name">Example name.</param>
/// <param name="Outcome">Run outcome.</param>
/// <param name="Duration">Elapsed time.</param>
/// <param name="Message">Failure or skip message.</param>
internal sealed record SampleRunResult(
    string Category,
    string Name,
    SampleRunOutcome Outcome,
    TimeSpan Duration,
    string? Message);

/// <summary>
/// Prints dotnet test-style status lines and summary for <c>--run-all</c> mode.
/// </summary>
internal sealed class SampleRunReporter
{
    private readonly bool _useColour;
    private readonly List<SampleRunResult> _results = [];
    private readonly Stopwatch _totalStopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Initialises the reporter.
    /// </summary>
    public SampleRunReporter()
    {
        _useColour = !Console.IsOutputRedirected
            && Environment.GetEnvironmentVariable("NO_COLOR") is null;
    }

    /// <summary>
    /// Executes an example and records the outcome.
    /// </summary>
    /// <param name="category">Example category.</param>
    /// <param name="name">Example name.</param>
    /// <param name="action">Example action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RunExampleAsync(
        string category,
        string name,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await action(cancellationToken);
            stopwatch.Stop();
            var result = new SampleRunResult(category, name, SampleRunOutcome.Passed, stopwatch.Elapsed, null);
            _results.Add(result);
            WriteLine(result);
        }
        catch (Samples.SampleSkippedException ex)
        {
            stopwatch.Stop();
            var result = new SampleRunResult(category, name, SampleRunOutcome.Skipped, stopwatch.Elapsed, ex.Message);
            _results.Add(result);
            WriteLine(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var result = new SampleRunResult(category, name, SampleRunOutcome.Failed, stopwatch.Elapsed, ex.Message);
            _results.Add(result);
            WriteLine(result);
        }
    }

    /// <summary>
    /// Prints the final summary and returns the process exit code.
    /// </summary>
    /// <returns><c>0</c> when no failures occurred; otherwise <c>1</c>.</returns>
    public int WriteSummaryAndGetExitCode()
    {
        _totalStopwatch.Stop();

        var passed = _results.Count(r => r.Outcome == SampleRunOutcome.Passed);
        var failed = _results.Count(r => r.Outcome == SampleRunOutcome.Failed);
        var skipped = _results.Count(r => r.Outcome == SampleRunOutcome.Skipped);
        var total = _results.Count;

        Console.WriteLine();
        Console.WriteLine($"Test run for FreeAgent.Client.ConsoleSample ({Environment.Version})");
        Console.WriteLine($"Total examples: {total}");
        WriteSummaryCount("     Passed", passed, ConsoleColor.Green);
        WriteSummaryCount("     Failed", failed, ConsoleColor.Red);
        WriteSummaryCount("    Skipped", skipped, ConsoleColor.DarkYellow);
        Console.WriteLine($" Total time: {_totalStopwatch.Elapsed.TotalSeconds:F4} Seconds");
        Console.WriteLine();

        if (failed > 0)
        {
            WriteColoured($"Failed!  - Failed: {failed}, Passed: {passed}, Skipped: {skipped}, Total: {total}", ConsoleColor.Red);
        }
        else
        {
            WriteColoured($"Passed!  - Failed: {failed}, Passed: {passed}, Skipped: {skipped}, Total: {total}", ConsoleColor.Green);
        }

        Console.WriteLine();
        return failed > 0 ? 1 : 0;
    }

    private void WriteLine(SampleRunResult result)
    {
        var duration = $"({result.Duration.TotalMilliseconds:F0} ms)";
        var label = $"[{result.Category}] {result.Name}";

        switch (result.Outcome)
        {
            case SampleRunOutcome.Passed:
                WriteColoured($"  {SymbolPassed}  {label,-52} {duration}", ConsoleColor.Green);
                break;
            case SampleRunOutcome.Failed:
                WriteColoured($"  {SymbolFailed}  {label,-52} {duration}", ConsoleColor.Red);
                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    WriteColoured($"      {result.Message}", ConsoleColor.Red);
                }

                break;
            case SampleRunOutcome.Skipped:
                WriteColoured($"  {SymbolSkipped}  {label,-52} {duration}", ConsoleColor.DarkYellow);
                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    WriteColoured($"      Skipped: {result.Message}", ConsoleColor.DarkYellow);
                }

                break;
        }
    }

    private void WriteSummaryCount(string label, int count, ConsoleColor colour)
    {
        if (_useColour)
        {
            Console.ForegroundColor = colour;
            Console.Write(label);
            Console.ResetColor();
            Console.WriteLine($": {count}");
        }
        else
        {
            Console.WriteLine($"{label}: {count}");
        }
    }

    private void WriteColoured(string text, ConsoleColor colour)
    {
        if (!_useColour)
        {
            Console.WriteLine(text);
            return;
        }

        Console.ForegroundColor = colour;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    private const string SymbolPassed = "\u2713";
    private const string SymbolFailed = "\u2717";
    private const string SymbolSkipped = "\u2298";
}
