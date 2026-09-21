namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Parses numeric identifiers from FreeAgent resource URLs.
/// </summary>
internal static class FreeAgentResourceId
{
    /// <summary>
    /// Parses the numeric identifier from a FreeAgent resource URL.
    /// </summary>
    /// <param name="url">Resource URL.</param>
    /// <returns>Numeric identifier.</returns>
    /// <exception cref="ArgumentException">The URL does not contain a valid numeric identifier.</exception>
    public static long Parse(string url)
    {
        if (!TryParse(url, out var id))
        {
            throw new ArgumentException("The URL does not contain a valid numeric resource identifier.", nameof(url));
        }

        return id;
    }

    /// <summary>
    /// Attempts to parse the numeric identifier from a FreeAgent resource URL.
    /// </summary>
    /// <param name="url">Resource URL.</param>
    /// <param name="id">Parsed identifier when successful.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(string? url, out long id)
    {
        id = 0;

        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        var segments = url.TrimEnd('/').Split('/');
        return segments.Length > 0 && long.TryParse(segments[^1], out id) && id > 0;
    }

    /// <summary>
    /// Validates that a resource URI targets the expected collection segment.
    /// </summary>
    /// <param name="uri">Resource URI.</param>
    /// <param name="resourceSegment">Expected path segment (for example <c>contacts</c>).</param>
    /// <param name="paramName">Parameter name for exceptions.</param>
    /// <exception cref="ArgumentException">The URI does not refer to the expected resource type.</exception>
    internal static void ValidateResourceSegment(string uri, string resourceSegment, string paramName = "uri")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceSegment);

        if (!ContainsResourceSegment(uri, resourceSegment))
        {
            throw new ArgumentException(
                $"The URI does not refer to a {resourceSegment} resource.",
                paramName);
        }
    }

    private static bool ContainsResourceSegment(string uri, string resourceSegment) =>
        uri.Contains($"/{resourceSegment}/", StringComparison.OrdinalIgnoreCase)
        || uri.EndsWith($"/{resourceSegment}", StringComparison.OrdinalIgnoreCase);
}
