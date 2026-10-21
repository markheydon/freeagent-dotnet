using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.IncomeTaxReturns;
using FreeAgent.Client.Services.IncomeTaxReturns;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.IncomeTaxReturns;

public class IncomeTaxReturnsServiceTests
{
    private const long UserId = 119;

    [Fact]
    public async Task ListAsync_CallsUserSelfAssessmentReturnsEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal($"/v2/users/{UserId}/self_assessment_returns", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "self_assessment_returns": [
                    {
                      "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2023-04-05",
                      "period_ends_on": "2023-04-05",
                      "filing_status": "unfiled"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var returns = await service.ListAsync(UserId);

        Assert.Single(returns);
        Assert.Equal(new DateOnly(2023, 4, 5), returns[0].PeriodEndsOn);
        Assert.Equal(IncomeTaxReturnFilingStatus.Unfiled, returns[0].FilingStatus);
    }

    [Fact]
    public async Task GetAsync_UsesPeriodEndsOnInPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal($"/v2/users/{UserId}/self_assessment_returns/2022-04-05", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "self_assessment_return": {
                    "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2022-04-05",
                    "period_ends_on": "2022-04-05",
                    "payments": [
                      {
                        "label": "Balancing Payment",
                        "amount_due": "6181.82",
                        "due_on": "2024-01-31",
                        "status": "unpaid"
                      }
                    ],
                    "filing_status": "unfiled"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var incomeTaxReturn = await service.GetAsync(UserId, new DateOnly(2022, 4, 5));

        Assert.Equal(6181.82m, incomeTaxReturn.Payments![0].AmountDue);
        Assert.Equal(IncomeTaxReturnPaymentStatus.Unpaid, incomeTaxReturn.Payments[0].Status);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenIncomeTaxReturnsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync(UserId));
    }

    [Theory]
    [InlineData("mark_as_filed", IncomeTaxReturnFilingStatus.MarkedAsFiled)]
    [InlineData("mark_as_unfiled", IncomeTaxReturnFilingStatus.Unfiled)]
    public async Task ReturnTransitionAsync_CallsDocumentedPutEndpoint(string action, IncomeTaxReturnFilingStatus expectedStatus)
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal(
                $"/v2/users/{UserId}/self_assessment_returns/2022-04-05/{action}",
                request.RequestUri!.AbsolutePath);

            var filingStatus = action == "mark_as_filed" ? "marked_as_filed" : "unfiled";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "self_assessment_return": {
                    "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2022-04-05",
                    "period_ends_on": "2022-04-05",
                    "filing_status": "{{filingStatus}}"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var period = new DateOnly(2022, 4, 5);

        var result = action switch
        {
            "mark_as_filed" => await service.MarkAsFiledAsync(UserId, period),
            "mark_as_unfiled" => await service.MarkAsUnfiledAsync(UserId, period),
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expectedStatus, result.FilingStatus);
    }

    [Theory]
    [InlineData("mark_as_paid", IncomeTaxReturnPaymentStatus.MarkedAsPaid)]
    [InlineData("mark_as_unpaid", IncomeTaxReturnPaymentStatus.Unpaid)]
    public async Task PaymentTransitionAsync_CallsDocumentedPutEndpoint(string action, IncomeTaxReturnPaymentStatus expectedStatus)
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal(
                $"/v2/users/{UserId}/self_assessment_returns/2022-04-05/payments/2024-01-31/{action}",
                request.RequestUri!.AbsolutePath);

            var paymentStatus = action == "mark_as_paid" ? "marked_as_paid" : "unpaid";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "self_assessment_return": {
                    "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2022-04-05",
                    "period_ends_on": "2022-04-05",
                    "payments": [
                      {
                        "label": "Balancing Payment",
                        "amount_due": "6181.82",
                        "due_on": "2024-01-31",
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
        var period = new DateOnly(2022, 4, 5);
        var paymentDate = new DateOnly(2024, 1, 31);

        var result = action switch
        {
            "mark_as_paid" => await service.MarkPaymentAsPaidAsync(UserId, period, paymentDate),
            "mark_as_unpaid" => await service.MarkPaymentAsUnpaidAsync(UserId, period, paymentDate),
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expectedStatus, result.Payments![0].Status);
    }

    private static IncomeTaxReturnsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new IncomeTaxReturnsService(client);
    }
}
