using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// PDF payload returned by the credit note PDF endpoint.
/// </summary>
public sealed class CreditNotePdfContent
{
    /// <summary>
    /// Base64-encoded PDF data.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// Response envelope for a credit note PDF download.
/// </summary>
public sealed class CreditNotePdfResponse
{
    /// <summary>
    /// PDF payload.
    /// </summary>
    [JsonPropertyName("pdf")]
    public CreditNotePdfContent? Pdf { get; set; }
}
