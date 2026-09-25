namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.CreditNoteReconciliations.CreditNoteReconciliationsService.GetCreditNoteReconciliationAsync(long, CreditNoteReconciliationGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class CreditNoteReconciliationGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the linked invoice when the response contains only an invoice URI.
    /// </summary>
    public bool IncludeInvoice { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the linked credit note when the response contains only a credit note URI.
    /// </summary>
    public bool IncludeCreditNote { get; init; }
}
