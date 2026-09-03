namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Parses contact resource identifiers from FreeAgent contact URLs.
/// </summary>
internal static class ContactUrlParser
{
    /// <summary>
    /// Extracts the numeric contact ID from a FreeAgent contact resource URL.
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
