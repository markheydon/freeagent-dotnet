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
    /// When <see langword="true"/>, runs interactive OAuth once and prints the refresh token for CI setup.
    /// </summary>
    public bool BootstrapRefreshToken { get; init; }

    /// <summary>
    /// When <see langword="true"/>, interactive mutating examples may run on non-sandbox environments.
    /// Ignored for <see cref="RunAll"/>.
    /// </summary>
    public bool AllowProductionWrites { get; init; }

    /// <summary>
    /// Parses command-line arguments for console sample options.
    /// </summary>
    /// <param name="args">Application arguments.</param>
    /// <returns>Parsed options.</returns>
    public static ConsoleRunOptions Parse(string[] args)
    {
        var runAll = false;
        var bootstrapRefreshToken = false;
        var allowProductionWrites = false;
        string? categoryFilter = null;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg is "--run-all" or "-a")
            {
                runAll = true;
                continue;
            }

            if (arg is "--bootstrap-refresh-token" or "-b")
            {
                bootstrapRefreshToken = true;
                continue;
            }

            if (arg is "--allow-production-writes")
            {
                allowProductionWrites = true;
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

        if (!string.IsNullOrWhiteSpace(categoryFilter) && !runAll)
        {
            throw new InvalidOperationException("--category requires --run-all.");
        }

        if (bootstrapRefreshToken && runAll)
        {
            throw new InvalidOperationException("--bootstrap-refresh-token cannot be combined with --run-all.");
        }

        if (allowProductionWrites && runAll)
        {
            throw new InvalidOperationException("--allow-production-writes cannot be combined with --run-all.");
        }

        return new ConsoleRunOptions
        {
            RunAll = runAll,
            BootstrapRefreshToken = bootstrapRefreshToken,
            AllowProductionWrites = allowProductionWrites,
            CategoryFilter = categoryFilter,
        };
    }
}
