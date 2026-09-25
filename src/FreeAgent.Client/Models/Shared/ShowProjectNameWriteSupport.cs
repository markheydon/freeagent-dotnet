using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Shared write rules for <c>show_project_name</c> on billing documents.
/// </summary>
internal static class ShowProjectNameWriteSupport
{
    /// <summary>
    /// Validates that <see cref="Invoice.ShowProjectName"/> can be written on update.
    /// </summary>
    /// <param name="invoice">Invoice update model.</param>
    /// <exception cref="ArgumentNullException"><paramref name="invoice"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <see cref="Invoice.ShowProjectName"/> is set but <see cref="Invoice.Status"/> is not.
    /// </exception>
    internal static void ValidateInvoiceUpdate(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ValidateUpdateWrite(invoice.ShowProjectName, invoice.Status, nameof(invoice));
    }

    /// <summary>
    /// Validates that <see cref="CreditNote.ShowProjectName"/> can be written on update.
    /// </summary>
    /// <param name="creditNote">Credit note update model.</param>
    /// <exception cref="ArgumentNullException"><paramref name="creditNote"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <see cref="CreditNote.ShowProjectName"/> is set but <see cref="CreditNote.Status"/> is not.
    /// </exception>
    internal static void ValidateCreditNoteUpdate(CreditNote creditNote)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        ValidateUpdateWrite(creditNote.ShowProjectName, creditNote.Status, nameof(creditNote));
    }

    /// <summary>
    /// Whether <c>show_project_name</c> should be included in an invoice update payload.
    /// </summary>
    internal static bool ShouldIncludeOnInvoiceUpdate(InvoiceStatus? status) => status == InvoiceStatus.Draft;

    /// <summary>
    /// Whether <c>show_project_name</c> should be included in a credit note update payload.
    /// </summary>
    internal static bool ShouldIncludeOnCreditNoteUpdate(CreditNoteStatus? status) => status == CreditNoteStatus.Draft;

    private static void ValidateUpdateWrite<TStatus>(bool? showProjectName, TStatus? status, string parameterName)
        where TStatus : struct
    {
        if (!showProjectName.HasValue)
        {
            return;
        }

        if (status is null)
        {
            throw new ArgumentException(
                "ShowProjectName can only be sent on update when Status is Draft. " +
                "Set Status on the update model (typically from a GET response) before setting ShowProjectName.",
                parameterName);
        }
    }
}
