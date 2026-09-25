using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.StockItems;
using FreeAgent.Client.Services.StockItems;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.StockItems;

public class StockItemsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsStockItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/stock_items", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "stock_items": [
                    {
                      "url": "https://api.freeagent.com/v2/stock_items/3",
                      "description": "Apple",
                      "opening_quantity": "10.0",
                      "opening_balance": "1.0",
                      "cost_of_sale_category": "https://api.freeagent.com/v2/categories/2",
                      "stock_on_hand": "10.0",
                      "created_at": "2023-05-22T09:22:45Z",
                      "updated_at": "2023-05-25T12:43:36Z"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var stockItems = await service.ListAsync();

        Assert.Single(stockItems);
        Assert.Equal("Apple", stockItems[0].Description);
        Assert.Equal(10m, stockItems[0].OpeningQuantity);
        Assert.Equal(10m, stockItems[0].StockOnHand);
        Assert.Equal("2", stockItems[0].CostOfSaleCategoryNominalCode);
        Assert.Equal(3, stockItems[0].ResourceId);
    }

    [Fact]
    public async Task ListAsync_WithSort_IncludesQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-description", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "stock_items": [] }""")
            };
        });

        var service = CreateService(handler);
        await service.ListAsync(sort: $"-{StockItemSortOptions.Description}");
    }

    [Fact]
    public async Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async Task GetStockItemAsync_ReturnsStockItem()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/stock_items/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "stock_item": {
                    "url": "https://api.freeagent.com/v2/stock_items/42",
                    "description": "Pear",
                    "opening_quantity": "5.0",
                    "opening_balance": "2.5",
                    "cost_of_sale_category": "https://api.freeagent.com/v2/categories/3",
                    "stock_on_hand": "5.0",
                    "created_at": "2023-05-22T09:22:45Z",
                    "updated_at": "2023-05-25T12:43:36Z"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var stockItem = await service.GetStockItemAsync(42);

        Assert.Equal("Pear", stockItem.Description);
        Assert.Equal(42, stockItem.ResourceId);
    }

    [Fact]
    public async Task GetStockItemAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetStockItemAsync(1));
    }

    private static StockItemsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new StockItemsService(client);
    }
}
