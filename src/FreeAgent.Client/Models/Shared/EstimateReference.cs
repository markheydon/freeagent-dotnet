using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent estimate resource URI.
/// </summary>
[JsonConverter(typeof(EstimateReferenceJsonConverter))]
public readonly record struct EstimateReference : IResourceReference
{
    /// <summary>
    /// Initialises an estimate reference.
    /// </summary>
    /// <param name="uri">Estimate resource URI.</param>
    /// <param name="id">Estimate identifier.</param>
    public EstimateReference(string uri, long id)
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
    /// Parses an estimate reference from a resource URI.
    /// </summary>
    /// <param name="uri">Estimate resource URI.</param>
    /// <returns>Parsed estimate reference.</returns>
    public static EstimateReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "estimates");

        var id = FreeAgentResourceId.Parse(uri);
        return new EstimateReference(uri, id);
    }

    /// <summary>
    /// Creates an estimate reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Estimate identifier.</param>
    /// <returns>Estimate reference.</returns>
    public static EstimateReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}estimates/{id}";
        return new EstimateReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Estimate reference.</param>
    public static implicit operator string(EstimateReference reference) => reference.Uri;
}
