using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Request envelope for creating or updating a credit note reconciliation.
/// </summary>
internal sealed class CreditNoteReconciliationRequest
{
    [JsonPropertyName("credit_note_reconciliation")]
    public CreditNoteReconciliationWritePayload? CreditNoteReconciliation { get; set; }
}
