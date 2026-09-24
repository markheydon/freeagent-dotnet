using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent user resource URI.
/// </summary>
[JsonConverter(typeof(UserReferenceJsonConverter))]
public readonly record struct UserReference : IResourceReference
{
    /// <summary>
    /// Initialises a user reference.
    /// </summary>
    /// <param name="uri">User resource URI.</param>
    /// <param name="id">User identifier.</param>
    public UserReference(string uri, long id)
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
    /// Parses a user reference from a resource URI.
    /// </summary>
    /// <param name="uri">User resource URI.</param>
    /// <returns>Parsed user reference.</returns>
    public static UserReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "users");

        var id = FreeAgentResourceId.Parse(uri);
        return new UserReference(uri, id);
    }

    /// <summary>
    /// Creates a user reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">User identifier.</param>
    /// <returns>User reference.</returns>
    public static UserReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}users/{id}";
        return new UserReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">User reference.</param>
    public static implicit operator string(UserReference reference) => reference.Uri;
}
