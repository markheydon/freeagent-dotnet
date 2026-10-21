using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Typed reference to a FreeAgent bill resource URI.
/// </summary>
[JsonConverter(typeof(BillReferenceJsonConverter))]
public readonly record struct BillReference : IResourceReference
{
    /// <summary>
    /// Initialises a bill reference.
    /// </summary>
    /// <param name="uri">Bill resource URI.</param>
    /// <param name="id">Bill identifier.</param>
    public BillReference(string uri, long id)
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
    /// Parses a bill reference from a resource URI.
    /// </summary>
    /// <param name="uri">Bill resource URI.</param>
    /// <returns>Parsed bill reference.</returns>
    public static BillReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "bills");

        var id = FreeAgentResourceId.Parse(uri);
        return new BillReference(uri, id);
    }

    /// <summary>
    /// Creates a bill reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Bill identifier.</param>
    /// <returns>Bill reference.</returns>
    public static BillReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}bills/{id}";
        return new BillReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Bill reference.</param>
    public static implicit operator string(BillReference reference) => reference.Uri;
}
