namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Parses user resource identifiers from FreeAgent user URLs.
/// </summary>
internal static class UserUrlParser
{
    /// <summary>
    /// Extracts the numeric user ID from a FreeAgent user resource URL.
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
