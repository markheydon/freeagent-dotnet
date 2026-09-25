namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Invoices.InvoiceService.UpdateInvoiceAsync(long, Models.Invoices.Invoice, InvoiceUpdateOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class InvoiceUpdateOptions
{
    /// <summary>
    /// When <see langword="true"/>, line items are excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitLineItems { get; init; }

    /// <summary>
    /// When <see langword="true"/>, the contact link is excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, the project link is excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitProject { get; init; }

    /// <summary>
    /// When <see langword="true"/>, the bank account link is excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitBankAccount { get; init; }
}
