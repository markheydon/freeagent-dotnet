namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Parses project resource identifiers from FreeAgent project URLs.
/// </summary>
internal static class ProjectUrlParser
{
    /// <summary>
    /// Extracts the numeric project ID from a FreeAgent project resource URL.
    /// </summary>
    public static long? ParseId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var segments = url.TrimEnd('/').Split('/');
        return long.TryParse(segments[^1], out var id) ? id : null;
    }
}
