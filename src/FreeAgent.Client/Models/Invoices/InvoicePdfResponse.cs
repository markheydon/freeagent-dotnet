using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// PDF payload returned by the invoice PDF endpoint.
/// </summary>
public sealed class InvoicePdfContent
{
    /// <summary>
    /// Base64-encoded PDF data.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// Response envelope for an invoice PDF download.
/// </summary>
public sealed class InvoicePdfResponse
{
    /// <summary>
    /// PDF payload.
    /// </summary>
    [JsonPropertyName("pdf")]
    public InvoicePdfContent? Pdf { get; set; }
}
