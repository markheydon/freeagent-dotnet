using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Writable invoice line item attributes for create and update requests.
/// </summary>
internal sealed class InvoiceItemWritePayload
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

    public static InvoiceItemWritePayload FromInvoiceItem(InvoiceItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        CategoryReference? category = item.Category;
        if (category is null && item.CategoryLink?.Uri is string categoryUri)
        {
            category = CategoryReference.Parse(categoryUri);
        }

        ProjectReference? project = item.LinkedProject;
        if (project is null && item.ProjectLink?.Uri is string projectUri)
        {
            project = ProjectReference.Parse(projectUri);
        }

        StockItemReference? stockItem = item.StockItem;
        if (stockItem is null && item.StockItemLink?.Uri is string stockItemUri)
        {
            stockItem = StockItemReference.Parse(stockItemUri);
        }

        return new InvoiceItemWritePayload
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
            StockItem = stockItem,
            Category = category,
            Project = project,
            Id = item.ItemId,
            Destroy = item.Destroy
        };
    }
}
