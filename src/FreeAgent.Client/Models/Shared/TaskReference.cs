using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent task resource URI.
/// </summary>
[JsonConverter(typeof(TaskReferenceJsonConverter))]
public readonly record struct TaskReference : IResourceReference
{
    /// <summary>
    /// Initialises a task reference.
    /// </summary>
    /// <param name="uri">Task resource URI.</param>
    /// <param name="id">Task identifier.</param>
    public TaskReference(string uri, long id)
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
    /// Parses a task reference from a resource URI.
    /// </summary>
    /// <param name="uri">Task resource URI.</param>
    /// <returns>Parsed task reference.</returns>
    public static TaskReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "tasks");

        var id = FreeAgentResourceId.Parse(uri);
        return new TaskReference(uri, id);
    }

    /// <summary>
    /// Creates a task reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Task identifier.</param>
    /// <returns>Task reference.</returns>
    public static TaskReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}tasks/{id}";
        return new TaskReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Task reference.</param>
    public static implicit operator string(TaskReference reference) => reference.Uri;
}
