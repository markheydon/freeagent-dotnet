namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Raised when an example cannot run because prerequisites are missing.
/// </summary>
internal sealed class SampleSkippedException : Exception
{
    /// <summary>
    /// Initialises a new skip exception with a reason.
    /// </summary>
    /// <param name="reason">Human-readable skip reason.</param>
    public SampleSkippedException(string reason)
        : base(reason)
    {
    }
}
