using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.PriceListItems;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.PriceListItems;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.PriceListItems;

public class PriceListItemsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPriceListItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/price_list_items", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "price_list_items": [
                    {
                      "url": "https://api.freeagent.com/v2/price_list_items/1",
                      "code": "A001",
                      "item_type": "Products",
                      "quantity": "1.0",
                      "price": "10.99",
                      "description": "Apple",
                      "vat_status": "standard",
                      "category": "https://api.freeagent.com/v2/categories/2",
                      "created_at": "2023-05-22T09:22:45Z",
                      "updated_at": "2023-05-25T12:43:36Z"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync();

        Assert.Single(items);
        Assert.Equal("A001", items[0].Code);
        Assert.Equal(InvoiceItemType.Products, items[0].ItemType);
        Assert.Equal(PriceListItemVatStatus.Standard, items[0].VatStatus);
        Assert.Equal("2", items[0].CategoryNominalCode);
    }

    [Fact]
    public async Task ListAsync_WithSort_IncludesQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=code", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "price_list_items": [] }""")
            };
        });

        var service = CreateService(handler);
        await service.ListAsync(sort: PriceListItemSortOptions.Code);
    }

    [Fact]
    public async Task GetPriceListItemAsync_ReturnsPriceListItem()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/price_list_items/17", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "price_list_item": {
                    "url": "https://api.freeagent.com/v2/price_list_items/17",
                    "code": "A001",
                    "item_type": "Products",
                    "quantity": "1.0",
                    "price": "1.99",
                    "description": "Apple",
                    "vat_status": "standard"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var item = await service.GetPriceListItemAsync(17);

        Assert.Equal("A001", item.Code);
        Assert.Equal(17, item.ResourceId);
    }

    [Fact]
    public async Task CreatePriceListItemAsync_SerialisesRequiredFields()
    {
        var handler = CreatePriceListItemPostHandler(body =>
        {
            Assert.Contains("\"code\":\"A001\"", body, StringComparison.Ordinal);
            Assert.Contains("\"item_type\":\"Products\"", body, StringComparison.Ordinal);
            Assert.Contains("\"description\":\"Apple\"", body, StringComparison.Ordinal);
            Assert.Contains("\"price\":1.99", body, StringComparison.Ordinal);
            Assert.Contains("\"vat_status\":\"standard\"", body, StringComparison.Ordinal);
            Assert.Contains("\"category\":\"https://api.freeagent.com/v2/categories/2\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("stock_item", body, StringComparison.Ordinal);
        }, """
            {
              "price_list_item": {
                "url": "https://api.freeagent.com/v2/price_list_items/17",
                "code": "A001",
                "item_type": "Products",
                "quantity": "1.0",
                "price": "1.99",
                "description": "Apple",
                "vat_status": "standard"
              }
            }
            """);

        var service = CreateService(handler);
        var created = await service.CreatePriceListItemAsync(new CreatePriceListItemRequest
        {
            Code = "A001",
            Quantity = 1,
            ItemType = InvoiceItemType.Products,
            Description = "Apple",
            Price = 1.99m,
            VatStatus = PriceListItemVatStatus.Standard,
            Category = CategoryReference.Parse("https://api.freeagent.com/v2/categories/2")
        });

        Assert.Equal("A001", created.Code);
    }

    [Fact]
    public async Task CreatePriceListItemAsync_WithStockItem_SerialisesStockItemLink()
    {
        var handler = CreatePriceListItemPostHandler(body =>
        {
            Assert.Contains("\"item_type\":\"Stock\"", body, StringComparison.Ordinal);
            Assert.Contains("\"stock_item\":\"https://api.freeagent.com/v2/stock_items/3\"", body, StringComparison.Ordinal);
        }, """
            {
              "price_list_item": {
                "url": "https://api.freeagent.com/v2/price_list_items/18",
                "code": "S001",
                "item_type": "Stock",
                "quantity": "1.0",
                "price": "5.00",
                "description": "Widget",
                "stock_item": "https://api.freeagent.com/v2/stock_items/3"
              }
            }
            """);

        var service = CreateService(handler);
        var created = await service.CreatePriceListItemAsync(new CreatePriceListItemRequest
        {
            Code = "S001",
            Quantity = 1,
            ItemType = InvoiceItemType.Stock,
            Description = "Widget",
            Price = 5m,
            StockItem = StockItemReference.Parse("https://api.freeagent.com/v2/stock_items/3")
        });

        Assert.Equal(InvoiceItemType.Stock, created.ItemType);
        Assert.Equal(3, created.StockItemId);
    }

    [Fact]
    public async Task UpdatePriceListItemAsync_SerialisesOnlySuppliedFields()
    {
        var handler = CreatePriceListItemPutHandler(17, body =>
        {
            Assert.Contains("\"description\":\"Pear\"", body, StringComparison.Ordinal);
            Assert.Contains("\"price\":3.99", body, StringComparison.Ordinal);
            Assert.DoesNotContain("code", body, StringComparison.Ordinal);
        }, """
            {
              "price_list_item": {
                "url": "https://api.freeagent.com/v2/price_list_items/17",
                "code": "A001",
                "item_type": "Products",
                "quantity": "1.0",
                "price": "3.99",
                "description": "Pear"
              }
            }
            """);

        var service = CreateService(handler);
        var updated = await service.UpdatePriceListItemAsync(17, new UpdatePriceListItemRequest
        {
            Description = "Pear",
            Price = 3.99m
        });

        Assert.Equal("Pear", updated.Description);
    }

    [Fact]
    public async Task DeletePriceListItemAsync_UsesPluralPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/price_list_items/17", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.DeletePriceListItemAsync(17);
    }

    [Fact]
    public async Task GetPriceListItemAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetPriceListItemAsync(17));
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

    private static PriceListItemsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new PriceListItemsService(client);
    }

    private static QueueHttpMessageHandler CreatePriceListItemPostHandler(Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/price_list_items", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            Assert.Contains("\"price_list_item\"", body, StringComparison.Ordinal);
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson)
            };
        })]);

    private static QueueHttpMessageHandler CreatePriceListItemPutHandler(long priceListItemId, Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith($"/price_list_items/{priceListItemId}", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            };
        })]);
}
