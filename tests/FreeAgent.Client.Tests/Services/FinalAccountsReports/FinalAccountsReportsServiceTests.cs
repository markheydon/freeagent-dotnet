using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.FinalAccountsReports;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.FinalAccountsReports;

public class FinalAccountsReportsServiceTests
{
    [Fact]
    public async Task ListAsync_CallsFinalAccountsReportsEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/final_accounts_reports", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "final_accounts_reports": [
                    {
                      "url": "https://api.freeagent.com/v2/final_accounts_reports/2023-12-31",
                      "period_ends_on": "2023-12-31",
                      "filing_status": "draft"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var reports = await service.ListAsync();

        Assert.Single(reports);
        Assert.Equal(new DateOnly(2023, 12, 31), reports[0].PeriodEndsOn);
        Assert.Equal(FinalAccountsReportFilingStatus.Draft, reports[0].FilingStatus);
    }

    [Fact]
    public async Task GetAsync_UsesPeriodEndsOnInPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/final_accounts_reports/2022-12-31", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "final_accounts_report": {
                    "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
                    "period_ends_on": "2022-12-31",
                    "filing_status": "pending"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var report = await service.GetAsync(new DateOnly(2022, 12, 31));

        Assert.Equal(FinalAccountsReportFilingStatus.Pending, report.FilingStatus);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenFinalAccountsReportsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Theory]
    [InlineData("mark_as_filed", FinalAccountsReportFilingStatus.MarkedAsFiled)]
    [InlineData("mark_as_unfiled", FinalAccountsReportFilingStatus.Unfiled)]
    public async Task TransitionAsync_CallsDocumentedPutEndpoint(string action, FinalAccountsReportFilingStatus expectedStatus)
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/v2/final_accounts_reports/2022-12-31/{action}", request.RequestUri!.AbsolutePath);

            var filingStatus = action == "mark_as_filed" ? "marked_as_filed" : "unfiled";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "final_accounts_report": {
                    "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
                    "period_ends_on": "2022-12-31",
                    "filing_status": "{{filingStatus}}"
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
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expectedStatus, result.FilingStatus);
    }

    private static FinalAccountsReportsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new FinalAccountsReportsService(client);
    }
}
