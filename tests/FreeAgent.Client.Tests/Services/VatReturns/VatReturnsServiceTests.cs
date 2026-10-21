using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.VatReturns;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.VatReturns;

public class VatReturnsServiceTests
{
    [Fact]
    public async Task ListAsync_CallsVatReturnsEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/vat_returns", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "vat_returns": [
                    {
                      "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
                      "period_ends_on": "2023-07-31",
                      "filing_status": "filed"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var returns = await service.ListAsync();

        Assert.Single(returns);
        Assert.Equal(new DateOnly(2023, 7, 31), returns[0].PeriodEndsOn);
    }

    [Fact]
    public async Task GetAsync_UsesPeriodEndsOnInPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/vat_returns/2023-07-31", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "vat_return": {
                    "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
                    "period_ends_on": "2023-07-31",
                    "breakdown": { "title": "VAT Return", "rows": [] }
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var vatReturn = await service.GetAsync(new DateOnly(2023, 7, 31));

        Assert.Equal("VAT Return", vatReturn.Breakdown?.Title);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenVatReturnsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    private static VatReturnsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new VatReturnsService(client);
    }
}
