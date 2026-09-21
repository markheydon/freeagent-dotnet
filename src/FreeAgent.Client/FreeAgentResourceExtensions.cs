namespace FreeAgent.Client;

/// <summary>
/// Helpers for working with <see cref="IFreeAgentResource"/> identifiers.
/// </summary>
public static class FreeAgentResourceExtensions
{
    /// <summary>
    /// Attempts to obtain a valid numeric resource identifier.
    /// </summary>
    /// <param name="resource">Resource instance.</param>
    /// <param name="id">Parsed identifier when successful.</param>
    /// <returns><see langword="true"/> when <see cref="IFreeAgentResource.ResourceId"/> is greater than zero.</returns>
    public static bool TryGetResourceId(this IFreeAgentResource resource, out long id)
    {
        ArgumentNullException.ThrowIfNull(resource);

        id = resource.ResourceId;
        return id > 0;
    }

    /// <summary>
    /// Returns a valid numeric resource identifier.
    /// </summary>
    /// <param name="resource">Resource instance.</param>
    /// <returns>Parsed identifier.</returns>
    /// <exception cref="InvalidOperationException">The resource URL or identifier could not be parsed.</exception>
    public static long GetResourceId(this IFreeAgentResource resource)
    {
        if (!TryGetResourceId(resource, out var id))
        {
            throw new InvalidOperationException(
                $"Could not parse a valid resource identifier from '{resource.Url}'.");
        }

        return id;
    }
}
