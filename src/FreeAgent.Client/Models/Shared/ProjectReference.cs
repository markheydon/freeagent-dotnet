using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent project resource URI.
/// </summary>
[JsonConverter(typeof(ProjectReferenceJsonConverter))]
public readonly record struct ProjectReference : IResourceReference
{
    /// <summary>
    /// Initialises a project reference.
    /// </summary>
    /// <param name="uri">Project resource URI.</param>
    /// <param name="id">Project identifier.</param>
    public ProjectReference(string uri, long id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        Uri = uri;
        Id = id;
    }

    /// <inheritdoc />
    public string Uri { get; }

    /// <inheritdoc />
    public long Id { get; }

    /// <summary>
    /// Parses a project reference from a resource URI.
    /// </summary>
    /// <param name="uri">Project resource URI.</param>
    /// <returns>Parsed project reference.</returns>
    public static ProjectReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "projects");

        var id = FreeAgentResourceId.Parse(uri);
        return new ProjectReference(uri, id);
    }

    /// <summary>
    /// Creates a project reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Project identifier.</param>
    /// <returns>Project reference.</returns>
    public static ProjectReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}projects/{id}";
        return new ProjectReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Project reference.</param>
    public static implicit operator string(ProjectReference reference) => reference.Uri;
}
