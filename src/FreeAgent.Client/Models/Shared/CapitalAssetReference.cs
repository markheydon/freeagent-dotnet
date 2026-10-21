using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent capital asset resource URI.
/// </summary>
[JsonConverter(typeof(CapitalAssetReferenceJsonConverter))]
public readonly record struct CapitalAssetReference : IResourceReference
{
    /// <summary>
    /// Initialises a capital asset reference.
    /// </summary>
    /// <param name="uri">Capital asset resource URI.</param>
    /// <param name="id">Capital asset identifier.</param>
    public CapitalAssetReference(string uri, long id)
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
    /// Parses a capital asset reference from a resource URI.
    /// </summary>
    /// <param name="uri">Capital asset resource URI.</param>
    /// <returns>Parsed capital asset reference.</returns>
    public static CapitalAssetReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "capital_assets");

        var id = FreeAgentResourceId.Parse(uri);
        return new CapitalAssetReference(uri, id);
    }

    /// <summary>
    /// Creates a capital asset reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Capital asset identifier.</param>
    /// <returns>Capital asset reference.</returns>
    public static CapitalAssetReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}capital_assets/{id}";
        return new CapitalAssetReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Capital asset reference.</param>
    public static implicit operator string(CapitalAssetReference reference) => reference.Uri;
}
