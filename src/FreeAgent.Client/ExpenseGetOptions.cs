namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Expenses.ExpenseService.GetExpenseAsync(long, ExpenseGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class ExpenseGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the user when the response contains only a user URI.
    /// </summary>
    public bool IncludeUser { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the category when the response contains only a category URI.
    /// </summary>
    public bool IncludeCategory { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the rebill-to project when the response contains only a URI.
    /// </summary>
    public bool IncludeRebillToProject { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the stock item when the response contains only a stock item URI.
    /// </summary>
    public bool IncludeStockItem { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the invoice when the response contains only an invoice URI.
    /// </summary>
    public bool IncludeRebilledOnInvoice { get; init; }
}
