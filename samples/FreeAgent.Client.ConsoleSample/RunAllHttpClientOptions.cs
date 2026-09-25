using FreeAgent.Client.Samples.Shared.Seeding;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// HTTP client pacing for non-interactive <c>--run-all</c> smoke runs.
/// </summary>
internal static class RunAllHttpClientOptions
{
    /// <summary>
    /// Creates pacing options for non-interactive smoke and bulk seed runs.
    /// </summary>
    public static FreeAgentHttpClientOptions Create() => SampleHttpClientPacing.CreateRunAllOptions();
}
