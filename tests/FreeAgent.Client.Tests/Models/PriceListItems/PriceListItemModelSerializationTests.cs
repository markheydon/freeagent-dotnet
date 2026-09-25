using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.PriceListItems;

namespace FreeAgent.Client.Tests.Models.PriceListItems;

public class PriceListItemModelSerializationTests
{
    [Fact]
    public void DeserializePriceListItem_MapsStockItemUriLink()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/price_list_items/18",
              "code": "S001",
              "item_type": "Stock",
              "quantity": "1.0",
              "price": "5.00",
              "description": "Widget",
              "stock_item": "https://api.freeagent.com/v2/stock_items/3"
            }
            """;

        var item = JsonSerializer.Deserialize<PriceListItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(InvoiceItemType.Stock, item!.ItemType);
        Assert.Equal(3, item.StockItemId);
        Assert.Null(item.StockItemResource);
    }

    [Fact]
    public void DeserializePriceListItem_MapsNestedStockItem()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/price_list_items/18",
              "code": "S001",
              "item_type": "Stock",
              "quantity": "1.0",
              "price": "5.00",
              "description": "Widget",
              "stock_item": {
                "url": "https://api.freeagent.com/v2/stock_items/3",
                "description": "Widget stock"
              }
            }
            """;

        var item = JsonSerializer.Deserialize<PriceListItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(3, item!.StockItemId);
        Assert.Equal("Widget stock", item.StockItemResource?.Description);
    }
}
