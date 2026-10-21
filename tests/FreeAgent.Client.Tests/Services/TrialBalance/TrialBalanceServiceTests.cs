using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.TrialBalance;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.TrialBalance;

public class TrialBalanceServiceTests
{
    [Fact]
    public async Task ListSummaryAsync_WithoutFilters_CallsSummaryEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/trial_balance/summary", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "trial_balance_summary": [
                    {
                      "category": "https://api.freeagent.com/v2/categories/101",
                      "nominal_code": "101",
                      "display_nominal_code": "101",
                      "name": "Cost of Sales",
                      "total": "275.0"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var lines = await service.ListSummaryAsync();

        Assert.Single(lines);
        Assert.Equal(275m, lines[0].Total);
    }

    [Fact]
    public async Task ListSummaryAsync_WithDateFilters_BuildsQueryString()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("from_date=2014-01-01", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2014-05-01", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "trial_balance_summary": [] }""")
            };
        });

        var service = CreateService(handler);
        var lines = await service.ListSummaryAsync(
            fromDate: new DateOnly(2014, 1, 1),
            toDate: new DateOnly(2014, 5, 1));

        Assert.Empty(lines);
    }

    [Fact]
    public async Task ListSummaryAsync_ThrowsWhenSummaryMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListSummaryAsync());
        Assert.Contains("Trial balance", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListOpeningBalancesAsync_CallsOpeningBalancesEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/trial_balance/summary/opening_balances", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "trial_balance_summary": [] }""")
            };
        });

        var service = CreateService(handler);
        await service.ListOpeningBalancesAsync();
    }

    private static TrialBalanceService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new TrialBalanceService(client);
    }
}
