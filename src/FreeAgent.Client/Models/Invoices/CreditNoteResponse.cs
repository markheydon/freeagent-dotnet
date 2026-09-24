using System.Text.Json.Serialization;
using FreeAgent.Client.Models.CreditNotes;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Response envelope when an invoice is converted to a credit note.
/// </summary>
public sealed class CreditNoteResponse
{
    /// <summary>
    /// Converted credit note payload.
    /// </summary>
    [JsonPropertyName("credit_note")]
    public CreditNote? CreditNote { get; set; }
}
