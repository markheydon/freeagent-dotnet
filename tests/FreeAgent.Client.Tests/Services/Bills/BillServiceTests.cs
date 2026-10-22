using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Bills;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Bills;

public class BillServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=open", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bills": [
                    {
                      "url": "https://api.freeagent.com/v2/bills/1",
                      "reference": "001",
                      "status": "Open"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/bills/2",
                      "reference": "002",
                      "status": "Paid"
                    }
                  ]
                }
                """)
            };
            response.Headers.TryAddWithoutValidation("X-Total-Count", "5");
            return response;
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: BillView.Open);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("001", page.Items[0].Reference);
        Assert.Equal(BillStatus.Paid, page.Items[1].Status);
    }

    [Fact]
    public async Task ListAsync_IncludesFiltersAndNestedItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("from_date=2024-01-01", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2024-12-31", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested_bill_items=true", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "bills": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await service.ListAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 12, 31),
            updatedSince: new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            contactId: 2,
            projectId: 3,
            nestedBillItems: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task ListAsync_InvalidPerPage_Throws(int perPage)
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(perPage: perPage));
    }

    [Fact]
    public async Task ListAsync_PageBelowOne_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(page: 0));
    }

    [Fact]
    public async Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "bills": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async Task GetBillAsync_ReturnsBill()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/bills/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bill": {
                    "url": "https://api.freeagent.com/v2/bills/42",
                    "reference": "007",
                    "status": "Open"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        var bill = await service.GetBillAsync(42);

        Assert.Equal("007", bill.Reference);
        Assert.Equal(BillStatus.Open, bill.Status);
    }

    [Fact]
    public async Task GetBillAsync_WithHydration_FetchesRebilledInvoice()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bill": {
                    "url": "https://api.freeagent.com/v2/bills/42",
                    "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoices/99"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/99",
                    "reference": "REBILL"
                  }
                }
                """)
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        var bill = await service.GetBillAsync(42, new BillGetOptions { IncludeRebilledOnInvoice = true });

        Assert.Equal("REBILL", bill.RebilledOnInvoice?.Reference);
    }

    [Fact]
    public async Task GetBillAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "bill": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetBillAsync(1));
    }

    [Fact]
    public async Task CreateBillAsync_PostsBillPayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/bills", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bill": {
                    "url": "https://api.freeagent.com/v2/bills/3",
                    "status": "Open"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        var created = await service.CreateBillAsync(new Bill
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            DueOn = new DateOnly(2024, 4, 18),
            BillItems =
            [
                new BillItem
                {
                    Description = "Supplier charge",
                    TotalValue = 120m,
                    CategoryNominalCode = "285"
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"bill_items\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Supplier charge\"", postedJson, StringComparison.Ordinal);
        Assert.Equal(BillStatus.Open, created.Status);
    }

    [Fact]
    public async Task CreateBillAsync_ThrowsWhenLineItemHasStockQuantityWithoutStockItem()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        var bill = new Bill
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            DueOn = new DateOnly(2024, 4, 18),
            BillItems =
            [
                new BillItem
                {
                    Description = "Stock line",
                    TotalValue = 10m,
                    CategoryNominalCode = "285",
                    StockAlteringQuantity = 1m
                }
            ]
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateBillAsync(bill));
        Assert.Contains("Stock item and stock altering quantity", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UpdateBillAsync_PostsItemUpdateWithBillItemUrl()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/bills/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bill": {
                    "url": "https://api.freeagent.com/v2/bills/5",
                    "status": "Open"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await service.UpdateBillAsync(5, new Bill
        {
            BillItems =
            [
                new BillItem
                {
                    Url = "https://api.freeagent.com/v2/bill_items/10",
                    Description = "Updated supplier charge",
                    TotalValue = 90m
                },
                new BillItem
                {
                    Url = "https://api.freeagent.com/v2/bill_items/11",
                    Destroy = 1
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"url\":\"https://api.freeagent.com/v2/bill_items/10\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Updated supplier charge\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"_destroy\":1", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListAutoPagingAsync_YieldsAllPages()
    {
        var handler = new QueueHttpMessageHandler(
            request =>
            {
                Assert.Contains("page=1", request.RequestUri!.Query);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "bills": [
                        { "url": "https://api.freeagent.com/v2/bills/1" },
                        { "url": "https://api.freeagent.com/v2/bills/2" }
                      ]
                    }
                    """)
                };
                response.Headers.TryAddWithoutValidation("X-Total-Count", "3");
                return response;
            },
            request =>
            {
                Assert.Contains("page=2", request.RequestUri!.Query);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "bills": [
                        { "url": "https://api.freeagent.com/v2/bills/3" }
                      ]
                    }
                    """)
                };
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        var ids = new List<long>();
        await foreach (var bill in service.ListAutoPagingAsync(perPage: 2))
        {
            ids.Add(bill.ResourceId);
        }

        Assert.Equal([1, 2, 3], ids);
    }

    [Fact]
    public async Task DeleteBillAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/bills/8", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await service.DeleteBillAsync(8);
    }

    [Fact]
    public async Task ListAsync_ContactAndContactIdBothSpecified_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
            contactId: 1));
    }

    [Fact]
    public async Task ListAsync_ProjectAndProjectIdBothSpecified_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BillService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/1"),
            projectId: 1));
    }
}
