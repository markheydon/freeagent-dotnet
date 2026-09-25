using FreeAgent.Client.Models.Invoices;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Attributes for creating a price list item.
/// </summary>
public sealed class CreatePriceListItemRequest
{
    /// <summary>
    /// Unique code used to identify the item when adding it to an invoice or estimate.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Item quantity.
    /// </summary>
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Line item type.
    /// </summary>
    public required InvoiceItemType ItemType { get; set; }

    /// <summary>
    /// Free-text description of the item.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Unit price of one item.
    /// </summary>
    public required decimal Price { get; set; }

    /// <summary>
    /// UK VAT status for the item.
    /// </summary>
    public PriceListItemVatStatus? VatStatus { get; set; }

    /// <summary>
    /// Standard sales tax rate for universal and US accounts.
    /// </summary>
    public decimal? SalesTaxRate { get; set; }

    /// <summary>
    /// Second sales tax rate for universal accounts.
    /// </summary>
    public decimal? SecondSalesTaxRate { get; set; }

    /// <summary>
    /// Income accounting category nominal code.
    /// </summary>
    public string? CategoryNominalCode { get; set; }

    /// <summary>
    /// Stock item identifier when <see cref="ItemType"/> is <see cref="InvoiceItemType.Stock"/>.
    /// </summary>
    public long? StockItemId { get; set; }
}
