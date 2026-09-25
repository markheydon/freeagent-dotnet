namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.RecurringInvoices.RecurringInvoiceService.GetRecurringInvoiceAsync(long, RecurringInvoiceGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class RecurringInvoiceGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the contact when the response contains only a contact URI.
    /// </summary>
    public bool IncludeContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
