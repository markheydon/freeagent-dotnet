using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent credit note reconciliation resource URI.
/// </summary>
[JsonConverter(typeof(CreditNoteReconciliationReferenceJsonConverter))]
public readonly record struct CreditNoteReconciliationReference : IResourceReference
{
    /// <summary>
    /// Initialises a credit note reconciliation reference.
    /// </summary>
    /// <param name="uri">Credit note reconciliation resource URI.</param>
    /// <param name="id">Credit note reconciliation identifier.</param>
    public CreditNoteReconciliationReference(string uri, long id)
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
    /// Parses a credit note reconciliation reference from a resource URI.
    /// </summary>
    /// <param name="uri">Credit note reconciliation resource URI.</param>
    /// <returns>Parsed credit note reconciliation reference.</returns>
    public static CreditNoteReconciliationReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "credit_note_reconciliations");

        var id = FreeAgentResourceId.Parse(uri);
        return new CreditNoteReconciliationReference(uri, id);
    }

    /// <summary>
    /// Creates a credit note reconciliation reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Credit note reconciliation identifier.</param>
    /// <returns>Credit note reconciliation reference.</returns>
    public static CreditNoteReconciliationReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}credit_note_reconciliations/{id}";
        return new CreditNoteReconciliationReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Credit note reconciliation reference.</param>
    public static implicit operator string(CreditNoteReconciliationReference reference) => reference.Uri;
}
