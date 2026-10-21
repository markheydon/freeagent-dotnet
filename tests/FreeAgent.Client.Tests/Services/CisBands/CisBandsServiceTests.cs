using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.CisBands;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CisBands;

public class CisBandsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsCisBands()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/cis_bands", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "available_bands": [
                    {
                      "name": "cis_gross",
                      "deduction_rate": "0.0",
                      "income_description": "CIS Income (Gross)",
                      "deduction_description": "CIS Deduction (Gross)",
                      "nominal_code": "061"
                    },
                    {
                      "name": "cis_standard",
                      "deduction_rate": "0.2",
                      "income_description": "CIS Income (20%)",
                      "deduction_description": "CIS Deduction (20%)",
                      "nominal_code": "062"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync();

        Assert.Equal(2, items.Count);
        Assert.Equal(CisDeductionRate.Gross, items[0].Name);
        Assert.Equal(0m, items[0].DeductionRate);
        Assert.Equal("061", items[0].NominalCode);
        Assert.Equal(CisDeductionRate.Standard, items[1].Name);
        Assert.Equal(0.2m, items[1].DeductionRate);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenAvailableBandsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
        Assert.Contains("CIS bands", exception.Message, StringComparison.Ordinal);
    }

    private static CisBandsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CisBandsService(client);
    }
}
