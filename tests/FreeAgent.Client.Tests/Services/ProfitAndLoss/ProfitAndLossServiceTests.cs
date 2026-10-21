using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.ProfitAndLoss;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.ProfitAndLoss;

public class ProfitAndLossServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_WithoutFilters_CallsSummaryEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/profit_and_loss/summary", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "profit_and_loss_summary": {
                    "from": "2024-04-06",
                    "to": "2024-10-21",
                    "income": "100",
                    "expenses": "50",
                    "operating_profit": "50"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var summary = await service.GetSummaryAsync();

        Assert.Equal(100m, summary.Income);
        Assert.Equal(50m, summary.OperatingProfit);
    }

    [Fact]
    public async Task GetSummaryAsync_WithDateFilters_BuildsQueryString()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("from_date=2024-01-01", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2024-03-31", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                { "profit_and_loss_summary": { "income": "0", "expenses": "0" } }
                """)
            };
        });

        var service = CreateService(handler);
        await service.GetSummaryAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 3, 31));
    }

    [Fact]
    public async Task GetSummaryAsync_ThrowsWhenSummaryMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetSummaryAsync());
        Assert.Contains("Profit and loss", exception.Message, StringComparison.Ordinal);
    }

    private static ProfitAndLossService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new ProfitAndLossService(client);
    }
}
