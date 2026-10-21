using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.HirePurchases;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.HirePurchases;

public class HirePurchasesServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsHirePurchases()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/hire_purchases", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "hire_purchases": [
                    {
                      "url": "https://api.freeagent.com/v2/hire_purchases/1",
                      "description": "My hire purchase bill",
                      "bill": "https://api.freeagent.com/v2/bills/1",
                      "liabilities_over_one_year_category": "https://api.freeagent.com/v2/categories/793-1",
                      "liabilities_under_one_year_category": "https://api.freeagent.com/v2/categories/792-1"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var hirePurchases = await service.ListAsync();

        Assert.Single(hirePurchases);
        Assert.Equal("My hire purchase bill", hirePurchases[0].Description);
        Assert.Equal(1, hirePurchases[0].ResourceId);
        Assert.Equal(1, hirePurchases[0].BillId);
        Assert.Equal("793-1", hirePurchases[0].LiabilitiesOverOneYearCategoryNominalCode);
        Assert.Equal("792-1", hirePurchases[0].LiabilitiesUnderOneYearCategoryNominalCode);
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
    public async Task GetHirePurchaseAsync_ReturnsHirePurchase()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/hire_purchases/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "hire_purchase": {
                    "url": "https://api.freeagent.com/v2/hire_purchases/42",
                    "description": "Office equipment",
                    "bill": "https://api.freeagent.com/v2/bills/7",
                    "liabilities_over_one_year_category": "https://api.freeagent.com/v2/categories/793-1",
                    "liabilities_under_one_year_category": "https://api.freeagent.com/v2/categories/792-1"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var hirePurchase = await service.GetHirePurchaseAsync(42);

        Assert.Equal("Office equipment", hirePurchase.Description);
        Assert.Equal(42, hirePurchase.ResourceId);
        Assert.Equal(7, hirePurchase.BillId);
    }

    [Fact]
    public async Task GetHirePurchaseAsync_WithHydration_FetchesBill()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "hire_purchase": {
                    "url": "https://api.freeagent.com/v2/hire_purchases/42",
                    "bill": "https://api.freeagent.com/v2/bills/7"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bill": {
                    "url": "https://api.freeagent.com/v2/bills/7",
                    "reference": "HP-BILL"
                  }
                }
                """)
            });

        var service = CreateService(handler);
        var hirePurchase = await service.GetHirePurchaseAsync(42, new HirePurchaseGetOptions { IncludeBill = true });

        Assert.Equal("HP-BILL", hirePurchase.Bill?.Reference);
    }

    [Fact]
    public async Task GetHirePurchaseAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetHirePurchaseAsync(1));
    }

    private static HirePurchasesService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new HirePurchasesService(client);
    }
}
