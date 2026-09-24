using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent note resource URI.
/// </summary>
[JsonConverter(typeof(NoteReferenceJsonConverter))]
public readonly record struct NoteReference : IResourceReference
{
    /// <summary>
    /// Initialises a note reference.
    /// </summary>
    /// <param name="uri">Note resource URI.</param>
    /// <param name="id">Note identifier.</param>
    public NoteReference(string uri, long id)
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
    /// Parses a note reference from a resource URI.
    /// </summary>
    /// <param name="uri">Note resource URI.</param>
    /// <returns>Parsed note reference.</returns>
    public static NoteReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "notes");

        var id = FreeAgentResourceId.Parse(uri);
        return new NoteReference(uri, id);
    }

    /// <summary>
    /// Creates a note reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Note identifier.</param>
    /// <returns>Note reference.</returns>
    public static NoteReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}notes/{id}";
        return new NoteReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Note reference.</param>
    public static implicit operator string(NoteReference reference) => reference.Uri;
}
