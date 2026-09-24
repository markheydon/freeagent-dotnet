using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent credit note resource URI.
/// </summary>
[JsonConverter(typeof(CreditNoteReferenceJsonConverter))]
public readonly record struct CreditNoteReference : IResourceReference
{
    /// <summary>
    /// Initialises a credit note reference.
    /// </summary>
    /// <param name="uri">Credit note resource URI.</param>
    /// <param name="id">Credit note identifier.</param>
    public CreditNoteReference(string uri, long id)
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
    /// Parses a credit note reference from a resource URI.
    /// </summary>
    /// <param name="uri">Credit note resource URI.</param>
    /// <returns>Parsed credit note reference.</returns>
    public static CreditNoteReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "credit_notes");

        var id = FreeAgentResourceId.Parse(uri);
        return new CreditNoteReference(uri, id);
    }

    /// <summary>
    /// Creates a credit note reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Credit note identifier.</param>
    /// <returns>Credit note reference.</returns>
    public static CreditNoteReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}credit_notes/{id}";
        return new CreditNoteReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Credit note reference.</param>
    public static implicit operator string(CreditNoteReference reference) => reference.Uri;
}
