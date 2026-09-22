namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Command-line options for the console sample runner.
/// </summary>
internal sealed class ConsoleRunOptions
{
    /// <summary>When <see langword="true"/>, runs every registered example without the interactive menu.</summary>
    public bool RunAll { get; init; }

    /// <summary>Optional category filter for <see cref="RunAll"/> (for example <c>Contacts</c>).</summary>
    public string? CategoryFilter { get; init; }

    /// <summary>
    /// Parses command-line arguments for console sample options.
    /// </summary>
    /// <param name="args">Application arguments.</param>
    /// <returns>Parsed options.</returns>
    public static ConsoleRunOptions Parse(string[] args)
    {
        var runAll = false;
        string? categoryFilter = null;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg is "--run-all" or "-a")
            {
                runAll = true;
                continue;
            }

            if (arg is "--category" or "-c")
            {
                if (i + 1 >= args.Length || string.IsNullOrWhiteSpace(args[i + 1]))
                {
                    throw new InvalidOperationException("--category requires a value (for example --category Contacts).");
                }

                categoryFilter = args[++i].Trim();
            }
        }

        return new ConsoleRunOptions
        {
            RunAll = runAll,
            CategoryFilter = categoryFilter,
        };
    }
}
