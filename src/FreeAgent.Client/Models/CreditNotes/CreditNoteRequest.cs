using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Request envelope for creating or updating a credit note.
/// </summary>
internal sealed class CreditNoteRequest
{
    [JsonPropertyName("credit_note")]
    public CreditNoteWritePayload? CreditNote { get; set; }
}
