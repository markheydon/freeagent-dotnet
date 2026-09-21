namespace FreeAgent.Client;

/// <summary>
/// Typed identity for a FreeAgent resource URI.
/// </summary>
public interface IResourceReference
{
    /// <summary>
    /// Resource URI for the API environment.
    /// </summary>
    string Uri { get; }

    /// <summary>
    /// Numeric resource identifier.
    /// </summary>
    long Id { get; }
}
