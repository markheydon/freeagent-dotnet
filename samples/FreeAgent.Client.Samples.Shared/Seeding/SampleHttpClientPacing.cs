using FreeAgent.Client;

namespace FreeAgent.Client.Samples.Shared.Seeding;

/// <summary>
/// HTTP client pacing for non-interactive sample runs that issue many API calls.
/// </summary>
public static class SampleHttpClientPacing
{
    /// <summary>
    /// FreeAgent allows 120 requests per minute per user. Six hundred milliseconds between
    /// outbound calls keeps a full smoke or bulk seed run under that ceiling with headroom.
    /// </summary>
    public static FreeAgentHttpClientOptions CreateRunAllOptions() =>
        new()
        {
            MinimumRequestSpacing = TimeSpan.FromMilliseconds(600),
        };
}
