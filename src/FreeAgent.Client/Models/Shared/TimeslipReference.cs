using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent timeslip resource URI.
/// </summary>
[JsonConverter(typeof(TimeslipReferenceJsonConverter))]
public readonly record struct TimeslipReference : IResourceReference
{
    /// <summary>
    /// Initialises a timeslip reference.
    /// </summary>
    /// <param name="uri">Timeslip resource URI.</param>
    /// <param name="id">Timeslip identifier.</param>
    public TimeslipReference(string uri, long id)
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
    /// Parses a timeslip reference from a resource URI.
    /// </summary>
    /// <param name="uri">Timeslip resource URI.</param>
    /// <returns>Parsed timeslip reference.</returns>
    public static TimeslipReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "timeslips");

        var id = FreeAgentResourceId.Parse(uri);
        return new TimeslipReference(uri, id);
    }

    /// <summary>
    /// Creates a timeslip reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Timeslip identifier.</param>
    /// <returns>Timeslip reference.</returns>
    public static TimeslipReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}timeslips/{id}";
        return new TimeslipReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Timeslip reference.</param>
    public static implicit operator string(TimeslipReference reference) => reference.Uri;
}
