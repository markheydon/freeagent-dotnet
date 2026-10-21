using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.BalanceSheet;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.BalanceSheet;

public class BalanceSheetServiceTests
{
    [Fact]
    public async Task GetAsync_WithoutFilter_CallsBalanceSheetEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/balance_sheet", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "balance_sheet": {
                    "currency": "GBP",
                    "total_assets": 100,
                    "total_owners_equity": -100
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var sheet = await service.GetAsync();

        Assert.Equal("GBP", sheet.Currency);
        Assert.Equal(100, sheet.TotalAssets);
    }

    [Fact]
    public async Task GetAsync_WithAsAtDate_BuildsQueryString()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("as_at_date=2023-09-30", request.RequestUri!.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                { "balance_sheet": { "currency": "GBP" } }
                """)
            };
        });

        var service = CreateService(handler);
        await service.GetAsync(asAtDate: new DateOnly(2023, 9, 30));
    }

    [Fact]
    public async Task GetAsync_ThrowsWhenBalanceSheetMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetAsync());
        Assert.Contains("Balance sheet", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetOpeningBalancesAsync_CallsOpeningBalancesEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/balance_sheet/opening_balances", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "balance_sheet": {
                    "currency": "GBP",
                    "total_assets": 0
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var sheet = await service.GetOpeningBalancesAsync();

        Assert.Equal(0, sheet.TotalAssets);
    }

    [Fact]
    public async Task GetOpeningBalancesAsync_ThrowsWhenBalanceSheetMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetOpeningBalancesAsync());
        Assert.Contains("opening balances", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static BalanceSheetService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new BalanceSheetService(client);
    }
}
