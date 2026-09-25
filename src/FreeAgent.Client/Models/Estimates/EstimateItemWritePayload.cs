using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Writable estimate line item attributes for create and update requests.
/// </summary>
internal sealed class EstimateItemWritePayload
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("position")]
    public decimal? Position { get; set; }

    [JsonPropertyName("item_type")]
    [JsonConverter(typeof(NullableEstimateItemTypeJsonConverter))]
    public EstimateItemType? ItemType { get; set; }

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

    [JsonPropertyName("category")]
    [JsonConverter(typeof(WriteLinkJsonConverter<CategoryReference>))]
    public WriteLink<CategoryReference>? Category { get; set; }

    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("_destroy")]
    public int? Destroy { get; set; }

    public static EstimateItemWritePayload FromEstimateItem(EstimateItem item, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new EstimateItemWritePayload
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
            Category = LinkedResourceWriteMapper.ResolveCategoryReference(
                environment,
                item.CategoryNominalCodeBacking,
                item.CategoryLinkNominalCode),
            Id = item.ItemId,
            Destroy = item.Destroy
        };
    }
}
