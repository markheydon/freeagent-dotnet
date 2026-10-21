using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.CapitalAssets;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CapitalAssets;

public class CapitalAssetsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsCapitalAssets()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/capital_assets", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.Equal(string.Empty, request.RequestUri.Query);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "capital_assets": [
                    {
                      "url": "https://api.freeagent.com/v2/capital_assets/1",
                      "description": "Computer",
                      "asset_life_years": 2,
                      "asset_type": "Computer Equipment Purchase",
                      "purchased_on": "2015-10-07"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync();

        Assert.Single(items);
        Assert.Equal("Computer", items[0].Description);
        Assert.Equal(1, items[0].ResourceId);
    }

    [Fact]
    public async Task ListAsync_WithViewAndIncludeHistory_BuildsQueryString()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("view=disposed", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("include_history=true", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "capital_assets": [
                    {
                      "url": "https://api.freeagent.com/v2/capital_assets/2",
                      "description": "Monitor",
                      "capital_asset_history": [
                        {
                          "type": "purchase",
                          "date": "2019-08-19",
                          "value": "1403.0",
                          "link": "https://api.freeagent.com/v2/expenses/30"
                        }
                      ]
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync(CapitalAssetView.Disposed, includeHistory: true);

        Assert.Single(items);
        Assert.NotNull(items[0].CapitalAssetHistory);
        Assert.Equal(30, items[0].CapitalAssetHistory![0].ExpenseId);
    }

    [Fact]
    public async Task GetCapitalAssetAsync_ReturnsCapitalAsset()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/capital_assets/3", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.Equal(string.Empty, request.RequestUri.Query);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "capital_asset": {
                    "url": "https://api.freeagent.com/v2/capital_assets/3",
                    "description": "Workstation"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var item = await service.GetCapitalAssetAsync(3);

        Assert.Equal("Workstation", item.Description);
        Assert.Equal(3, item.ResourceId);
    }

    [Fact]
    public async Task GetCapitalAssetAsync_WithIncludeHistory_BuildsQueryString()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/capital_assets/9", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.Contains("include_history=true", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "capital_asset": {
                    "url": "https://api.freeagent.com/v2/capital_assets/9",
                    "description": "Laptop",
                    "capital_asset_history": []
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var item = await service.GetCapitalAssetAsync(
            9,
            new CapitalAssetGetOptions { IncludeHistory = true });

        Assert.Equal("Laptop", item.Description);
        Assert.NotNull(item.CapitalAssetHistory);
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

    private static CapitalAssetsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CapitalAssetsService(client);
    }
}
