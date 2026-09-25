namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// HTTP client pacing for non-interactive <c>--run-all</c> smoke runs.
/// </summary>
internal static class RunAllHttpClientOptions
{
    /// <summary>
    /// FreeAgent allows 120 requests per minute per user. Six hundred milliseconds between
    /// outbound calls keeps a full smoke run under that ceiling with headroom.
    /// </summary>
    public static FreeAgentHttpClientOptions Create() =>
        new()
        {
            MinimumRequestSpacing = TimeSpan.FromMilliseconds(600),
        };
}
