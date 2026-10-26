using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Parses numeric resource identifiers from FreeAgent resource URLs.
/// </summary>
public static class FreeAgentResourceUri
{
    /// <summary>
    /// Attempts to parse the numeric identifier from a FreeAgent resource URL.
    /// </summary>
    /// <param name="url">Resource URL (for example from a webhook or stored wire value).</param>
    /// <param name="id">Parsed identifier when successful.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParseResourceId(string? url, out long id) =>
        FreeAgentResourceId.TryParse(url, out id);

    /// <summary>
    /// Parses the numeric identifier from a FreeAgent resource URL.
    /// </summary>
    /// <param name="url">Resource URL.</param>
    /// <returns>Numeric identifier.</returns>
    /// <exception cref="ArgumentException">The URL does not contain a valid numeric resource identifier.</exception>
    public static long ParseResourceId(string url) => FreeAgentResourceId.Parse(url);
}
