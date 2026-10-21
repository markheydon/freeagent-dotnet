using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Typed reference to a FreeAgent bank transaction explanation resource URI.
/// </summary>
[JsonConverter(typeof(BankTransactionExplanationReferenceJsonConverter))]
public readonly record struct BankTransactionExplanationReference : IResourceReference
{
    /// <summary>
    /// Initialises a bank transaction explanation reference.
    /// </summary>
    /// <param name="uri">Bank transaction explanation resource URI.</param>
    /// <param name="id">Bank transaction explanation identifier.</param>
    public BankTransactionExplanationReference(string uri, long id)
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
    /// Parses a bank transaction explanation reference from a resource URI.
    /// </summary>
    /// <param name="uri">Bank transaction explanation resource URI.</param>
    /// <returns>Parsed bank transaction explanation reference.</returns>
    public static BankTransactionExplanationReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "bank_transaction_explanations");

        var id = FreeAgentResourceId.Parse(uri);
        return new BankTransactionExplanationReference(uri, id);
    }

    /// <summary>
    /// Creates a bank transaction explanation reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Bank transaction explanation identifier.</param>
    /// <returns>Bank transaction explanation reference.</returns>
    public static BankTransactionExplanationReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}bank_transaction_explanations/{id}";
        return new BankTransactionExplanationReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Bank transaction explanation reference.</param>
    public static implicit operator string(BankTransactionExplanationReference reference) => reference.Uri;
}
