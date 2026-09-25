using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Response envelope for a paginated credit note list.
/// </summary>
public sealed class CreditNotesResponse
{
    /// <summary>
    /// Credit notes on this page.
    /// </summary>
    [JsonPropertyName("credit_notes")]
    public List<CreditNote>? CreditNotes { get; set; }
}
