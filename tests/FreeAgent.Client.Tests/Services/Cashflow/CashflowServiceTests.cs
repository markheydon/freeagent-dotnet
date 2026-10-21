using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Cashflow;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Cashflow;

public class CashflowServiceTests
{
    [Fact]
    public async Task GetAsync_BuildsQueryStringAndDeserialises()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/cashflow", request.RequestUri!.AbsolutePath);
            Assert.Contains("from_date=2019-07-01", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2019-09-30", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "cashflow": {
                    "from": "2019-07-01",
                    "to": "2019-09-30",
                    "balance": "12593.21",
                    "incoming": { "total": "68869.76", "months": [] },
                    "outgoing": { "total": "56276.55", "months": [] }
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var cashflow = await service.GetCashflowAsync(new DateOnly(2019, 7, 1), new DateOnly(2019, 9, 30));

        Assert.Equal(12593.21m, cashflow.Balance);
    }

    [Fact]
    public async Task GetAsync_ThrowsWhenCashflowMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() =>
            service.GetCashflowAsync(new DateOnly(2019, 1, 1), new DateOnly(2019, 1, 31)));

        Assert.Contains("Cashflow", exception.Message, StringComparison.Ordinal);
    }

    private static CashflowService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CashflowService(client);
    }
}
