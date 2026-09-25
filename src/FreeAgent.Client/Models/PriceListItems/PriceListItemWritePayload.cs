using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.PriceListItems;

/// <summary>
/// Writable price list item attributes for create and update requests.
/// </summary>
internal sealed class PriceListItemWritePayload
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    [JsonPropertyName("item_type")]
    [JsonConverter(typeof(NullableInvoiceItemTypeJsonConverter))]
    public InvoiceItemType? ItemType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("vat_status")]
    public PriceListItemVatStatus? VatStatus { get; set; }

    [JsonPropertyName("sales_tax_rate")]
    public decimal? SalesTaxRate { get; set; }

    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; set; }

    [JsonPropertyName("category")]
    public CategoryReference? Category { get; set; }

    [JsonPropertyName("stock_item")]
    public StockItemReference? StockItem { get; set; }

    public static PriceListItemWritePayload FromCreate(CreatePriceListItemRequest request, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PriceListItemWritePayload
        {
            Code = request.Code,
            Quantity = request.Quantity,
            ItemType = request.ItemType,
            Description = request.Description,
            Price = request.Price,
            VatStatus = request.VatStatus,
            SalesTaxRate = request.SalesTaxRate,
            SecondSalesTaxRate = request.SecondSalesTaxRate,
            Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode),
            StockItem = LinkedResourceWriteMapper.ToStockItemReference(environment, request.StockItemId)
        };
    }

    public static PriceListItemWritePayload FromUpdate(UpdatePriceListItemRequest request, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PriceListItemWritePayload
        {
            Code = request.Code,
            Quantity = request.Quantity,
            ItemType = request.ItemType,
            Description = request.Description,
            Price = request.Price,
            VatStatus = request.VatStatus,
            SalesTaxRate = request.SalesTaxRate,
            SecondSalesTaxRate = request.SecondSalesTaxRate,
            Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode),
            StockItem = LinkedResourceWriteMapper.ToStockItemReference(environment, request.StockItemId)
        };
    }
}
