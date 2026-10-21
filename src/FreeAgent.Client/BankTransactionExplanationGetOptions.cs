namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.BankTransactionExplanations.BankTransactionExplanationsService.GetBankTransactionExplanationAsync(long, BankTransactionExplanationGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class BankTransactionExplanationGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the category when the explanation response contains only a category URI.
    /// </summary>
    public bool IncludeCategory { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the explanation response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the bank account when the explanation response contains only a bank account URI.
    /// </summary>
    public bool IncludeBankAccount { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the paid invoice when <c>paid_invoice</c> links to an invoice URI.
    /// </summary>
    public bool IncludePaidInvoice { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the paid credit note when <c>paid_invoice</c> links to a credit note URI.
    /// </summary>
    public bool IncludePaidCreditNote { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the paid bill when the explanation response contains only a bill URI.
    /// </summary>
    public bool IncludePaidBill { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the paid user when the explanation response contains only a user URI.
    /// </summary>
    public bool IncludePaidUser { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the direct contact when the explanation response contains only a contact URI.
    /// </summary>
    public bool IncludeDirectContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the transfer bank account when the explanation response contains only a transfer account URI.
    /// </summary>
    public bool IncludeTransferBankAccount { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the stock item when the explanation response contains only a stock item URI.
    /// </summary>
    public bool IncludeStockItem { get; init; }
}
