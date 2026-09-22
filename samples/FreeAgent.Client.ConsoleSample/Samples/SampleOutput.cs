namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Simple console formatting helpers for SDK examples.
/// </summary>
internal static class SampleOutput
{
    /// <summary>
    /// Writes a section header.
    /// </summary>
    /// <param name="title">Section title.</param>
    public static void WriteHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
        Console.WriteLine();
    }

    /// <summary>
    /// Writes a key/value line.
    /// </summary>
    /// <param name="label">Field label.</param>
    /// <param name="value">Field value.</param>
    public static void WriteField(string label, object? value)
    {
        Console.WriteLine($"  {label,-20} {value}");
    }

    /// <summary>
    /// Writes a list of items with an optional maximum row count.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="items">Items to display.</param>
    /// <param name="formatter">Row formatter.</param>
    /// <param name="maxRows">Maximum rows to print.</param>
    public static void WriteRows<T>(IEnumerable<T> items, Func<T, string> formatter, int maxRows = 10)
    {
        var list = items.ToList();
        var shown = Math.Min(list.Count, maxRows);

        for (var i = 0; i < shown; i++)
        {
            Console.WriteLine($"  {formatter(list[i])}");
        }

        if (list.Count > shown)
        {
            Console.WriteLine($"  ... and {list.Count - shown} more");
        }
    }
}
