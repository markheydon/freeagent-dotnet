using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent recurring invoice resource URI.
/// </summary>
[JsonConverter(typeof(RecurringInvoiceReferenceJsonConverter))]
public readonly record struct RecurringInvoiceReference : IResourceReference
{
    /// <summary>
    /// Initialises a recurring invoice reference.
    /// </summary>
    /// <param name="uri">Recurring invoice resource URI.</param>
    /// <param name="id">Recurring invoice identifier.</param>
    public RecurringInvoiceReference(string uri, long id)
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
    /// Parses a recurring invoice reference from a resource URI.
    /// </summary>
    /// <param name="uri">Recurring invoice resource URI.</param>
    /// <returns>Parsed recurring invoice reference.</returns>
    public static RecurringInvoiceReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "recurring_invoices");

        var id = FreeAgentResourceId.Parse(uri);
        return new RecurringInvoiceReference(uri, id);
    }

    /// <summary>
    /// Creates a recurring invoice reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="id">Recurring invoice identifier.</param>
    /// <returns>Recurring invoice reference.</returns>
    public static RecurringInvoiceReference ForEnvironment(FreeAgentEnvironment environment, long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}recurring_invoices/{id}";
        return new RecurringInvoiceReference(uri, id);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Recurring invoice reference.</param>
    public static implicit operator string(RecurringInvoiceReference reference) => reference.Uri;
}
