using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Typed reference to a FreeAgent capital asset type resource URI.
/// </summary>
[JsonConverter(typeof(CapitalAssetTypeReferenceJsonConverter))]
public readonly record struct CapitalAssetTypeReference : IResourceReference
{
    /// <summary>
    /// Initialises a capital asset type reference.
    /// </summary>
    /// <param name="uri">Capital asset type resource URI.</param>
    /// <param name="id">Capital asset type identifier.</param>
    public CapitalAssetTypeReference(string uri, long id)
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
    /// Parses a capital asset type reference from a resource URI.
    /// </summary>
    /// <param name="uri">Capital asset type resource URI.</param>
    /// <returns>Parsed capital asset type reference.</returns>
    public static CapitalAssetTypeReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "capital_asset_types");

        var id = FreeAgentResourceId.Parse(uri);
        return new CapitalAssetTypeReference(uri, id);
    }

    /// <summary>
    /// Creates a capital asset type reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Capital asset type identifier.</param>
    /// <returns>Capital asset type reference.</returns>
    public static CapitalAssetTypeReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}capital_asset_types/{id}";
        return new CapitalAssetTypeReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Capital asset type reference.</param>
    public static implicit operator string(CapitalAssetTypeReference reference) => reference.Uri;
}
