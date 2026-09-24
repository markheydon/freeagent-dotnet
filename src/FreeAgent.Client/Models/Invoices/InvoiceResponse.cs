using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Response envelope for a single invoice.
/// </summary>
public sealed class InvoiceResponse
{
    /// <summary>
    /// Invoice payload.
    /// </summary>
    [JsonPropertyName("invoice")]
    public Invoice? Invoice { get; set; }
}
