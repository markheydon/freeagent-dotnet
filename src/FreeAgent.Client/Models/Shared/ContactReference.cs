using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent contact resource URI.
/// </summary>
[JsonConverter(typeof(ContactReferenceJsonConverter))]
public readonly record struct ContactReference : IResourceReference
{
    /// <summary>
    /// Initialises a contact reference.
    /// </summary>
    /// <param name="uri">Contact resource URI.</param>
    /// <param name="id">Contact identifier.</param>
    public ContactReference(string uri, long id)
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
    /// Parses a contact reference from a resource URI.
    /// </summary>
    /// <param name="uri">Contact resource URI.</param>
    /// <returns>Parsed contact reference.</returns>
    public static ContactReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);

        var id = FreeAgentResourceId.Parse(uri);
        return new ContactReference(uri, id);
    }

    /// <summary>
    /// Creates a contact reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Contact identifier.</param>
    /// <returns>Contact reference.</returns>
    public static ContactReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}contacts/{id}";
        return new ContactReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Contact reference.</param>
    public static implicit operator string(ContactReference reference) => reference.Uri;
}
