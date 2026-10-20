using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent attachment resource URI.
/// </summary>
[JsonConverter(typeof(AttachmentReferenceJsonConverter))]
public readonly record struct AttachmentReference : IResourceReference
{
    /// <summary>
    /// Initialises an attachment reference.
    /// </summary>
    /// <param name="uri">Attachment resource URI.</param>
    /// <param name="id">Attachment identifier.</param>
    public AttachmentReference(string uri, long id)
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
    /// Parses an attachment reference from a resource URI.
    /// </summary>
    /// <param name="uri">Attachment resource URI.</param>
    /// <returns>Parsed attachment reference.</returns>
    public static AttachmentReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "attachments");

        var id = FreeAgentResourceId.Parse(uri);
        return new AttachmentReference(uri, id);
    }

    /// <summary>
    /// Creates an attachment reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Attachment identifier.</param>
    /// <returns>Attachment reference.</returns>
    public static AttachmentReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}attachments/{id}";
        return new AttachmentReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Attachment reference.</param>
    public static implicit operator string(AttachmentReference reference) => reference.Uri;
}
