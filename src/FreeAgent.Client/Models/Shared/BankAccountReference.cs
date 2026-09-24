using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent bank account resource URI.
/// </summary>
[JsonConverter(typeof(BankAccountReferenceJsonConverter))]
public readonly record struct BankAccountReference : IResourceReference
{
    /// <summary>
    /// Initialises a bank account reference.
    /// </summary>
    /// <param name="uri">Bank account resource URI.</param>
    /// <param name="id">Bank account identifier.</param>
    public BankAccountReference(string uri, long id)
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
    /// Parses a bank account reference from a resource URI.
    /// </summary>
    /// <param name="uri">Bank account resource URI.</param>
    /// <returns>Parsed bank account reference.</returns>
    public static BankAccountReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "bank_accounts");

        var id = FreeAgentResourceId.Parse(uri);
        return new BankAccountReference(uri, id);
    }

    /// <summary>
    /// Creates a bank account reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Bank account identifier.</param>
    /// <returns>Bank account reference.</returns>
    public static BankAccountReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}bank_accounts/{id}";
        return new BankAccountReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Bank account reference.</param>
    public static implicit operator string(BankAccountReference reference) => reference.Uri;
}
