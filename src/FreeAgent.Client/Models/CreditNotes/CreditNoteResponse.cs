using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Response envelope for a single credit note.
/// </summary>
public sealed class CreditNoteResponse
{
    /// <summary>
    /// Credit note payload.
    /// </summary>
    [JsonPropertyName("credit_note")]
    public CreditNote? CreditNote { get; set; }
}
