using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNoteReconciliations;

/// <summary>
/// Response envelope for a single credit note reconciliation.
/// </summary>
public sealed class CreditNoteReconciliationResponse
{
    /// <summary>
    /// Credit note reconciliation payload.
    /// </summary>
    [JsonPropertyName("credit_note_reconciliation")]
    public CreditNoteReconciliation? CreditNoteReconciliation { get; set; }

    /// <summary>
    /// Alternate envelope key shown on the FreeAgent GET docs page (object, not array).
    /// </summary>
    [JsonPropertyName("credit_note_reconciliations")]
    [JsonInclude]
    internal CreditNoteReconciliation? CreditNoteReconciliationAlternateEnvelope { get; set; }

    /// <summary>
    /// Returns the reconciliation payload from whichever envelope key FreeAgent returned.
    /// </summary>
    internal CreditNoteReconciliation? ResolvePayload() =>
        CreditNoteReconciliation ?? CreditNoteReconciliationAlternateEnvelope;
}
