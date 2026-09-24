using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent invoice resource URI.
/// </summary>
[JsonConverter(typeof(InvoiceReferenceJsonConverter))]
public readonly record struct InvoiceReference : IResourceReference
{
    /// <summary>
    /// Initialises an invoice reference.
    /// </summary>
    /// <param name="uri">Invoice resource URI.</param>
    /// <param name="id">Invoice identifier.</param>
    public InvoiceReference(string uri, long id)
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
    /// Parses an invoice reference from a resource URI.
    /// </summary>
    /// <param name="uri">Invoice resource URI.</param>
    /// <returns>Parsed invoice reference.</returns>
    public static InvoiceReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "invoices");

        var id = FreeAgentResourceId.Parse(uri);
        return new InvoiceReference(uri, id);
    }

    /// <summary>
    /// Creates an invoice reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Invoice identifier.</param>
    /// <returns>Invoice reference.</returns>
    public static InvoiceReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}invoices/{id}";
        return new InvoiceReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Invoice reference.</param>
    public static implicit operator string(InvoiceReference reference) => reference.Uri;
}
