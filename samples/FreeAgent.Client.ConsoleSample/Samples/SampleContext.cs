using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Shared context for console SDK examples.
/// </summary>
internal sealed class SampleContext
{
    /// <summary>
    /// Initialises a new sample context.
    /// </summary>
    /// <param name="client">Authenticated FreeAgent API client.</param>
    public SampleContext(FreeAgentClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        Client = client;
        Data = new SampleData(this);
    }

    /// <summary>
    /// Authenticated FreeAgent API client.
    /// </summary>
    public FreeAgentClient Client { get; }

    /// <summary>
    /// Helpers for resolving sandbox data without user input.
    /// </summary>
    public SampleData Data { get; }

    /// <summary>
    /// Marks the current example as skipped with a reason.
    /// </summary>
    /// <param name="reason">Why the example could not run.</param>
    [DoesNotReturn]
    public static void Skip(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        throw new SampleSkippedException(reason);
    }
}
