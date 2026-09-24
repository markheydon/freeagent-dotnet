using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.RecurringInvoices;

/// <summary>
/// Minimal recurring invoice resource shape for deserialising recurring invoice URI links on invoices.
/// </summary>
/// <remarks>
/// Full recurring invoice operations are not yet implemented in the SDK.
/// </remarks>
public sealed class RecurringInvoice : IFreeAgentResource
{
    /// <summary>
    /// Recurring invoice resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;
}
