namespace FreeAgent.Client;

/// <summary>
/// Common shape for a top-level FreeAgent API resource.
/// </summary>
public interface IFreeAgentResource
{
    /// <summary>
    /// Resource URL from the API.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Numeric resource identifier parsed from <see cref="Url"/> or the API <c>id</c> field.
    /// </summary>
    long ResourceId { get; }
}
