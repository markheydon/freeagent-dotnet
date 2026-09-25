using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Response envelope for a credit note reconciliation list.
/// </summary>
public sealed class CreditNoteReconciliationsResponse
{
    /// <summary>
    /// Credit note reconciliations returned by FreeAgent.
    /// </summary>
    [JsonPropertyName("credit_note_reconciliations")]
    public List<CreditNoteReconciliation>? CreditNoteReconciliations { get; set; }
}
