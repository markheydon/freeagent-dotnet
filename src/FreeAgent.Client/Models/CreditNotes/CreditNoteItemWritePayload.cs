using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Writable credit note line item attributes for create and update requests.
/// </summary>
internal sealed class CreditNoteItemWritePayload
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("position")]
    public decimal? Position { get; set; }

    [JsonPropertyName("item_type")]
    [JsonConverter(typeof(NullableInvoiceItemTypeJsonConverter))]
    public InvoiceItemType? ItemType { get; set; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("sales_tax_rate")]
    public decimal? SalesTaxRate { get; set; }

    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; set; }

    [JsonPropertyName("sales_tax_status")]
    public InvoiceSalesTaxStatus? SalesTaxStatus { get; set; }

    [JsonPropertyName("second_sales_tax_status")]
    public string? SecondSalesTaxStatus { get; set; }

    [JsonPropertyName("stock_item")]
    public StockItemReference? StockItem { get; set; }

    [JsonPropertyName("category")]
    public CategoryReference? Category { get; set; }

    [JsonPropertyName("project")]
    public ProjectReference? Project { get; set; }

    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("_destroy")]
    public int? Destroy { get; set; }

    public static CreditNoteItemWritePayload FromCreditNoteItem(CreditNoteItem item, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new CreditNoteItemWritePayload
        {
            Url = item.ItemId is null ? item.Url : null,
            Position = item.Position,
            ItemType = item.ItemType,
            Quantity = item.Quantity,
            Description = item.Description,
            Price = item.Price,
            SalesTaxRate = item.SalesTaxRate,
            SecondSalesTaxRate = item.SecondSalesTaxRate,
            SalesTaxStatus = item.SalesTaxStatus,
            SecondSalesTaxStatus = item.SecondSalesTaxStatus,
            StockItem = LinkedResourceWriteMapper.ToStockItemReference(environment, item.StockItemId),
            Category = LinkedResourceWriteMapper.ToCategoryReference(environment, item.CategoryNominalCode),
            Project = LinkedResourceWriteMapper.ToProjectReference(environment, item.ProjectId),
            Id = item.ItemId,
            Destroy = item.Destroy
        };
    }
}
