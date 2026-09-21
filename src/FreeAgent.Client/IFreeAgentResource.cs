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
    /// <remarks>
    /// Returns <c>0</c> when parsing fails. Prefer <see cref="FreeAgentResourceExtensions.TryGetResourceId(IFreeAgentResource, out long)"/>
    /// or <see cref="FreeAgentResourceExtensions.GetResourceId(IFreeAgentResource)"/> when a valid identifier is required.
    /// Categories expose nominal codes on the wire — use <c>NominalCode</c> for category API calls.
    /// </remarks>
    long ResourceId { get; }
}
