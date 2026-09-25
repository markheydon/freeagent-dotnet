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
}
