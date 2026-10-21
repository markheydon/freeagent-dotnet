using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.CorporationTaxReturns;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CorporationTaxReturns;

public class CorporationTaxReturnsServiceTests
{
    [Fact]
    public async Task ListAsync_CallsCorporationTaxReturnsEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/corporation_tax_returns", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "corporation_tax_returns": [
                    {
                      "url": "https://api.freeagent.com/v2/corporation_tax_returns/2023-12-31",
                      "period_ends_on": "2023-12-31",
                      "filing_status": "draft"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var returns = await service.ListAsync();

        Assert.Single(returns);
        Assert.Equal(new DateOnly(2023, 12, 31), returns[0].PeriodEndsOn);
        Assert.Equal(CorporationTaxReturnFilingStatus.Draft, returns[0].FilingStatus);
    }

    [Fact]
    public async Task GetAsync_UsesPeriodEndsOnInPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/corporation_tax_returns/2022-12-31", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "corporation_tax_return": {
                    "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
                    "period_ends_on": "2022-12-31",
                    "amount_due": "2724.03",
                    "payment_status": "unpaid"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var corporationTaxReturn = await service.GetCorporationTaxReturnAsync(new DateOnly(2022, 12, 31));

        Assert.Equal(2724.03m, corporationTaxReturn.AmountDue);
        Assert.Equal(CorporationTaxReturnPaymentStatus.Unpaid, corporationTaxReturn.PaymentStatus);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenCorporationTaxReturnsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Theory]
    [InlineData("mark_as_filed", CorporationTaxReturnFilingStatus.MarkedAsFiled)]
    [InlineData("mark_as_unfiled", CorporationTaxReturnFilingStatus.Unfiled)]
    [InlineData("mark_as_paid", CorporationTaxReturnPaymentStatus.MarkedAsPaid)]
    [InlineData("mark_as_unpaid", CorporationTaxReturnPaymentStatus.Unpaid)]
    public async Task TransitionAsync_CallsDocumentedPutEndpoint(string action, object expectedStatus)
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/v2/corporation_tax_returns/2022-12-31/{action}", request.RequestUri!.AbsolutePath);

            var filingStatus = action is "mark_as_filed" or "mark_as_unfiled"
                ? $"\"{(action == "mark_as_filed" ? "marked_as_filed" : "unfiled")}\""
                : "\"filed\"";
            var paymentStatus = action is "mark_as_paid" or "mark_as_unpaid"
                ? $"\"{(action == "mark_as_paid" ? "marked_as_paid" : "unpaid")}\""
                : "null";
            var paymentLine = action is "mark_as_paid" or "mark_as_unpaid"
                ? $",\"payment_status\": {paymentStatus}"
                : string.Empty;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "corporation_tax_return": {
                    "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
                    "period_ends_on": "2022-12-31",
                    "filing_status": {{filingStatus}}{{paymentLine}}
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var period = new DateOnly(2022, 12, 31);

        var result = action switch
        {
            "mark_as_filed" => await service.MarkAsFiledAsync(period),
            "mark_as_unfiled" => await service.MarkAsUnfiledAsync(period),
            "mark_as_paid" => await service.MarkAsPaidAsync(period),
            "mark_as_unpaid" => await service.MarkAsUnpaidAsync(period),
            _ => throw new InvalidOperationException()
        };

        if (expectedStatus is CorporationTaxReturnFilingStatus filing)
        {
            Assert.Equal(filing, result.FilingStatus);
        }
        else
        {
            Assert.Equal((CorporationTaxReturnPaymentStatus)expectedStatus, result.PaymentStatus);
        }
    }

    private static CorporationTaxReturnsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CorporationTaxReturnsService(client);
    }
}
