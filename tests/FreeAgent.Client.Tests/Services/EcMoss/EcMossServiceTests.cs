using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.EcMoss;
using FreeAgent.Client.Services.EcMoss;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.EcMoss;

public class EcMossServiceTests
{
    [Fact]
    public async Task GetSalesTaxRatesAsync_BuildsCountryAndDateQuery()
    {
        string? requestPath = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            requestPath = request.RequestUri!.PathAndQuery;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "sales_tax_rates": [
                    { "percentage": "20.0", "band": "Standard" }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var rates = await service.GetSalesTaxRatesAsync(EcMossPlaceOfSupply.Austria, new DateOnly(2017, 1, 1));

        Assert.Contains("country=Austria", requestPath, StringComparison.Ordinal);
        Assert.Contains("date=2017-01-01", requestPath, StringComparison.Ordinal);
        Assert.Single(rates);
        Assert.Equal(20.0m, rates[0].Percentage);
        Assert.Equal("Standard", rates[0].Band);
    }

    private static EcMossService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new EcMossService(client);
    }
}
