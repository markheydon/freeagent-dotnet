namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Invoices.InvoiceService.GetInvoiceAsync(long, InvoiceGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class InvoiceGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the contact when the invoice response contains only a contact URI.
    /// </summary>
    public bool IncludeContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the invoice response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
