using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Attributes for creating a price list item.
/// </summary>
public sealed class CreatePriceListItemRequest
{
    /// <summary>
    /// Unique code used to identify the item when adding it to an invoice or estimate.
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Item quantity.
    /// </summary>
    [JsonPropertyName("quantity")]
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Line item type.
    /// </summary>
    [JsonPropertyName("item_type")]
    public required InvoiceItemType ItemType { get; set; }

    /// <summary>
    /// Free-text description of the item.
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    /// <summary>
    /// Unit price of one item.
    /// </summary>
    [JsonPropertyName("price")]
    public required decimal Price { get; set; }

    /// <summary>
    /// UK VAT status for the item.
    /// </summary>
    [JsonPropertyName("vat_status")]
    public PriceListItemVatStatus? VatStatus { get; set; }

    /// <summary>
    /// Standard sales tax rate for universal and US accounts.
    /// </summary>
    [JsonPropertyName("sales_tax_rate")]
    public decimal? SalesTaxRate { get; set; }

    /// <summary>
    /// Second sales tax rate for universal accounts.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; set; }

    /// <summary>
    /// Income accounting category.
    /// </summary>
    [JsonPropertyName("category")]
    public CategoryReference? Category { get; set; }

    /// <summary>
    /// Stock item when <see cref="ItemType"/> is <see cref="InvoiceItemType.Stock"/>.
    /// </summary>
    [JsonPropertyName("stock_item")]
    public StockItemReference? StockItem { get; set; }
}
