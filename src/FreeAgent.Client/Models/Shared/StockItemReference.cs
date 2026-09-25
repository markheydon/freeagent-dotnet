using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent stock item resource URI.
/// </summary>
[JsonConverter(typeof(StockItemReferenceJsonConverter))]
public readonly record struct StockItemReference : IResourceReference
{
    /// <summary>
    /// Initialises a stock item reference.
    /// </summary>
    /// <param name="uri">Stock item resource URI.</param>
    /// <param name="id">Stock item identifier.</param>
    public StockItemReference(string uri, long id)
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
    /// Parses a stock item reference from a resource URI.
    /// </summary>
    /// <param name="uri">Stock item resource URI.</param>
    /// <returns>Parsed stock item reference.</returns>
    public static StockItemReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "stock_items");

        var id = FreeAgentResourceId.Parse(uri);
        return new StockItemReference(uri, id);
    }

    /// <summary>
    /// Creates a stock item reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Stock item identifier.</param>
    /// <returns>Stock item reference.</returns>
    public static StockItemReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}stock_items/{id}";
        return new StockItemReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Stock item reference.</param>
    public static implicit operator string(StockItemReference reference) => reference.Uri;
}
