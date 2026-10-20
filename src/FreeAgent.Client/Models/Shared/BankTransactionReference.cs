using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent bank transaction resource URI.
/// </summary>
[JsonConverter(typeof(BankTransactionReferenceJsonConverter))]
public readonly record struct BankTransactionReference : IResourceReference
{
    /// <summary>
    /// Initialises a bank transaction reference.
    /// </summary>
    /// <param name="uri">Bank transaction resource URI.</param>
    /// <param name="id">Bank transaction identifier.</param>
    public BankTransactionReference(string uri, long id)
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
    /// Parses a bank transaction reference from a resource URI.
    /// </summary>
    /// <param name="uri">Bank transaction resource URI.</param>
    /// <returns>Parsed bank transaction reference.</returns>
    public static BankTransactionReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "bank_transactions");

        var id = FreeAgentResourceId.Parse(uri);
        return new BankTransactionReference(uri, id);
    }

    /// <summary>
    /// Creates a bank transaction reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Bank transaction identifier.</param>
    /// <returns>Bank transaction reference.</returns>
    public static BankTransactionReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}bank_transactions/{id}";
        return new BankTransactionReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Bank transaction reference.</param>
    public static implicit operator string(BankTransactionReference reference) => reference.Uri;
}
