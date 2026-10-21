using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Typed reference to a FreeAgent property resource URI.
/// </summary>
[JsonConverter(typeof(PropertyReferenceJsonConverter))]
public readonly record struct PropertyReference : IResourceReference
{
    /// <summary>
    /// Initialises a property reference.
    /// </summary>
    /// <param name="uri">Property resource URI.</param>
    /// <param name="id">Property identifier.</param>
    public PropertyReference(string uri, long id)
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
    /// Parses a property reference from a resource URI.
    /// </summary>
    /// <param name="uri">Property resource URI.</param>
    /// <returns>Parsed property reference.</returns>
    public static PropertyReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "properties");

        var id = FreeAgentResourceId.Parse(uri);
        return new PropertyReference(uri, id);
    }

    /// <summary>
    /// Creates a property reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Property identifier.</param>
    /// <returns>Property reference.</returns>
    public static PropertyReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}properties/{id}";
        return new PropertyReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Property reference.</param>
    public static implicit operator string(PropertyReference reference) => reference.Uri;
}
