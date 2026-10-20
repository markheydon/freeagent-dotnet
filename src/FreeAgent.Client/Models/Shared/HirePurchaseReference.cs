using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent hire purchase resource URI.
/// </summary>
[JsonConverter(typeof(HirePurchaseReferenceJsonConverter))]
public readonly record struct HirePurchaseReference : IResourceReference
{
    /// <summary>
    /// Initialises a hire purchase reference.
    /// </summary>
    /// <param name="uri">Hire purchase resource URI.</param>
    /// <param name="id">Hire purchase identifier.</param>
    public HirePurchaseReference(string uri, long id)
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
    /// Parses a hire purchase reference from a resource URI.
    /// </summary>
    /// <param name="uri">Hire purchase resource URI.</param>
    /// <returns>Parsed hire purchase reference.</returns>
    public static HirePurchaseReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "hire_purchases");

        var id = FreeAgentResourceId.Parse(uri);
        return new HirePurchaseReference(uri, id);
    }

    /// <summary>
    /// Creates a hire purchase reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Hire purchase identifier.</param>
    /// <returns>Hire purchase reference.</returns>
    public static HirePurchaseReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}hire_purchases/{id}";
        return new HirePurchaseReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Hire purchase reference.</param>
    public static implicit operator string(HirePurchaseReference reference) => reference.Uri;
}
