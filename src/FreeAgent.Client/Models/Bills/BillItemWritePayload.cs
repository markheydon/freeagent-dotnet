using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Writable bill line item attributes for create and update requests.
/// </summary>
internal sealed class BillItemWritePayload
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(WriteLinkJsonConverter<CategoryReference>))]
    public WriteLink<CategoryReference>? Category { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("total_value")]
    public decimal? TotalValue { get; set; }

    [JsonPropertyName("total_value_ex_tax")]
    public decimal? TotalValueExTax { get; set; }

    [JsonPropertyName("manual_sales_tax_amount")]
    public decimal? ManualSalesTaxAmount { get; set; }

    [JsonPropertyName("sales_tax_rate")]
    public PurchaseSalesTaxRate? SalesTaxRate { get; set; }

    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; set; }

    [JsonPropertyName("sales_tax_status")]
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    [JsonPropertyName("second_sales_tax_status")]
    public SecondSalesTaxStatus? SecondSalesTaxStatus { get; set; }

    [JsonPropertyName("unit")]
    public BillItemUnit? Unit { get; set; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    [JsonPropertyName("stock_item")]
    [JsonConverter(typeof(WriteLinkJsonConverter<StockItemReference>))]
    public WriteLink<StockItemReference>? StockItem { get; set; }

    [JsonPropertyName("stock_altering_quantity")]
    public decimal? StockAlteringQuantity { get; set; }

    [JsonPropertyName("project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? Project { get; set; }

    [JsonPropertyName("_destroy")]
    public int? Destroy { get; set; }

    [JsonPropertyName("capital_asset")]
    public CapitalAssetPurchaseWritePayload? CapitalAsset { get; set; }

    public static BillItemWritePayload FromBillItem(
        BillItem item,
        FreeAgentEnvironment environment,
        bool isUpdate)
    {
        ArgumentNullException.ThrowIfNull(item);

        string? url = null;
        if (isUpdate)
        {
            if (item.Destroy is not null || item.Url is not null || item.ItemId is not null)
            {
                url = item.Url;
                if (string.IsNullOrEmpty(url) && item.ItemId is long itemId)
                {
                    url = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}bill_items/{itemId}";
                }

                if (item.Destroy is null && string.IsNullOrEmpty(url))
                {
                    url = string.Empty;
                }
            }
        }
        else if (!string.IsNullOrEmpty(item.Url))
        {
            url = item.Url;
        }

        return new BillItemWritePayload
        {
            Url = url,
            Category = LinkedResourceWriteMapper.ResolveCategoryReference(
                environment,
                item.CategoryNominalCodeBacking,
                item.CategoryLinkNominalCode),
            Description = item.Description,
            TotalValue = item.TotalValue,
            TotalValueExTax = item.TotalValueExTax,
            ManualSalesTaxAmount = item.ManualSalesTaxAmount,
            SalesTaxRate = item.SalesTaxRate,
            SecondSalesTaxRate = item.SecondSalesTaxRate,
            SalesTaxStatus = item.SalesTaxStatus,
            SecondSalesTaxStatus = item.SecondSalesTaxStatus,
            Unit = item.Unit,
            Quantity = item.Quantity,
            StockItem = LinkedResourceWriteMapper.ResolveStockItemReference(
                environment,
                item.StockItemIdBacking,
                item.StockItemLinkId),
            StockAlteringQuantity = item.StockAlteringQuantity,
            Project = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                item.ProjectIdBacking,
                item.ProjectLinkId),
            Destroy = item.Destroy,
            CapitalAsset = CapitalAssetPurchaseWritePayload.FromWrite(item.CapitalAssetPurchase)
        };
    }
}
