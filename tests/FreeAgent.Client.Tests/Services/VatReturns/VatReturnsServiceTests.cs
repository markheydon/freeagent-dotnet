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
        var vatReturn = await service.GetVatReturnAsync(new DateOnly(2023, 7, 31));

        Assert.Equal("VAT Return", vatReturn.Breakdown?.Title);
    }

    [Theory]
    [InlineData("mark_as_filed", VatReturnFilingStatus.MarkedAsFiled)]
    [InlineData("mark_as_unfiled", VatReturnFilingStatus.Unfiled)]
    public async Task TransitionReturnAsync_CallsDocumentedPutEndpoint(string action, VatReturnFilingStatus expectedStatus)
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/v2/vat_returns/2023-07-31/{action}", request.RequestUri!.AbsolutePath);

            var filingStatus = action == "mark_as_filed" ? "marked_as_filed" : "unfiled";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "vat_return": {
                    "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
                    "period_ends_on": "2023-07-31",
                    "filing_status": "{{filingStatus}}"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var period = new DateOnly(2023, 7, 31);

        var result = action switch
        {
            "mark_as_filed" => await service.MarkAsFiledAsync(period),
            "mark_as_unfiled" => await service.MarkAsUnfiledAsync(period),
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expectedStatus, result.FilingStatus);
    }

    [Theory]
    [InlineData("mark_as_paid", VatReturnPaymentStatus.MarkedAsPaid)]
    [InlineData("mark_as_unpaid", VatReturnPaymentStatus.Unpaid)]
    public async Task PaymentTransitionAsync_CallsDocumentedPutEndpoint(string action, VatReturnPaymentStatus expectedStatus)
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal(
                $"/v2/vat_returns/2023-07-31/payments/2023-09-07/{action}",
                request.RequestUri!.AbsolutePath);

            var paymentStatus = action == "mark_as_paid" ? "marked_as_paid" : "unpaid";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "vat_return": {
                    "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
                    "period_ends_on": "2023-07-31",
                    "payments": [
                      {
                        "label": "Payment Due",
                        "amount_due": "5450.91",
                        "due_on": "2023-09-07",
                        "status": "{{paymentStatus}}"
                      }
                    ],
                    "filing_status": "unfiled"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var period = new DateOnly(2023, 7, 31);
        var paymentDueOn = new DateOnly(2023, 9, 7);

        var result = action switch
        {
            "mark_as_paid" => await service.MarkPaymentAsPaidAsync(period, paymentDueOn),
            "mark_as_unpaid" => await service.MarkPaymentAsUnpaidAsync(period, paymentDueOn),
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expectedStatus, result.Payments![0].Status);
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
