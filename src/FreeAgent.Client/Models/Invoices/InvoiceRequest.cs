using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Request envelope for invoice create and update operations.
/// </summary>
internal sealed class InvoiceRequest
{
    [JsonPropertyName("invoice")]
    public InvoiceWritePayload? Invoice { get; set; }
}
